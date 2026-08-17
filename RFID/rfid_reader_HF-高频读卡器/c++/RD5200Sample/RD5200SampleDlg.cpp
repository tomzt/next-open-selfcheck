// RD5200SampleDlg.cpp : 实现文件
//

#include "stdafx.h"
#include "RD5200Sample.h"
#include "RD5200SampleDlg.h"

#include "InventoryDlg.h"
#include "CommandDlg.h"




#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// 用于应用程序“关于”菜单项的 CAboutDlg 对话框



class CAboutDlg : public CDialog
{
public:
	CAboutDlg();

	// 对话框数据
	enum { IDD = IDD_ABOUTBOX };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

	// 实现
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
END_MESSAGE_MAP()


// CRD5200SampleDlg 对话框


RFID_READER_HANDLE hr=NULL;



CRD5200SampleDlg::CRD5200SampleDlg(CWnd* pParent /*=NULL*/)
: CDialog(CRD5200SampleDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);


}

void CRD5200SampleDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_COMBO14, m_ComType);
	DDX_Control(pDX, IDC_COMBO1, m_comName);
	DDX_Control(pDX, IDC_COMBO2, m_baud);
	DDX_Control(pDX, IDC_COMBO3, m_frame);
	DDX_Control(pDX, IDC_COMBO12, m_usbType);
	DDX_Control(pDX, IDC_COMBO13, m_usbSn);
	DDX_Control(pDX, IDC_IPADDRESS1, m_ip);
	DDX_Control(pDX, IDC_EDIT1, m_port);

	DDX_Control(pDX, IDC_BUTTON3, m_btnOpen);
	DDX_Control(pDX, IDC_BUTTON4, m_btnClose);
	DDX_Control(pDX, IDC_TAB1, m_tab);
	

}

BEGIN_MESSAGE_MAP(CRD5200SampleDlg, CDialog)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDOK, &CRD5200SampleDlg::OnBnClickedOk)
	ON_BN_CLICKED(IDCANCEL, &CRD5200SampleDlg::OnBnClickedCancel)
	ON_BN_CLICKED(IDC_BUTTON3, &CRD5200SampleDlg::OnBnClickedButton3)
	ON_BN_CLICKED(IDC_BUTTON4, &CRD5200SampleDlg::OnBnClickedButton4)

	ON_NOTIFY(TCN_SELCHANGE, IDC_TAB1, &CRD5200SampleDlg::OnTcnSelchangeTab1)
END_MESSAGE_MAP()


// CRD5200SampleDlg 消息处理程序

