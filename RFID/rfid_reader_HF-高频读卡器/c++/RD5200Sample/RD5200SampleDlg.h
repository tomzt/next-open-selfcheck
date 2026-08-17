// RD5200SampleDlg.h : 头文件
//

#pragma once
#include "afxwin.h"
#include "afxcmn.h"
#include "rfidlib.h"
#include "rfidlib_reader.h"






// CRD5200SampleDlg 对话框
class CRD5200SampleDlg : public CDialog
{
	// 构造
public:
	CRD5200SampleDlg(CWnd* pParent = NULL);	// 标准构造函数

	// 对话框数据
	enum { IDD = IDD_RD5200SAMPLE_DIALOG };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV 支持


	// 实现
protected:
	HICON m_hIcon;

	// 生成的消息映射函数
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedOk();

	afx_msg void OnBnClickedCancel();

	CComboBox m_ComType;

	CComboBox m_comName;

	CComboBox m_baud;

	CComboBox m_frame;

	afx_msg void OnBnClickedButton3();

	afx_msg void OnBnClickedButton4();

	CComboBox m_usbType;

	CComboBox m_usbSn;

	CIPAddressCtrl m_ip;

	CEdit m_port;

	CEdit m_StartByte;

	CEdit m_numBytes;

	CButton m_btnOpen;

	CButton m_btnClose;

	




public:

	

	CTabCtrl m_tab;
	CArray<CDialog *> m_tabDlgs;
	int old_page;


	afx_msg void OnTcnSelchangeTab1(NMHDR *pNMHDR, LRESULT *pResult);
};
