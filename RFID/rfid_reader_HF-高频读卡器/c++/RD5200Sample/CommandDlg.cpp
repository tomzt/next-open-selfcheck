// ommandDlg.cpp : 实现文件
//

#include "stdafx.h"
#include "RD5200Sample.h"
#include "CommandDlg.h"


// CommandDlg 对话框

IMPLEMENT_DYNAMIC(CommandDlg, CDialog)

CommandDlg::CommandDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CommandDlg::IDD, pParent)
{

}

CommandDlg::~CommandDlg()
{
	
	for (int i=0;i<uidList.GetCount();i++)
	{
		ReportData * ptr=(ReportData *)uidList.GetItemData(i);	

		if(ptr!=NULL)
			delete ptr;
	}

}

void CommandDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LIST1, uidList);
	DDX_Control(pDX, IDC_CHECK1, enableReadBlock);
	DDX_Control(pDX, IDC_EDIT1, startReadBlock);
	DDX_Control(pDX, IDC_EDIT5, numOfReadBlocks);
	DDX_Control(pDX, IDC_CHECK2, enableWriteBlock);
	DDX_Control(pDX, IDC_EDIT6, startWriteBlock);
	DDX_Control(pDX, IDC_EDIT7, numOfWriteBlock);
	DDX_Control(pDX, IDC_EDIT2, dataOfWriteBlock);
	DDX_Control(pDX, IDC_CHECK3, enableWriteAfi);

	DDX_Control(pDX, IDC_CHECK4, enableChangeEAS);
	DDX_Control(pDX, IDC_COMBO3, EASOption);
	DDX_Control(pDX, IDC_RICHEDIT21, logCommand);
	
	DDX_Control(pDX, IDC_EDIT_AFIVAL, afiedit);
}


BEGIN_MESSAGE_MAP(CommandDlg, CDialog)
	ON_BN_CLICKED(IDC_BUTTON1, &CommandDlg::OnBnClickedButton1)
END_MESSAGE_MAP()


// CommandDlg 消息处理程序



bool HasCmdReadData = false;
bool HasCmdWriteData = false;
bool HasCmdWriteAFI = false;
bool HasCmdEas = false;

bool CmdEnableEas = false;
RFID_DN_HANDLE pTagSet=NULL;

//
//  函数CharToInt说明
//  转字符成数值
//
int CharToInt(TCHAR val)
{
	int ret=-1;
	if(val>='0' && val <='9') 
	{
		ret=val-0x30 ;
	}
	if(val>='A'  && val<='F')
	{
		ret=val-'A'+ 0x0a;
	}
	if(val>='a' && val<='f')
	{
		ret=val-'a'+0x0a ;
	}

	return ret;

}



//
//  函数HexStrToBytes说明
//  转字符串"12220E" 字节数组{0x12,0x22,0x0e}
//
bool HexStrToBytes(const TCHAR *strBuf,BYTE *byBuf,int *byLen)
{
	int                      slen=0 ;
	int                      byteCount;
	int                      index=0 ;
	BYTE                     btmp ;
	int                      val;
	int                      itmp =0;

	slen=(int)_tcslen(strBuf) ;	 
	if(slen<2)              return false;
	//不是2的倍数
	if((slen % 2)!=0)       return false;

	byteCount=slen / 2 ;
	itmp=byteCount ;
	//全部转大写
	//CharToUpper(strBuf) ;
	while(byteCount)
	{
		btmp=0 ;
		val=CharToInt(strBuf[index]) ;
		if(val==-1)  return false;
		btmp=(val << 4) & 0xf0;

		val=CharToInt(strBuf[index+1]) ;
		if(val==-1)  return false;

		btmp=btmp | (val & 0x0f) ;

		byBuf[itmp-byteCount] =btmp;

		index=index+2;

		byteCount-- ;
	}

	if(byLen!=NULL) *byLen=itmp ;

	return true;
}


