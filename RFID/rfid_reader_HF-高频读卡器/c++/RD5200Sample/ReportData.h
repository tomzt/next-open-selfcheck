#pragma once
class ReportData
{

public:
	DWORD aipid;
	DWORD	tag_id;
	DWORD ant_id;
	DWORD readCnt;
	BYTE dsfid;
	WORD rssi;
	BYTE uid[8];
	BYTE data[64];
	DWORD dataLen;

	BYTE EAScmdRes;
	BYTE AFIcmdRes;
	CString uidStr;
	


	ReportData()
	{
		aipid=0;
		tag_id=0;
		ant_id=0;
		readCnt=0;
		dsfid=0;
		rssi=0;
		memset(uid,0,8);
		memset(data,0,64);
		dataLen=64;
		EAScmdRes=0;
		AFIcmdRes=0;
	}

	ReportData(const ReportData& C)  
	{  
		aipid = C.aipid;  
		tag_id=C.tag_id;
		ant_id=C.ant_id;
		readCnt=C.readCnt;
		dsfid=C.dsfid;
		rssi=C.rssi;


		memcpy(uid,C.uid,8);
		memcpy(data,C.data,8);

		dataLen=C.dataLen;

		EAScmdRes=C.EAScmdRes;
		AFIcmdRes=C.AFIcmdRes;

	}  


};
