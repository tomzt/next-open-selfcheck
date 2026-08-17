// RD5200Sample.h : PROJECT_NAME 应用程序的主头文件
//

#pragma once

#ifndef __AFXWIN_H__
	#error "在包含此文件之前包含“stdafx.h”以生成 PCH 文件"
#endif

#include "resource.h"		// 主符号


// CRD5200SampleApp:
// 有关此类的实现，请参阅 RD5200Sample.cpp
//

class CRD5200SampleApp : public CWinApp
{
public:
	CRD5200SampleApp();

// 重写
	public:
	virtual BOOL InitInstance();

// 实现

	DECLARE_MESSAGE_MAP()
};

extern CRD5200SampleApp theApp;