DWORD WINAPI cmdExceThrd( LPVOID lpThreadParameter)
{

	CommandDlg *pDlg=(CommandDlg *)lpThreadParameter;


	int iret=RDR_AccessMultipleTags(hr,pTagSet);

	if(iret!=0)
	{
		CString str=_T("");
		str.Format(_T("RDR_AccessMultipleTags fail code:%d"),iret);
		pDlg->PrintLog(str,RGB(255,0,0));

	}
	else
	{
		int iret=RDR_SeekAccessTag(pTagSet,0);

		if(iret!=0)
		{
			goto exit_point;
		}
		else
		{
			int idx=0;	
			for (int i=0;i<pDlg->uidList.GetCount();i++)
			{
				if(pDlg->uidList.GetCheck(i)==BST_CHECKED)
				{

					CString uid;

					pDlg->uidList.GetText(i,uid);


					BYTE flag=RFID_SEEK_FIRST;

					if(HasCmdReadData)
					{
						RFID_DN_HANDLE ptrCmd = NULL;

						ptrCmd = RDR_GetTagAccessCommand(pTagSet, flag);

						flag=RFID_SEEK_NEXT;
						if(ptrCmd==NULL)
							goto exit_point;

						DWORD numofblock=0;
						byte buffBlock[1024]={0};
						DWORD len=1024;


						int iret=ISO15693_ParseReadMultiBlocksResult(ptrCmd,&numofblock,buffBlock,&len);

						if(iret==0)
						{
							if(numofblock!=0)
							{
								CString blockstr;
								blockstr.Preallocate(50);
								TCHAR *pstr=blockstr.GetBuffer();
								memset(pstr,0,50*sizeof(TCHAR));
								BytesToHexStr(buffBlock,len,pstr);
								blockstr.ReleaseBuffer();

								CString str=_T("");
								str.Format(_T("[%s]块数量:%d 数据:%s"),uid,numofblock,blockstr);
								pDlg->PrintLog(str,RGB(0,0,0));
							}
						}
						else
						{
							CString str=_T("");
							str.Format(_T("[%s] ISO15693_ParseReadMultiBlocksResult error:%s"),uid,iret);
							pDlg->PrintLog(str,RGB(255,0,0));

							goto exit_point;

						}
					}
					if(HasCmdWriteData)
					{
						RFID_DN_HANDLE ptrCmd = NULL;
						ptrCmd = RDR_GetTagAccessCommand(pTagSet, flag);

						flag=RFID_SEEK_NEXT;
						if(ptrCmd==NULL)
							goto exit_point;

						int iret=ISO15693_ParseWriteMultipleBlocksResult(ptrCmd);

						if(iret==0)
						{
							CString str;
							str.Format(_T("[%s] 写数据成功"),uid);
							pDlg->PrintLog(str,RGB(0,0,0));
						}
						else
						{
							CString str;
							str.Format(_T("[%s] 写数据失败 error:%d"),uid,iret);
							pDlg->PrintLog(str,RGB(255,0,0));
						}

					}
					if(HasCmdWriteAFI)
					{
						RFID_DN_HANDLE ptrCmd = NULL;
						ptrCmd = RDR_GetTagAccessCommand(pTagSet, flag);

						flag=RFID_SEEK_NEXT;
						if(ptrCmd==NULL)
							goto exit_point;

						int iret=ISO15693_ParseWriteAFIResult(ptrCmd);

						CString str;
						COLORREF clr;

						if(iret==0)
						{
							str.Format(_T("[%s] 写AFI成功"),uid);
							clr=RGB(0,0,0);
						}
						else
						{
							str.Format(_T("[%s] 写AFI失败 error:%d"),uid,iret);
							clr=RGB(0,0,0);	
						}
						pDlg->PrintLog(str,clr);

					}

					if(HasCmdEas)
					{
						RFID_DN_HANDLE ptrCmd = NULL;
						ptrCmd =RDR_GetTagAccessCommand(pTagSet, flag);

						flag=RFID_SEEK_NEXT;
						if(ptrCmd==NULL)
							goto exit_point;


						CString str;
						COLORREF clr;
						if(!CmdEnableEas)
						{
							iret=NXPICODESLI_ParseDisableEASResult(ptrCmd);

							if(iret==0)
							{
								str.Format(_T("[%s] Disable EAS成功"),uid);	

								clr=RGB(0,0,0);
							}
							else
							{
								str.Format(_T("[%s] Disable EAS失败 error:%d"),uid,iret);	
								clr=RGB(255,0,0);
							}

						}
						else
						{
							iret = NXPICODESLI_ParseEableEASResult(ptrCmd);

							if(iret==0)
							{
								str.Format(_T("[%s] Enable EAS成功"),uid);	
								clr=RGB(0,0,0);
							}
							else
							{
								str.Format(_T("[%s] Enable EAS失败 error:%d"),uid,iret);	
								clr=RGB(255,0,0);
							}	
						}

						pDlg->PrintLog(str,clr);
					}

					idx++;		
					iret=RDR_SeekAccessTag(pTagSet,idx);		
				}
			}
		}
	}
exit_point:

	DNODE_Destroy(pTagSet);
	pTagSet=NULL;


	return 0;
}




