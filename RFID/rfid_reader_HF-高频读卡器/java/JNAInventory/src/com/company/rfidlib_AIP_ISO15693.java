package com.company;

import com.sun.jna.ptr.ByteByReference;
import com.sun.jna.ptr.IntByReference;
import com.sun.jna.win32.StdCallLibrary;

public interface rfidlib_AIP_ISO15693 extends StdCallLibrary
{


	// inventory need to match the AFI value
	public  int ISO15693_CreateInvenParam(int hInvenParamSpecList, byte AntennaID, byte en_afi, byte afi, byte slot_type);


	public   int ISO15693_ParseTagDataReport(int hTagReport,
											 IntByReference aip_id, IntByReference tag_id, IntByReference ant_id, ByteByReference dsfid,
											 byte uid[]);


	public   int ISO15693_ParseTagDataReportEx(int hTagReport,
											   IntByReference aip_id, IntByReference tag_id, IntByReference ant_id, ByteByReference dsfid,
											   IntByReference rssi, IntByReference readCnt, byte uid[]);

	public   int ISO15693_Connect(int hr, int tagType,
			byte address_mode, byte[] uid, IntByReference ht);

	public   int ISO15693_Reset(int hr, int ht);

	public   int ISO15693_ReadSingleBlock(int hr, int ht,
			byte readSecSta, int blkAddr, byte bufBlockDat[], int nSize,
			IntByReference bytesBlkDatRead);

	public   int ISO15693_WriteSingleBlock(int hr, int ht,
			int blkAddr, byte[] newBlkData, int bytesToWrite);

	public   int ISO15693_LockBlock(int hr, int ht, int blkAddr);

	public   int ISO15693_ReadMultiBlocks(int hr, int ht,
			byte readSecSta, int blkAddr, int numOfBlksToRead,
			IntByReference numOfBlksRead, byte[] bufBlocks, int nSize,
										  IntByReference bytesBlkDatRead);

	public   int ISO15693_WriteMultipleBlocks(int hr, int ht,
			int blkAddr, int numOfBlks, byte[] newBlksData, int bytesToWrite);

	public   int ISO15693_WriteAFI(int hr, int ht, byte afi);

	public   int ISO15693_LockAFI(int hr, int ht);

	public   int ISO15693_WriteDSFID(int hr, int ht, byte dsfid);

	public   int ISO15693_LockDSFID(int hr, int ht);

	public   int ISO15693_GetSystemInfo(int hr, int ht,
			byte[] uid, ByteByReference dsfid, ByteByReference afi, IntByReference blkSize,
			Integer numOfBloks, ByteByReference icRef);

	public   int ISO15693_GetBlockSecStatus(int hr, int ht,
			int blkAddr, int numOfBlks, byte[] bufBlkSecs, int nSize/*
																	 * in: size
																	 * of the
																	 * buffer
																	 */,
			Integer bytesSecRead /* out:number of block status byte copied */);

	public   int ISO15693_LockMultipleBlocks(int hr, int ht,
			int blkAddr, int numOfBlks);



	public  int NXPICODESLI_EableEAS(int hr, int ht);

	public  int NXPICODESLI_DisableEAS(int hr, int ht);

	public  int NXPICODESLI_LockEAS(int hr, int ht);

	public  int NXPICODESLI_EASCheck(int hr, int ht, ByteByReference EASFlag);

	public  int NXPICODESLIX_EableEAS(int hr, int ht);

	public  int NXPICODESLIX_DisableEAS(int hr, int ht);

	public  int NXPICODESLIX_LockEAS(int hr, int ht);

	public  int NXPICODESLIX_EASAlarm(int hr, int ht,
			byte[] EAS_data, int nSize, Integer bytesWritten);

	public  int NXPICODESLIX_GetRandomNum(int hr, int ht,
			Integer random/* 16bits */);

	public  int NXPICODESLIX_SetPassword(int hr, int ht,
			byte pwdNo,/*
						 * password adress,Only one password is supported
						 * now,the address is 10H
						 */
			int random, int pwd/* 32bits */);

	public   int NXPICODESLIX_WritePassword(int hr, int ht,
			byte pwdNo, int pwd/* 32bits */);

	public   int NXPICODESLIX_LockPassword(int hr, int ht,
			byte pwdNo);

	public   int NXPICODESLIX_PasswordProtect(int hr, int ht,
			byte bandType/* EAS=0 or AFI=1 */);

	public   int NXPICODESLIX_EASCheck(int hr, int ht,
			ByteByReference EASFlag);





}
