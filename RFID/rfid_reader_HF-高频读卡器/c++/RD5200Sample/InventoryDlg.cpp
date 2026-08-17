// InventoryDlg.cpp : 实现文件
//

#include "stdafx.h"
#include "RD5200Sample.h"
#include "InventoryDlg.h"
#include "RD5200SampleDlg.h"
#include "CommandDlg.h"


// InventoryDlg 对话框

IMPLEMENT_DYNAMIC(InventoryDlg, CDialog)

InventoryDlg::InventoryDlg(CWnd* pParent /*=NULL*/)
	: CDialog(InventoryDlg::IDD, pParent)
{
	runInventory=false;
}

InventoryDlg::~InventoryDlg()
{
}

void InventoryDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);

	DDX_Control(pDX, IDC_EDIT2, m_StartByte);
	DDX_Control(pDX, IDC_EDIT3, m_numBytes);
	DDX_Control(pDX, IDC_BUTTON1, m_btnStart);
	DDX_Control(pDX, IDC_BUTTON2, m_btnStop);
	DDX_Control(pDX, IDC_LIST1, m_antList);
	DDX_Control(pDX, IDC_STATIC_1, m_tagCnt);
	DDX_Control(pDX, IDC_STATIC_2, m_timeCnt);
	DDX_Control(pDX, IDC_LIST2, m_listUid);
	DDX_Control(pDX, IDC_CHECK1, m_enableRead);
	DDX_Control(pDX, IDC_CK_EAS, m_enableEAS);
	DDX_Control(pDX, IDC_CK_AFI, m_enableAfi);
	DDX_Control(pDX, IDC_EDIT4, m_afi);

}


BEGIN_MESSAGE_MAP(InventoryDlg, CDialog)
	ON_BN_CLICKED(IDC_BUTTON1, &InventoryDlg::OnBnClickedButton1)
	ON_BN_CLICKED(IDC_BUTTON2, &InventoryDlg::OnBnClickedButton2)
	ON_MESSAGE(WM_INVENTORY, &OnInventoryMsg)

END_MESSAGE_MAP()


// InventoryDlg 消息处理程序



