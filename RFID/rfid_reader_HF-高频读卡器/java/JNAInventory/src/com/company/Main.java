package com.company;


import com.sun.jna.Memory;
import com.sun.jna.Native;
import com.sun.jna.Pointer;
import com.sun.jna.ptr.ByteByReference;
import com.sun.jna.ptr.IntByReference;

import java.lang.management.ManagementFactory;
import java.util.ArrayList;
import java.util.List;

public class Main {

    public static void main(String[] args) {
	// write your code here


        String name = ManagementFactory.getRuntimeMXBean().getName();
        System.out.println(name);
    // get pid
        String pid = name.split("@")[0];
        System.out.println("Pid is:"+pid);

        rfidlib_reader reader_readerInstance   = (rfidlib_reader) Native.load("E:\\C++\\Project\\rfid.sdk.win\\rfid_reader-读卡器开发包\\HF-高频\\samples\\java\\JNAInventory\\lib\\rfidlib_reader.dll",rfidlib_reader.class);

        rfidlib_AIP_ISO15693 rfidlib_aip_iso15693Instance=(rfidlib_AIP_ISO15693) Native.load("E:\\C++\\Project\\rfid.sdk.win\\rfid_reader-读卡器开发包\\HF-高频\\samples\\java\\JNAInventory\\lib\\rfidlib_aip_iso15693.dll",rfidlib_AIP_ISO15693.class);


        reader_readerInstance.RDR_LoadReaderDrivers("E:\\C++\\Project\\rfid.sdk.win\\rfid_reader-读卡器开发包\\HF-高频\\samples\\java\\JNAInventory\\lib\\Drivers");
        int cnt1= reader_readerInstance. RDR_GetLoadedReaderDriverCount() ;


        Pointer pM_Hr=new Memory(4);

        pM_Hr.setInt(0,0);

        int iret=  reader_readerInstance.RDR_Open("RDType=RL8000;CommType=USB;AddrMode=0;SerNum=",pM_Hr);

        if(iret==0)
        {
          System.out.println("pM_Hr is:"+pM_Hr.getInt(0));

            byte []data=new byte[255];

            IntByReference len=new IntByReference();
            len.setValue(255);
            int hr=pM_Hr.getInt(0);

          iret = reader_readerInstance.RDR_GetReaderInfor(hr,  (byte)1, data, len);

          int getlen=len.getValue();

        int     dnInvenParamList =reader_readerInstance.RDR_CreateInvenParamSpecList() ;
          if(dnInvenParamList!=0)
          {

              rfidlib_aip_iso15693Instance.ISO15693_CreateInvenParam(dnInvenParamList,(byte)0,(byte)0,(byte)0,(byte)0x00) ;

          }

          byte aiype=1;
          byte[] ant=new byte[1];
          iret = reader_readerInstance.RDR_TagInventory(hr,aiype,(byte)1,ant ,dnInvenParamList) ;

          List<Tag> tagList=new ArrayList<Tag>();

          if(iret==0)
          {
            int hReport=reader_readerInstance.RDR_GetTagDataReport(hr,(byte)1);

              while(hReport!=0)
              {

                  IntByReference aip_id=new IntByReference(0);
                  IntByReference tag_id=new IntByReference(0);
                  IntByReference ant_id=new IntByReference(0);
                  ByteByReference dsfid=new ByteByReference((byte)0);
                  byte [] uid=new byte[8];

                 iret= rfidlib_aip_iso15693Instance.ISO15693_ParseTagDataReport(hReport,aip_id,tag_id,ant_id,dsfid,uid);

                 if(iret==0)
                 {
                     Tag tag=new Tag();
                     tag.setUid(uid);
                     tag.setAnt((byte)ant_id.getValue());
                     tag.setTagid((byte)tag_id.getValue());
                     tagList.add(tag);
                 }


                  hReport=reader_readerInstance.RDR_GetTagDataReport(hr,(byte)2);
              }
          }

          reader_readerInstance.DNODE_Destroy(dnInvenParamList);


          for (int i=0;i<tagList.size();i++)
          {
              reader_readerInstance.RDR_SetAcessAntenna(hr,tagList.get(i).getAnt());

              IntByReference hTag=new IntByReference();

            iret=rfidlib_aip_iso15693Instance.ISO15693_Connect(hr
                      ,tagList.get(i).getTagid()
                      , (byte) 1
                      ,tagList.get(i).getUid()
                      ,hTag
                      );
            if(iret==0)
            {
                IntByReference blockReaded=new IntByReference(0);
                IntByReference bytesReadedNum=new IntByReference(0);

                int readBlock=1;
                byte []blockdata=new byte[readBlock*4];

                iret= rfidlib_aip_iso15693Instance.ISO15693_ReadMultiBlocks(hr,hTag.getValue()
                        , (byte) 0
                        ,0
                        ,1
                        ,blockReaded
                        ,blockdata
                        ,readBlock*4
                        ,bytesReadedNum
                        );

                if(iret==0)
                {
                    tagList.get(i).setReadBlockData(blockdata);
                }

                reader_readerInstance.RDR_TagDisconnect(hr,hTag.getValue());
            }

          }

         reader_readerInstance.RDR_Close(hr);
        }
    }

}
