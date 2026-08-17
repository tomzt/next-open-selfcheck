using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace RFIDLIB
{
    class rfidlib_aip_iso14443BST
    {
#if UNICODE
        /**********************************************Use Unicode Character Set********************************************/
        [DllImport("rfidlib_aip_st_iso14443B.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 STISO14443B_CreateInvenParam(UIntPtr hInvenParamSpecList,
                                                            Byte AntennaID);

        [DllImport("rfidlib_aip_st_iso14443B.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int STISO14443B_ParseTagDataReport(UIntPtr hTagReport,
                                          ref UInt32 aip_id,
                                         ref UInt32 tag_id,
                                          ref UInt32 ant_id,
                                          ref byte chip_id,
                                          Byte[] uid,
                                          ref UInt32 uidlen);

        [DllImport("rfidlib_aip_st_iso14443B.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int STISO14443B_Connect(UIntPtr hr, UInt32 tag_type, Byte chip_id, ref UIntPtr ht);

        [DllImport("rfidlib_aip_st_iso14443B.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int STISO14443B_ReadBlocks(UIntPtr hr,
                                                        UIntPtr ht,
                                                        UInt32 blkAddr,
                                                        UInt32 numOfBlksToRead,
                                                        ref UInt32 numofBlksRead,
                                                        Byte[] bufBlocks,
                                                        UInt32 nSize,
                                                        ref UInt32 bytesBlkDatRead);
        [DllImport("rfidlib_aip_st_iso14443B.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int STISO14443B_WriteBlocks(UIntPtr hr, UIntPtr ht,
                                                        UInt32 blkAddr,
                                                        UInt32 numOfBlks,
                                                        Byte[] newBlksData,
                                                        UInt32 bytesToWrite);
#else
        /**************************************************Use Multi-Byte Character Set**********************************************/
        [DllImport("rfidlib_aip_st_iso14443B.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 STISO14443B_CreateInvenParam(UIntPtr hInvenParamSpecList,
                                                            Byte AntennaID);
        [DllImport("rfidlib_aip_st_iso14443B.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int STISO14443B_ParseTagDataReport(UIntPtr hTagReport,
                                          ref UInt32 aip_id,
                                         ref UInt32 tag_id,
                                          ref UInt32 ant_id,
                                          ref byte chip_id,
                                          Byte[] uid,
                                          ref UInt32 uidlen);
        [DllImport("rfidlib_aip_st_iso14443B.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int STISO14443B_Connect(UIntPtr hr, UInt32 tag_type, Byte chip_id, ref UIntPtr ht);

        [DllImport("rfidlib_aip_st_iso14443B.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int STISO14443B_ReadBlocks(UIntPtr hr, 
                                                        UIntPtr ht,
                                                        UInt32 blkAddr,
                                                        UInt32 numOfBlksToRead,
                                                        ref  UInt32 numofBlksRead,
                                                        Byte[] bufBlocks,
                                                        UInt32 nSize,
                                                        ref UInt32 bytesBlkDatRead);
        [DllImport("rfidlib_aip_st_iso14443B.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int STISO14443B_WriteBlocks(UIntPtr hr, UIntPtr ht,
                                                        UInt32 blkAddr, 
                                                        UInt32 numOfBlks,                                                  
                                                        Byte[] newBlksData,
                                                        UInt32 bytesToWrite);



#endif
    }
}