DWORD WINAPI inventoryThrd(
						   LPVOID lpThreadParameter
						   )
{
	InventoryDlg *pdlg=(InventoryDlg *)lpThreadParameter;
	pdlg->runInventory=true;

	bool enableRead=false;
	if(pdlg->m_enableRead.GetCheck()==BST_CHECKED)
	{
		enableRead=true;
	}

	int StartByte;
	int numByte;

	CString str;
	pdlg->m_StartByte.GetWindowText(str);

	StartByte=_tstoi(str);
	pdlg->m_numBytes.GetWindowText(str);
	numByte=_tstoi(str);

	CArray<byte,byte> ant;

	for (int i=0;i<pdlg->m_antList.GetCount();i++)
	{
		if(pdlg->m_antList.GetCheck(i))
		{
			ant.Add(i+1);
		}
	}

	RFID_DN_HANDLE	 m_hInvenParamSpecList=RDR_CreateInvenParamSpecList();

	if (!m_hInvenParamSpecList)
	{
		goto exit;
	}

	RFID_DN_HANDLE hIso15693InvenParam=ISO15693_CreateInvenParam(m_hInvenParamSpecList,0,0,0,0);

	if (!hIso15693InvenParam)
	{
		goto exit;
	}
	if (enableRead)
	{
		ISO15693_SetInventoryReadParam(hIso15693InvenParam,0,0);
		ISO15693_AddInventoryReadBlockArea(hIso15693InvenParam,StartByte,numByte);
	}
	RFID_DN_HANDLE	 hDisableEAS=NULL;
	RFID_DN_HANDLE   hWriteAFI=NULL;
	if(pdlg->m_enableEAS.GetCheck()==BST_CHECKED)
	{
		hDisableEAS = NXPICODESLI_CreateTADisableEAS(NULL);
		if (hDisableEAS != NULL) RDR_AddTagAccessToInvenParam(hIso15693InvenParam, hDisableEAS);
	}

	if(pdlg->m_enableAfi.GetCheck()==BST_CHECKED)
	{
		TCHAR buff[100]={0};
		pdlg->m_afi.GetWindowText(buff,100);

		BYTE afival=(BYTE)_tcstol(buff,NULL,16);

		hWriteAFI = ISO15693_CreateTAWriteAFI(NULL, afival);
		if (hWriteAFI != NULL) RDR_AddTagAccessToInvenParam(hIso15693InvenParam, hWriteAFI);
	}


	while(pdlg->runInventory)
	{
		int beginTime=GetTickCount();	
		int iret=RDR_TagInventory(hr,1,(BYTE)ant.GetCount(), ant.GetData(),m_hInvenParamSpecList);

		pdlg->m_ProcTime=GetTickCount()-beginTime;

		if (iret==NO_ERR)
		{
			RFID_DN_HANDLE dnhReport = RDR_GetTagDataReport(hr, RFID_SEEK_FIRST);

			while(dnhReport!=NULL)
			{
				ReportData report;

				iret=ISO15693_ParseTagDataReportEx(dnhReport,&report.aipid,&report.tag_id,&report.ant_id,&report.dsfid,&report.rssi,&report.readCnt,report.uid);

				if (iret==0)
				{

					if(report.tag_id==0)report.tag_id=1;

				
					report.uidStr.Preallocate(50);
					TCHAR *pstr=report.uidStr.GetBuffer();
					memset(pstr,0,50*sizeof(TCHAR));

					BytesToHexStr(report.uid,8,pstr);
					report.uidStr.ReleaseBuffer();

					if (enableRead)
					{

						RDR_ParseTagDataReportBlockData(dnhReport,report.data,&report.dataLen);
					}

					if (hDisableEAS)
					{
						RDR_ParseTagDataReportWriteResult(dnhReport, hDisableEAS,  &(report.EAScmdRes));
					}
					if (hWriteAFI)
					{
						RDR_ParseTagDataReportWriteResult(dnhReport, hWriteAFI,&(report.AFIcmdRes));
					}	

					pdlg->SendMessage(WM_INVENTORY,(WPARAM)&report,enableRead);
				}

				dnhReport=RDR_GetTagDataReport(hr,RFID_SEEK_NEXT);
			}
		}

	}

exit:
	RDR_ResetCommuImmeTimeout(hr);
	pdlg->m_btnStart.EnableWindow(true);	
	pdlg->m_btnStop.EnableWindow(false);

	return 0;
}

void InventoryDlg::OnBnClickedButton1()
{
	// TODO: 在此添加控件通知处理程序代码
	CString str=_T("Tag:0");
	m_tagCnt.SetWindowText(str);
	str=_T("Time:0");
	m_timeCnt.SetWindowText(str);

	m_listUid.DeleteAllItems();

	m_btnStart.EnableWindow(false);
	m_btnStop.EnableWindow(true);

	CRD5200SampleDlg* pMainWnd = (CRD5200SampleDlg *)AfxGetMainWnd();

	CCheckListBox *plist=&((CommandDlg *)pMainWnd->m_tabDlgs[1])->uidList;

	for (int i=0;i<plist->GetCount();i++)
	{
		ReportData * ptr=(ReportData *)plist->GetItemData(i);	

		if(ptr!=NULL)
			delete ptr;
	}
	

	plist->ResetContent();


	CreateThread(NULL,NULL,inventoryThrd,this,NULL,NULL);
}



BOOL InventoryDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	m_listUid.InsertColumn(0,_T("Antenna"),LVCFMT_LEFT,50);
	m_listUid.InsertColumn(1,_T("UID"),LVCFMT_LEFT,150);
	m_listUid.InsertColumn(2,_T("RSSI"),LVCFMT_LEFT,50);
	m_listUid.InsertColumn(3,_T("Data"),LVCFMT_LEFT,180);
	m_listUid.InsertColumn(4,_T("Read Cnt"),LVCFMT_LEFT,100);
	m_listUid.InsertColumn(5,_T("EAS"),LVCFMT_LEFT,100);
	m_listUid.InsertColumn(6,_T("AFI"),LVCFMT_LEFT,100);

	m_StartByte.SetWindowText(_T("0"));
	m_numBytes.SetWindowText(_T("4"));
	m_enableRead.SetCheck(BST_CHECKED);

	m_btnStart.EnableWindow(FALSE);
	m_btnStop.EnableWindow(false);


	m_afi.SetWindowText(_T("aa"));


	return true;
}

