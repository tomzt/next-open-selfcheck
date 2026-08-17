using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Collections;

namespace ISO14443ATransparentTransceive
{
    public partial class Form1 : Form
    {

        private UIntPtr hreader = UIntPtr.Zero;
        Thread m_thread = null;
        bool b_threadRun = false;
        List<String> m_blueAddrList = new List<string>();
        public ArrayList readerDriverInfoList;

        UIntPtr hTag = UIntPtr.Zero;

        public Form1()
        {
            InitializeComponent();


            int cnt = RFIDLIB.rfidlib_reader.RDR_LoadReaderDrivers("\\Drivers");


            UInt32 nCOMCnt = RFIDLIB.rfidlib_reader.COMPort_Enum();
            for (UInt32 i = 0; i < nCOMCnt; i++)
            {
                StringBuilder comName = new StringBuilder();
                comName.Append('\0', 64);
                RFIDLIB.rfidlib_reader.COMPort_GetEnumItem(i, comName, (UInt32)comName.Capacity);
                comboBoxCOM.Items.Add(comName);
            }

            if (comboBoxCOM.Items.Count > 0)
            {
                comboBoxCOM.SelectedIndex = 0;
            }
            comboBoxCommType.SelectedIndex = 0;
            comboBoxBaud.SelectedIndex = 1;
            comboBoxFrame.SelectedIndex = 0;

            //枚举已经配对的蓝牙设备
            UInt32 nBluetooth = RFIDLIB.rfidlib_reader.Bluetooth_Enum();
            for (UInt32 j = 0; j < nBluetooth; j++)
            {
                StringBuilder nameBuf = new StringBuilder();
                StringBuilder addrBuf = new StringBuilder();
                UInt32 nSize = 256;
                nameBuf.Append('\0', (int)nSize);
                addrBuf.Append('\0', (int)nSize);
                RFIDLIB.rfidlib_reader.Bluetooth_GetEnumItem(j, 1, nameBuf, ref nSize);
                nSize = 256;
                RFIDLIB.rfidlib_reader.Bluetooth_GetEnumItem(j, 2, addrBuf, ref nSize);
                comboBoxBluetoolName.Items.Add(nameBuf);
                m_blueAddrList.Add(addrBuf.ToString());
            }
            if (comboBoxBluetoolName.Items.Count > 0)
            {
                comboBoxBluetoolName.SelectedIndex = 0;
            }

            comboBoxCommType.Enabled = true;
            buttonOpen.Enabled = true;
            buttonClose.Enabled = false;

            comboBoxCOM.Enabled = true;
            comboBoxBaud.Enabled = true;
            comboBoxFrame.Enabled = true;
            textBoxAddr.Enabled = true;
            textBoxPort.Enabled = true;


            readerDriverInfoList = new ArrayList();

            tbxResponseNum.Text = "4";
            tbxWaittime.Text = "20000";
            btnSend.Enabled = false;
            cbxTX.Checked = true;
            cbxRX.Checked = true;

            btnConnect.Enabled = false;
            btnDisconnect.Enabled = false;
            btnSend.Enabled = false;
            btnInventory.Enabled = false;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UInt32 nCount;
            nCount = RFIDLIB.rfidlib_reader.RDR_GetLoadedReaderDriverCount();
            uint i;
            for (i = 0; i < nCount; i++)
            {
                UInt32 nSize;
                CReaderDriverInf driver = new CReaderDriverInf();
                StringBuilder strCatalog = new StringBuilder();
                strCatalog.Append('\0', 64);

                nSize = (UInt32)strCatalog.Capacity;
                RFIDLIB.rfidlib_reader.RDR_GetLoadedReaderDriverOpt(i, RFIDLIB.rfidlib_def.LOADED_RDRDVR_OPT_CATALOG, strCatalog, ref nSize);
                driver.m_catalog = strCatalog.ToString();
                if (driver.m_catalog == RFIDLIB.rfidlib_def.RDRDVR_TYPE_READER) // Only reader we need
                {
                    StringBuilder strName = new StringBuilder();
                    strName.Append('\0', 64);
                    nSize = (UInt32)strName.Capacity;
                    RFIDLIB.rfidlib_reader.RDR_GetLoadedReaderDriverOpt(i, RFIDLIB.rfidlib_def.LOADED_RDRDVR_OPT_NAME, strName, ref nSize);
                    driver.m_name = strName.ToString();

                    StringBuilder strProductType = new StringBuilder();
                    strProductType.Append('\0', 64);
                    nSize = (UInt32)strProductType.Capacity;
                    RFIDLIB.rfidlib_reader.RDR_GetLoadedReaderDriverOpt(i, RFIDLIB.rfidlib_def.LOADED_RDRDVR_OPT_ID, strProductType, ref nSize);
                    driver.m_productType = strProductType.ToString();

                    StringBuilder strCommSupported = new StringBuilder();
                    strCommSupported.Append('\0', 64);
                    nSize = (UInt32)strCommSupported.Capacity;
                    RFIDLIB.rfidlib_reader.RDR_GetLoadedReaderDriverOpt(i, RFIDLIB.rfidlib_def.LOADED_RDRDVR_OPT_COMMTYPESUPPORTED, strCommSupported, ref nSize);
                    driver.m_commTypeSupported = (UInt32)int.Parse(strCommSupported.ToString());

                    readerDriverInfoList.Add(driver);
                }

            }
            for (i = 0; i < readerDriverInfoList.Count; i++)
            {
                CReaderDriverInf drv = (CReaderDriverInf)(readerDriverInfoList[(int)i]);
                comboBox1.Items.Add(drv.m_name);
            }
            if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            int iret = 0;
            int commTypeIdx = comboBoxCommType.SelectedIndex;
            if (commTypeIdx < 0)
            {
                MessageBox.Show("Please select the type of communication!");
                return;
            }
            string readerDriverName = comboBox1.SelectedItem.ToString();
            string connstr = "";
            if (commTypeIdx == 0)
            {
                connstr = RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                         RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_USB + ";" +
                         RFIDLIB.rfidlib_def.CONNSTR_NAME_HIDADDRMODE + "=" + "0" + ";" +
                         RFIDLIB.rfidlib_def.CONNSTR_NAME_HIDSERNUM + "=";
            }
            else if (commTypeIdx == 1)
            {
                connstr = RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_COM + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMNAME + "=" + comboBoxCOM.Text + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMBARUD + "=" + comboBoxBaud.Text + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMFRAME + "=" + comboBoxFrame.Text + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_BUSADDR + "=" + "255";

            }
            else if (commTypeIdx == 2)
            {
                string ipAddr;
                UInt16 port;
                ipAddr = textBoxAddr.Text;
                port = (UInt16)int.Parse(textBoxPort.Text);
                connstr = RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_NET + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_REMOTEIP + "=" + ipAddr + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_REMOTEPORT + "=" + port.ToString() + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_LOCALIP + "=" + "";
            }
            else if (commTypeIdx == 3)
            {
                if (textBoxBluetoolAddr.Text == "")
                {
                    MessageBox.Show("The address of the bluetooth can not be null!");
                    return;
                }
                connstr = RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                         RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_BLUETOOTH + ";" +
                         RFIDLIB.rfidlib_def.CONNSTR_NAME_BLUETOOTH_SN + "=" + textBoxBluetoolAddr.Text;
            }



            iret = RFIDLIB.rfidlib_reader.RDR_Open(connstr, ref hreader);
            if (iret != 0)
            {
                MessageBox.Show("Open reader failed!");
                return;
            }


            comboBoxCommType.Enabled = false;
            buttonOpen.Enabled = false;
            buttonClose.Enabled = true;

            comboBoxCOM.Enabled = false;
            comboBoxBaud.Enabled = false;
            comboBoxFrame.Enabled = false;
            textBoxAddr.Enabled = false;
            textBoxPort.Enabled = false;
            btnSend.Enabled = false;
            btnInventory.Enabled = true;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            if (hreader == UIntPtr.Zero)
            {
                return;
            }
            RFIDLIB.rfidlib_reader.RDR_Close(hreader);
            hreader = UIntPtr.Zero;

            comboBoxCommType.Enabled = true;
            buttonOpen.Enabled = true;
            buttonClose.Enabled = false;

            comboBoxCOM.Enabled = true;
            comboBoxBaud.Enabled = true;
            comboBoxFrame.Enabled = true;
            textBoxAddr.Enabled = true;
            textBoxPort.Enabled = true;

            btnSend.Enabled = false;

            btnConnect.Enabled = false;
            btnDisconnect.Enabled = false;
            btnSend.Enabled = false;
            btnInventory.Enabled = false;
       
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<tagInfo> inventoryList = new List<tagInfo>();
            comboBoxUid.Items.Clear();
            int iret = 0;
            UIntPtr dnInvenParamList = RFIDLIB.rfidlib_reader.RDR_CreateInvenParamSpecList();
            //RFIDLIB.rfidlib_aip_iso15693.ISO15693_CreateInvenParam(dnInvenParamList, (byte)0, (byte)0, (byte)0, (byte)0);
            RFIDLIB.rfidlib_aip_iso14443A.ISO14443A_CreateInvenParam(dnInvenParamList, (byte)0);
            iret = RFIDLIB.rfidlib_reader.RDR_TagInventory(hreader, RFIDLIB.rfidlib_def.AI_TYPE_NEW, 0, null, dnInvenParamList);
            if (iret != 0)
            {
                return;
            }

            UIntPtr TagDataReport = UIntPtr.Zero;
            TagDataReport = RFIDLIB.rfidlib_reader.RDR_GetTagDataReport(hreader, RFIDLIB.rfidlib_def.RFID_SEEK_FIRST); //first
            while (TagDataReport != UIntPtr.Zero)
            {
                tagInfo tag = new tagInfo();
                UInt32 aip_id = 0;
                UInt32 tag_id = 0;
                UInt32 ant_id = 0;
                Byte dsfid = 0;
                Byte[] uid = new Byte[16];
                byte uidLen = 0;
                string strUid = "";
                
                //ISO14443A标签
                iret = RFIDLIB.rfidlib_aip_iso14443A.ISO14443A_ParseTagDataReport(TagDataReport, ref aip_id, ref tag_id, ref ant_id, uid, ref uidLen);
                if (iret == 0)
                {
                    strUid = BitConverter.ToString(uid, 0, uidLen).Replace("-", string.Empty);
                    tag.uid = strUid;
                    tag.aip_id = aip_id;
                    tag.tag_id = tag_id;
                    inventoryList.Add(tag);
                }

          
                TagDataReport = RFIDLIB.rfidlib_reader.RDR_GetTagDataReport(hreader, RFIDLIB.rfidlib_def.RFID_SEEK_NEXT); //next
            }


            foreach (tagInfo info in inventoryList)
            {
                comboBoxUid.Items.Add(info.uid);
              
            }

            if (inventoryList.Count > 0)
            {
                comboBoxUid.SelectedIndex = 0;
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
            }
            else
            {
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = false;
                btnSend.Enabled = false;
            }



            if (dnInvenParamList != UIntPtr.Zero)
            {
                RFIDLIB.rfidlib_reader.DNODE_Destroy(dnInvenParamList);
            }
        }
        public static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            int len = hex.Length >> 1;
            byte[] arr = new byte[len];

