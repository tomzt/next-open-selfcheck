package com.company;

import com.sun.jna.Library;
import com.sun.jna.Pointer;
import com.sun.jna.ptr.IntByReference;
import com.sun.jna.win32.StdCallLibrary;

public interface rfidlib_reader  extends StdCallLibrary{
	/*********************************functions opened*****************************************************/

	

	
	
	public   int RDR_GetLibVersion(char[] buffer ,int nSize);
	public   int RDR_LoadReaderDrivers(String path) ;
	public   int RDR_GetLoadedReaderDriverCount() ;
	public   int RDR_Open(String connStr ,Pointer hrOut/*out*/);
	public   int RDR_Close(int hr);
	public   int RDR_GetReaderInfor(int hr, byte Type , byte[] buffer, IntByReference nSize);

    public    int  RDR_CreateInvenParamSpecList();

	public   int  RDR_TagInventory(int hr,
									   byte AIType,
									   byte AntennaCount,
									   byte AntennaIDs[],
									   int InvenParamSpecList);

	public	int	 RDR_GetTagDataReport(int hr, byte seek);
	public int  DNODE_Destroy(int dn);
	public   int RDR_SetAcessAntenna(int hr, byte AntennaID);
	public   int RDR_TagDisconnect(int hr, int hTag);
}
