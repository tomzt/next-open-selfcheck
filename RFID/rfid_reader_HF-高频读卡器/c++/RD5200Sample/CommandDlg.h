#pragma once
#include "afxwin.h"
#include "afxcmn.h"


// CommandDlg 对话框

class CommandDlg : public CDialog
{
	DECLARE_DYNAMIC(CommandDlg)

public:
	CommandDlg(CWnd* pParent = NULL);   // 标准构造函数
	virtual ~CommandDlg();

// 对话框数据
	enum { IDD = IDD_COMMAND };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV 支持

	DECLARE_MESSAGE_MAP()
public:
	CCheckListBox uidList;
public:
	afx_msg void OnBnClickedButton1();

	CButton enableReadBlock;

	CEdit startReadBlock;

	CEdit numOfReadBlocks;

	CButton enableWriteBlock;

	CEdit startWriteBlock;

	CEdit numOfWriteBlock;

	CEdit dataOfWriteBlock;

	CButton enableWriteAfi;

	//CEdit afi;

	CButton enableChangeEAS;

	CComboBox EASOption;

	CRichEditCtrl logCommand;

	virtual BOOL OnInitDialog();


	void PrintLog(CString str,COLORREF color=RGB(0,0,0));


public:
	CEdit afiedit;
};