void CommandDlg::OnBnClickedButton1()
{
	// TODO: 在此添加控件通知处理程序代码


	bool inheritTagType = false;
	bool inheritAnt = false;
	bool inheritCmd = false;

	bool firstCheckItem=true;
	int  lastCheckItemIndex=0;


	if(enableReadBlock.GetCheck()==BST_CHECKED)
		HasCmdReadData=TRUE;
	else
		HasCmdReadData=false;

	if(enableWriteBlock.GetCheck()==BST_CHECKED)
		HasCmdWriteData=TRUE;
	else
		HasCmdWriteData=false;


	if(enableWriteAfi.GetCheck()==BST_CHECKED)
		HasCmdWriteAFI=TRUE;
	else
		HasCmdWriteAFI=false;

	if(enableChangeEAS.GetCheck()==BST_CHECKED)
	{    HasCmdEas=true;
		
		if(EASOption.GetCurSel()==1)
			CmdEnableEas=true;
		else
			CmdEnableEas=false;
	}
	else
		HasCmdEas=false;


	CString str;
	startReadBlock.GetWindowText(str);
	byte readStartBlock=_tstoi(str);

	numOfReadBlocks.GetWindowText(str);
	byte readBlockNum=_tstoi(str);
	

	startWriteBlock.GetWindowText(str);
	byte writeStartBlock=_tstoi(str);
	
	numOfWriteBlock.GetWindowText(str);
	byte writeBlockNum=_tstoi(str);

	dataOfWriteBlock.GetWindowText(str);	
	
	byte writeBuff[1024]={0};
	int len=1024;
	HexStrToBytes(str,writeBuff,&len);


	afiedit.GetWindowText(str);
		
	BYTE afival=(BYTE)_tcstol(str,NULL,16);


	pTagSet=CreateMultipleAccessTagSet(0,0,0,100);
	for (int i=0;i<uidList.GetCount();i++)
	{
	
		if(uidList.GetCheck(i)==BST_CHECKED)
		{	
			CString uid=_T("");
			uidList.GetText(i,uid);	

			ReportData *ItemReport;

		

			ItemReport=(ReportData *)uidList.GetItemData(i);


			if(firstCheckItem)
			{
					inheritTagType=false;
					inheritAnt=false;
					inheritCmd=false;

			}
			else
			{
					inheritCmd=true;

					CString lastItemUid=_T("");
					uidList.GetText(lastCheckItemIndex,lastItemUid);
					ReportData *lastItemReport=NULL;
					lastItemReport=(ReportData *)uidList.GetItemData(lastCheckItemIndex);
					
				
					if(ItemReport->tag_id==lastItemReport->tag_id)
					{
						inheritTagType=true;
					}
					else
					{
						inheritTagType=false;
					}
					
					if(ItemReport->ant_id==lastItemReport->ant_id)
					{
						inheritAnt=true;
					}
					else
					{
						inheritAnt=false;
					}
					
					
			}
			BYTE uidByte[8]={0};
			int uidLen=8;
			HexStrToBytes(uid,uidByte,&uidLen);

			int iret=ISO15693_AddNewAccessTag(pTagSet,1,uidByte,inheritTagType,inheritAnt,inheritCmd);	
			
			if(iret!=0)
			{
				CString str=_T("");
				str.Format(_T("ISO15693_AddNewAccessTag fail code:%d"),iret);

				PrintLog(str,RGB(255,0,0));

				goto exit_fail;

			}
				
			if(!inheritTagType)
			{
				 iret=RDR_SetLastATagTagType(pTagSet,ItemReport->tag_id);
				
				 if(iret!=0)
				 {
					 CString str=_T("");
					 str.Format(_T("RDR_SetLastATagTagType fail code:%d"),iret);

					 PrintLog(str,RGB(255,0,0));

					 goto exit_fail;
				 }

			}

			if(!inheritAnt)
			{
				
				byte ant[1]={0};
				ant[0]=(BYTE)ItemReport->ant_id;

				iret=RDR_SetLastATagAntennas(pTagSet, ant, 1);


				if(iret!=0)
				 {
					 CString str=_T("");
					 str.Format(_T("RDR_SetLastATagAntennas fail code:%d"),iret);

					 PrintLog(str,RGB(255,0,0));

					 goto exit_fail;
				 }
			}

			if(!inheritCmd)
			{
				   if(HasCmdReadData)
				   {

					RFID_DN_HANDLE	ptrCmd=ISO15693_CreateTAReadMultipleBlocks(NULL,false,readStartBlock,readBlockNum);

						if(ptrCmd!=NULL)
						{
							iret=RDR_AddLastATagAccessCommand(pTagSet,ptrCmd);

							if(iret!=0)
							{
								CString str=_T("");
								str.Format(_T("RDR_AddLastATagAccessCommand fail code:%d"),iret);
								PrintLog(str,RGB(255,0,0));
								goto exit_fail;
							}

						}
						else
						{
							CString str=_T("");
							str.Format(_T("ISO15693_CreateTAReadMultipleBlocks fail"));
							PrintLog(str,RGB(255,0,0));
							goto exit_fail;
						}
				   }

				   if(HasCmdWriteData)
				   {
						RFID_DN_HANDLE	ptrCmd=ISO15693_CreateTAWriteMultipleBlocks(NULL,writeStartBlock,writeBlockNum,writeBuff,writeBlockNum*4);

						if(ptrCmd!=NULL)
						{
							iret=RDR_AddLastATagAccessCommand(pTagSet,ptrCmd);

							if(iret!=0)
							{
								CString str=_T("");
								str.Format(_T("RDR_AddLastATagAccessCommand fail code:%d"),iret);
								PrintLog(str,RGB(255,0,0));
								goto exit_fail;
							}
						}
						else
						{
							CString str=_T("");
							str.Format(_T("ISO15693_CreateTAWriteMultipleBlocks fail"));
							PrintLog(str,RGB(255,0,0));
							goto exit_fail;
						}

				   }

				   if(HasCmdWriteAFI)
				   {	
						RFID_DN_HANDLE	ptrCmd=	ISO15693_CreateTAWriteAFI(NULL,afival);

						
						if(ptrCmd!=NULL)
						{
							iret=RDR_AddLastATagAccessCommand(pTagSet,ptrCmd);

							if(iret!=0)
							{
								CString str=_T("");
								str.Format(_T("RDR_AddLastATagAccessCommand fail code:%d"),iret);
								PrintLog(str,RGB(255,0,0));
								goto exit_fail;
							}
						}
						else
						{
							CString str=_T("");
							str.Format(_T("ISO15693_CreateTAWriteAFI fail"));
							PrintLog(str,RGB(255,0,0));
							goto exit_fail;
						}

				   }

				   if(HasCmdEas)
				   {
						RFID_DN_HANDLE ptrCmd=NULL;

						if(CmdEnableEas)
						ptrCmd=NXPICODESLI_CreateTAEableEAS(NULL);
						else
						ptrCmd=NXPICODESLI_CreateTADisableEAS(NULL);

						if(ptrCmd)
						{
							iret=RDR_AddLastATagAccessCommand(pTagSet,ptrCmd);
							
							if(iret!=0)
							{
								CString str=_T("");
								str.Format(_T("RDR_AddLastATagAccessCommand fail code:%d"),iret);
								PrintLog(str,RGB(255,0,0));
								goto exit_fail;
							}
						}
						else
						{
							if(CmdEnableEas)
							{
								CString str=_T("");
								str.Format(_T("NXPICODESLI_CreateTAEableEAS fail"));
								PrintLog(str,RGB(255,0,0));
								goto exit_fail;
							}
							else
							{
								CString str=_T("");
								str.Format(_T("NXPICODESLI_CreateTADisableEAS fail"));
								PrintLog(str,RGB(255,0,0));
								goto exit_fail;
							}

							goto exit_fail;
						}
				   }




			}
			
			lastCheckItemIndex=i;
		}
	}

	CreateThread(NULL,NULL,cmdExceThrd,this,NULL,NULL);

	return;
exit_fail:
	
	if(pTagSet!=NULL)
	{
		DNODE_Destroy(pTagSet);

		pTagSet=NULL;
	}
	return;
}




 BOOL CommandDlg::OnInitDialog()
{
	CDialog::OnInitDialog();
	
	enableReadBlock.SetCheck(BST_CHECKED);
	startReadBlock.SetWindowText(_T("1"));
	numOfReadBlocks.SetWindowText(_T("1"));
	
	enableWriteBlock.SetCheck(BST_CHECKED);
	startWriteBlock.SetWindowText(_T("1"));
	numOfWriteBlock.SetWindowText(_T("1"));
	dataOfWriteBlock.SetWindowText(_T("ffffffff"));
	
	enableWriteAfi.SetCheck(true);

	enableChangeEAS.SetCheck(true);
	EASOption.SetCurSel(0);
	afiedit.SetWindowText(_T("05"));




	return true;
}

 void CommandDlg::PrintLog(CString str,COLORREF color)
 {
	logCommand.SetSel(-1, -1);
	

	CHARFORMAT cf;
	logCommand.GetSelectionCharFormat(cf);
	cf.cbSize = sizeof(cf);
	cf.dwMask = CFM_STRIKEOUT|CFM_COLOR;
	cf.crTextColor=color;
	logCommand.SetSelectionCharFormat(cf);


	str+=_T("\r\n");
	logCommand.ReplaceSel( str );

	logCommand.PostMessage(WM_VSCROLL, SB_BOTTOM,0);
 }