BOOL CRD5200SampleDlg::OnInitDialog()
{
	CDialog::OnInitDialog();
	AfxInitRichEdit();
	// 将“关于...”菜单项添加到系统菜单中。

	// IDM_ABOUTBOX 必须在系统命令范围内。
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		CString strAboutMenu;
		strAboutMenu.LoadString(IDS_ABOUTBOX);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// 设置此对话框的图标。当应用程序主窗口不是对话框时，框架将自动
	//  执行此操作
	SetIcon(m_hIcon, TRUE);			// 设置大图标
	SetIcon(m_hIcon, FALSE);		// 设置小图标

	// TODO: 在此添加额外的初始化代码

	m_ComType.InsertString(-1,_T("Com"));
	m_ComType.InsertString(-1,_T("USB"));
	m_ComType.InsertString(-1,_T("TCP"));

	m_ComType.SetCurSel(0);

	m_baud.InsertString(-1,_T("9600"));
	m_baud.InsertString(-1,_T("38400"));
	m_baud.InsertString(-1,_T("57600"));
	m_baud.InsertString(-1,_T("115200"));
	m_baud.SetCurSel(1);


	m_frame.InsertString(-1,_T("8E1"));
	m_frame.InsertString(-1,_T("8O1"));
	m_frame.InsertString(-1,_T("8N1"));
	m_frame.SetCurSel(0);

	m_ip.SetAddress(10,168,1,222);

	m_port.SetWindowText(_T("9909"));

	m_usbType.InsertString(-1,_T("Address"));
	m_usbType.InsertString(-1,_T("None Address"));
	m_usbType.SetCurSel(0);



	m_btnOpen.EnableWindow(TRUE);
	m_btnClose.EnableWindow(false);

	RDR_LoadReaderDrivers(_T("\\Drivers"));// Load rfid driver



	int comCnt=COMPort_Enum();

	for (int i=0;i<comCnt;i++)
	{
		CString comName;
		comName.Preallocate(50);
		TCHAR *pStr=comName.GetBuffer()	;
		int size=50;
		memset(pStr,0,50*sizeof(TCHAR));

		COMPort_GetEnumItem(i,pStr,50);
		comName.ReleaseBuffer();

		m_comName.InsertString(-1,comName);

	}
	if (m_comName.GetCount()>0)
	{
		m_comName.SetCurSel(0);
	}

	int hidCnt=HID_Enum(_T("RD5200"));

	for(int i=0;i<hidCnt;i++)
	{
		CString hidSn;
		hidSn.Preallocate(50);
		TCHAR *pStr=hidSn.GetBuffer()	;
		int size=50;
		memset(pStr,0,50*sizeof(TCHAR));

		COMPort_GetEnumItem(i,pStr,50);
		hidSn.ReleaseBuffer();

		m_usbSn.InsertString(-1,hidSn);
	}



	TCITEM   item;   
	item.mask  =   TCIF_TEXT;   
	item.pszText  =_T("Tag Inventory");
	m_tab.InsertItem(0,&item) ;

	item.mask  =   TCIF_TEXT;   
	item.pszText  =_T("Tag Access");
	m_tab.InsertItem(1,&item) ;	


	InventoryDlg *pdlg=new  InventoryDlg();
	pdlg->Create(IDD_INVENTORY,&m_tab);
	pdlg->ShowWindow(SW_HIDE);
	m_tabDlgs.Add(pdlg);


	CommandDlg *pdlg3=new CommandDlg();	
	pdlg3->Create(IDD_COMMAND,&m_tab);
	pdlg3->ShowWindow(SW_HIDE);
	m_tabDlgs.Add(pdlg3);

	
	if(m_tabDlgs.GetCount() > 0) {
		old_page= 0 ;
		RECT rect;
		m_tab.GetClientRect(&rect) ;
		m_tabDlgs[old_page]->SetWindowPos(NULL,10,30,rect.right -20,rect.bottom-40,SWP_SHOWWINDOW);
		m_tab.SetCurSel( 0) ;
	}



	return TRUE;  // 除非将焦点设置到控件，否则返回 TRUE
}

void CRD5200SampleDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialog::OnSysCommand(nID, lParam);
	}
}

// 如果向对话框添加最小化按钮，则需要下面的代码
//  来绘制该图标。对于使用文档/视图模型的 MFC 应用程序，
//  这将由框架自动完成。

void CRD5200SampleDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // 用于绘制的设备上下文

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// 使图标在工作矩形中居中
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// 绘制图标
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}
}

//当用户拖动最小化窗口时系统调用此函数取得光标显示。
//
HCURSOR CRD5200SampleDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}


void CRD5200SampleDlg::OnBnClickedOk()
{
	// TODO: 在此添加控件通知处理程序代码
	//OnOK();
}

void CRD5200SampleDlg::OnBnClickedCancel()
{
	// TODO: 在此添加控件通知处理程序代码
	OnCancel();
}

