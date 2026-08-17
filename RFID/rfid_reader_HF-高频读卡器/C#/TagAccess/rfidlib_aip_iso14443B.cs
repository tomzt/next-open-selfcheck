using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RFIDLIB
{
     class rfidlib_aip_iso14443B
    {
#if UNICODE
       /**********************************************Use Unicode Character Set********************************************/
      [DllImport("rfidlib_aip_iso14443B.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
       public static extern UInt32 ISO14443B_GetLibVersion(StringBuilder buf, UInt32 nSize);
      [DllImport("rfidlib_aip_iso14443B.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
       public static extern int ISO14443B_ParseTagDataReport(UIntPtr hTagReport,
                                          ref UInt32 aip_id,
                                         ref UInt32 tag_id,
                                          ref UInt32 ant_id,
                                          ref UInt32 metaFlags,
                                          Byte[] tagData,
                                          ref UInt32 tagDataLen);
      [DllImport("rfidlib_aip_iso14443B.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
       public static extern UIntPtr ISO14443B_CreateInvenParam(UIntPtr hInvenParamSpecList,
                                                            Byte AntennaID,
                                                            Byte AFI,
                                                            Byte SlotNum
                                                            );
#else
        /**************************************************Use Multi-Byte Character Set**********************************************/
        [DllImport("rfidlib_aip_iso14443B.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 ISO14443B_GetLibVersion(StringBuilder buf, UInt32 nSize);
        [DllImport("rfidlib_aip_iso14443B.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int ISO14443B_ParseTagDataReport(UIntPtr hTagReport,
                                          ref UInt32 aip_id,
                                         ref UInt32 tag_id,
                                          ref UInt32 ant_id,
                                          ref UInt32 metaFlags,
                                          Byte[] tagData,
                                          ref UInt32 tagDataLen);
        [DllImport("rfidlib_aip_iso14443B.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern UIntPtr ISO14443B_CreateInvenParam(UIntPtr hInvenParamSpecList,
                                                            Byte AntennaID,
                                                            Byte AFI,
                                                            Byte SlotNum
                                                            );
#endif
    }
}