            for (int i = 0; i < len; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            // return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int iret;
            string suid;
            if (hTag != UIntPtr.Zero)
            {
                MessageBox.Show("please disconnect tag first");
                return;
            }
            if (comboBoxUid.Text == "")
            {
                MessageBox.Show("please input a uid");
                return;
            }
            suid = comboBoxUid.Text;
            byte[] uid = StringToByteArrayFastest(suid);
            iret = RFIDLIB.rfidlib_aip_iso14443A.ISO14443A_Connect(hreader, 0, uid,(byte)uid.Length, ref hTag);
            if (iret == 0)
            {
                btnInventory.Enabled = false;
                btnDisconnect.Enabled = true;
                btnConnect.Enabled = false;
                btnSend.Enabled = true;
            }
            else
            {
                MessageBox.Show("fail");
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
         
            RFIDLIB.rfidlib_reader.RDR_TagDisconnect(hreader, hTag);
            btnInventory.Enabled = true;
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
            btnSend.Enabled = false;
            hTag = UIntPtr.Zero;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            
            byte txcrc=(byte)(cbxTX.Checked?1:0);
            byte rxcrc=(byte)(cbxRX.Checked?1:0);
            uint willToReceiveNum=uint.Parse(tbxResponseNum.Text);
            uint waitime=uint.Parse(tbxWaittime.Text);
            
            byte []recvDatas=new byte[255];
            uint nSize=(uint)255;
            String str=rtbxSendData.Text.Trim().Replace("\r","").Replace("\n","");

            byte []reqdata=StringToByteArrayFastest(str);


            int iret= RFIDLIB.rfidlib_aip_iso14443A.ISO14443Ap3_Transceive(hreader, hTag, txcrc, rxcrc, reqdata, (uint)reqdata.Length, willToReceiveNum, recvDatas, ref nSize, waitime);

            if (iret != 0)
            {
                MessageBox.Show("Fail");
            }
            else
            {

                String strRecv = BitConverter.ToString(recvDatas, 0, (int)nSize).Replace("-", string.Empty);
                rtbxReceiveData.Text = strRecv;
                MessageBox.Show("Success");
            }

        }
    }

    public class CReaderDriverInf
    {
        public string m_catalog;
        public string m_name;
        public string m_productType;
        public UInt32 m_commTypeSupported;
    }

    public class tagInfo
    {
        public string uid;
        public UInt32 aip_id = 0;
        public UInt32 tag_id;
    }


}