#pragma once


extern DWORD WINAPI inventoryThrd( LPVOID lpThreadParameter  );




// InventoryDlg 对话框

class InventoryDlg : public CDialog
{
	DECLARE_DYNAMIC(InventoryDlg)

public:
	InventoryDlg(CWnd* pParent = NULL);   // 标准构造函数
	virtual ~InventoryDlg();

	// 对话框数据
	enum { IDD = IDD_INVENTORY };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedButton1();
public:
	afx_msg void OnBnClickedButton2();


	CEdit m_StartByte;

	CEdit m_numBytes;

	CButton m_btnStart;

	CButton m_btnStop;




	CCheckListBox m_antList;

	CStatic m_tagCnt;

	CStatic m_timeCnt;

	CListCtrl m_listUid;

	CButton m_enableRead;
	long m_ProcTime;

	CButton m_enableEAS;

	CButton m_enableAfi;

	CEdit m_afi;

	afx_msg	 LRESULT InventoryDlg::OnInventoryMsg(WPARAM wParam, LPARAM lParam);
	virtual BOOL OnInitDialog();

	bool runInventory;

};