void CRD5200SampleDlg::OnBnClickedButton3()
{
	// TODO: 在此添加控件通知处理程序代码
	CString connstr ;
	CString sBaud,sFrame,sCom;

	CString ip;
	BYTE ip1,ip2,ip3,ip4;
	m_ip.GetAddress(ip1,ip2,ip3,ip4);

	ip.Format(_T("%d.%d.%d.%d"),ip1,ip2,ip3,ip4);


	switch(m_ComType.GetCurSel())
	{
	case 0: //COM
		{m_comName.GetWindowText(sCom);
		m_baud.GetWindowText(sBaud);
		m_frame.GetWindowText(sFrame);
		connstr.Format(_T("%s=%s;%s=%s;%s=%s;%s=%s;%s=%s;%s=%s"),_T(CONNSTR_NAME_RDTYPE) ,_T("RD5200"),_T(CONNSTR_NAME_COMMTYPE),_T(CONNSTR_NAME_COMMTYPE_COM) ,_T(CONNSTR_NAME_COMNAME),sCom,_T(CONNSTR_NAME_COMBARUD) ,sBaud, _T(CONNSTR_NAME_COMFRAME),sFrame, _T(CONNSTR_NAME_BUSADDR),_T("255")) ;
		}
		break;
	case 1: //USB 
		{CString sn;
		m_usbSn.GetWindowText(sn);
		connstr.Format(_T("%s=%s;%s=%s;%s=%d;%s=%s"),_T(CONNSTR_NAME_RDTYPE) ,_T("RD5200"),_T(CONNSTR_NAME_COMMTYPE) ,_T(CONNSTR_NAME_COMMTYPE_USB) ,_T(CONNSTR_NAME_HIDADDRMODE) ,m_usbType.GetCurSel() ,_T(CONNSTR_NAME_HIDSERNUM),sn) ;
		}
		break;
	case 2: //TCP
		{CString port;
		m_port.GetWindowText(port);
		connstr.Format(_T("%s=%s;%s=%s;%s=%s;%s=%d;%s=%s") ,_T(CONNSTR_NAME_RDTYPE),_T("RD5200"),_T(CONNSTR_NAME_COMMTYPE),_T(CONNSTR_NAME_COMMTYPE_NET),_T(CONNSTR_NAME_REMOTEIP),ip ,_T(CONNSTR_NAME_REMOTEPORT) ,_tstoi(port),_T(CONNSTR_NAME_LOCALIP),_T("")) ;
		}
		break;
	}
	//MessageBox(connstr,"",MB_OK) ;
	int	iret = RDR_Open(connstr,&hr) ;

	if (iret==NO_ERR)
	{
		m_btnClose.EnableWindow(TRUE);
		m_btnOpen.EnableWindow(false);

			
		
		((InventoryDlg *)m_tabDlgs[0])->m_btnStart.EnableWindow(TRUE);
		((InventoryDlg *)m_tabDlgs[0])->m_btnStop.EnableWindow(false);

		int cnt=RDR_GetAntennaInterfaceCount(hr);
		((InventoryDlg *)m_tabDlgs[0])->m_antList.ResetContent();
		for (int i=0;i<cnt;i++)
		{
			CString ant;
			ant.Format(_T("Ant#%d"),i+1);
			((InventoryDlg *)m_tabDlgs[0])->m_antList.InsertString(-1,ant);
			((InventoryDlg *)m_tabDlgs[0])->m_antList.SetItemData(((InventoryDlg *)m_tabDlgs[0])->m_antList.GetCount()-1,i+1);
		}

		if (cnt>0)
		{

			((InventoryDlg *)m_tabDlgs[0])->m_antList.SetCheck(0,true);
		}

	}else
	{
		m_btnClose.EnableWindow(false);
		m_btnOpen.EnableWindow(true);
	}
}





void CRD5200SampleDlg::OnBnClickedButton4()
{
	// TODO: 在此添加控件通知处理程序代码

	InventoryDlg * pdlg=(InventoryDlg *)m_tabDlgs[0];


	if (pdlg->runInventory)
	{
		MessageBox(_T("Please stop inventory!"));
		return;
	}

	RDR_Close(hr);

	hr=NULL;

	((InventoryDlg *)m_tabDlgs[0])->m_btnStart.EnableWindow(FALSE);
	((InventoryDlg *)m_tabDlgs[0])->m_btnStop.EnableWindow(FALSE);


	m_btnClose.EnableWindow(FALSE);
	m_btnOpen.EnableWindow(TRUE);
}



//
//  函数BytesToHexStr说明
//  转字节数组{0x12,0x22,0x0e} 成字符串 "12220E"
//
void BytesToHexStr(BYTE *bBuffer,int bLen,TCHAR *strBuf) 
{ 
	BYTE l4b,r4b;
	TCHAR l4c,r4c;
	int i;
	for(i=0;i<bLen;i++)
	{
		r4b=bBuffer[i] & 0x0f ;
		if(r4b>=0 && r4b<=9)  
			r4c=0x30+r4b ;
		else
			r4c=0x41+(r4b-0x0a) ;

		l4b=(bBuffer[i] & 0xf0) >> 4 ;
		if(l4b>=0 && l4b<=9)  
			l4c=0x30+l4b ;
		else
			l4c=0x41+(l4b-0x0a) ;


		strBuf[i*2]=l4c ;
		strBuf[i*2+1]=r4c;

	}

}






void CRD5200SampleDlg::OnTcnSelchangeTab1(NMHDR *pNMHDR, LRESULT *pResult)
{
	*pResult = 0;
	RECT rect;
	int idx ;
	m_tab.GetClientRect(&rect) ;
	idx  =m_tab.GetCurSel() ;
	m_tabDlgs[old_page]->ShowWindow(SW_HIDE) ; 
	m_tabDlgs[idx]->SetWindowPos(NULL,10,30,rect.right -20,rect.bottom-40,SWP_SHOWWINDOW);
	m_tabDlgs[idx]->BringWindowToTop() ;


	old_page = idx ;
}