LRESULT InventoryDlg::OnInventoryMsg(WPARAM wParam, LPARAM lParam)
{

	CRD5200SampleDlg* pMainWnd = (CRD5200SampleDlg *)AfxGetMainWnd();


	ReportData *report=(ReportData *)wParam;	
	bool enableRead=lParam != 0;

	CString reportUID;
	reportUID.Preallocate(50);
	TCHAR *pstr=reportUID.GetBuffer();
	memset(pstr,0,50*sizeof(TCHAR));

	BytesToHexStr(report->uid,8,pstr);
	reportUID.ReleaseBuffer();

	int i=0;
	for ( i=0;i<m_listUid.GetItemCount();i++)
	{

		CString t_uidStr=m_listUid.GetItemText(i,1);

		if (t_uidStr==reportUID&&_tstoi(m_listUid.GetItemText(i,0))==report->ant_id)
		{
			break;	
		}
	}

	if (i==m_listUid.GetItemCount())
	{
		CString strAnt,strRSSI,strData;
		strAnt.Format(_T("%d"),report->ant_id);
		strRSSI.Format(_T("%d"),report->rssi);

		BytesToHexStr(report->uid,8,pstr);
		reportUID.ReleaseBuffer();

		if (enableRead)
		{
			for (DWORD j=0;j< report->dataLen;j++)
			{
				strData.AppendFormat(_T("%02X"),report->data[j]);

			}
		}

		m_listUid.InsertItem(m_listUid.GetItemCount(),strAnt);
		m_listUid.SetItemText(m_listUid.GetItemCount()-1,1,reportUID);
		m_listUid.SetItemText(m_listUid.GetItemCount()-1,2,strRSSI);
		m_listUid.SetItemText(m_listUid.GetItemCount()-1,3,strData);		
		m_listUid.SetItemText(m_listUid.GetItemCount()-1,4,_T("1"));
		
		CString str;
		str.Format(_T("%d"),report->EAScmdRes);
		m_listUid.SetItemText(m_listUid.GetItemCount()-1,5,str);
		
		str.Format(_T("%d"),report->AFIcmdRes);
		m_listUid.SetItemText(m_listUid.GetItemCount()-1,6,str);
		
		

			

		CCheckListBox *plist=&(((CommandDlg *)(pMainWnd->m_tabDlgs[1]))->uidList);
			
		plist->AddString(report->uidStr);

		
		ReportData *itemData=new ReportData();			

	    *itemData=*report;
		int cnt=plist->GetCount();
		plist->SetItemData(plist->GetCount()-1,(DWORD_PTR)itemData);

	


	}
	else
	{
		CString strRSSI;

		TCHAR buff[100]={0};
		m_listUid.GetItemText(i,4,buff,100);
		int readCnt=_tstoi(buff);
		readCnt+=1;
		if (readCnt>=100000)
		{
			readCnt=1;
		}
		_stprintf(buff,_T("%d"),readCnt);

		strRSSI.Format(_T("%d"),report->rssi);

		m_listUid.SetItemText(i,2,strRSSI);
		m_listUid.SetItemText(i,4,buff);

		CString str;
		str.Format(_T("%d"),report->EAScmdRes);
		m_listUid.SetItemText(i,5,str);
		
		str.Format(_T("%d"),report->AFIcmdRes);
		m_listUid.SetItemText(i,6,str);	
		

		CCheckListBox *plist=&(((CommandDlg *)(pMainWnd->m_tabDlgs[1]))->uidList);

		ReportData *itemData=NULL;	
		itemData=(ReportData *)plist->GetItemData(i);

		
		itemData->ant_id=report->ant_id;
		itemData->tag_id=report->tag_id;
	}

	
	


	CString str;
	str.Format(_T("Tag:%d"),m_listUid.GetItemCount());
	this->m_tagCnt.SetWindowText(str);
	str.Format(_T("Time:%d"),m_ProcTime);
	this->m_timeCnt.SetWindowText(str);

	return true;
}



void InventoryDlg::OnBnClickedButton2()
{
	// TODO: 在此添加控件通知处理程序代码

	runInventory=false;
	RDR_SetCommuImmeTimeout(hr);

}
