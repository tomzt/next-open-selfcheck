using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ISO14443Ap4_Transceive
{
    public partial class Form1 : Form
    {
        UIntPtr hTag = UIntPtr.Zero;
        public ArrayList readerDriverInfoList;
        private UIntPtr hreader = UIntPtr.Zero;
        public Form1()
        {
            InitializeComponent();
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            comboBox2.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            readerDriverInfoList = new ArrayList();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            #region 初始化com口
            int cnt = rfidlib_reader.RDR_LoadReaderDrivers("\\Drivers");

            UInt32 nCOMCnt = rfidlib_reader.COMPort_Enum();
            for (UInt32 i = 0; i < nCOMCnt; i++)
            {
                StringBuilder comName = new StringBuilder();
                comName.Append('\0', 64);
                rfidlib_reader.COMPort_GetEnumItem(i, comName, (UInt32)comName.Capacity);
                comboBox3.Items.Add(comName);
            }
            if (comboBox3.Items.Count > 0)
            {
                comboBox3.SelectedIndex = 0;
            }
            #endregion

            #region 初始化设备驱动
            UInt32 nCount;
            nCount = rfidlib_reader.RDR_GetLoadedReaderDriverCount();
            for (uint i = 0; i < nCount; i++)
            {
                UInt32 nSize;
                CReaderDriverInf driver = new CReaderDriverInf();
                StringBuilder strCatalog = new StringBuilder();
                strCatalog.Append('\0', 64);

                nSize = (UInt32)strCatalog.Capacity;
                rfidlib_reader.RDR_GetLoadedReaderDriverOpt(i, rfidlib_def.LOADED_RDRDVR_OPT_CATALOG, strCatalog, ref nSize);
                driver.m_catalog = strCatalog.ToString();
                if (driver.m_catalog == rfidlib_def.RDRDVR_TYPE_READER) // Only reader we need
                {
                    StringBuilder strName = new StringBuilder();
                    strName.Append('\0', 64);
                    nSize = (UInt32)strName.Capacity;
                    rfidlib_reader.RDR_GetLoadedReaderDriverOpt(i, rfidlib_def.LOADED_RDRDVR_OPT_NAME, strName, ref nSize);
                    driver.m_name = strName.ToString();

                    StringBuilder strProductType = new StringBuilder();
                    strProductType.Append('\0', 64);
                    nSize = (UInt32)strProductType.Capacity;
                    rfidlib_reader.RDR_GetLoadedReaderDriverOpt(i, rfidlib_def.LOADED_RDRDVR_OPT_ID, strProductType, ref nSize);
                    driver.m_productType = strProductType.ToString();

                    StringBuilder strCommSupported = new StringBuilder();
                    strCommSupported.Append('\0', 64);
                    nSize = (UInt32)strCommSupported.Capacity;
                    rfidlib_reader.RDR_GetLoadedReaderDriverOpt(i, rfidlib_def.LOADED_RDRDVR_OPT_COMMTYPESUPPORTED, strCommSupported, ref nSize);
                    driver.m_commTypeSupported = (UInt32)int.Parse(strCommSupported.ToString());

                    readerDriverInfoList.Add(driver);
                }

            }
            for (int i = 0; i < readerDriverInfoList.Count; i++)
            {
                CReaderDriverInf drv = (CReaderDriverInf)(readerDriverInfoList[(int)i]);
                comboBox1.Items.Add(drv.m_name);
            }
            if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
            #endregion

        }

        /// <summary>
        /// 打开设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            int iret = 0;
            int commTypeIdx = comboBox2.SelectedIndex;
            if (commTypeIdx < 0)
            {
                MessageBox.Show("Please select the type of communication!");
                return;
            }
            string readerDriverName = comboBox1.SelectedItem.ToString();
            string connstr = "";
            if (commTypeIdx == 0)
            {
                connstr = rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                         rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + rfidlib_def.CONNSTR_NAME_COMMTYPE_USB + ";" +
                         rfidlib_def.CONNSTR_NAME_HIDADDRMODE + "=" + "0" + ";" +
                         rfidlib_def.CONNSTR_NAME_HIDSERNUM + "=";
            }
            else if (commTypeIdx == 1)
            {
                connstr = rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                          rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + rfidlib_def.CONNSTR_NAME_COMMTYPE_COM + ";" +
                          rfidlib_def.CONNSTR_NAME_COMNAME + "=" + comboBox3.Text + ";" +
                          rfidlib_def.CONNSTR_NAME_COMBARUD + "=" + comboBox4.Text + ";" +
                          rfidlib_def.CONNSTR_NAME_COMFRAME + "=" + comboBox5.Text + ";" +
                          rfidlib_def.CONNSTR_NAME_BUSADDR + "=" + "255";

            }
            else if (commTypeIdx == 2)
            {
                string ipAddr;
                UInt16 port;
                ipAddr = textBox3.Text;
                port = (UInt16)int.Parse(textBox4.Text);
                connstr = rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                          rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + rfidlib_def.CONNSTR_NAME_COMMTYPE_NET + ";" +
                          rfidlib_def.CONNSTR_NAME_REMOTEIP + "=" + ipAddr + ";" +
                          rfidlib_def.CONNSTR_NAME_REMOTEPORT + "=" + port.ToString() + ";" +
                          rfidlib_def.CONNSTR_NAME_LOCALIP + "=" + "";
            }

            iret = rfidlib_reader.RDR_Open(connstr, ref hreader);
            if (iret != 0)
            {
                MessageBox.Show("Open reader failed!");
                return;
            }

            MessageBox.Show("Open reader success!");

            button1.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            comboBox3.Enabled = false;
            comboBox4.Enabled = false;
            comboBox5.Enabled = false;
            textBox3.Enabled = false;
            textBox3.Enabled = false;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0) 
            {
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
                comboBox5.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
            }
            else if (comboBox2.SelectedIndex == 1) 
            {
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
            }
            else if (comboBox3.SelectedIndex == 2) 
            {
                textBox3.Enabled = true;
                textBox4.Enabled = true;
                comboBox3.Enabled = false;
                comboBox4.Enabled = false;
                comboBox5.Enabled = false;
            }
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (hreader == UIntPtr.Zero)
            {
                return;
            }
            rfidlib_reader.RDR_Close(hreader);
            hreader = UIntPtr.Zero;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;
            comboBox3.Enabled = true;
            comboBox4.Enabled = true;
            comboBox5.Enabled = true;

        }

        /// <summary>
        /// 盘点标签数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            List<tagInfo> inventoryList = new List<tagInfo>();
            comboBox8.Items.Clear();
            int iret = 0;
            UIntPtr dnInvenParamList = rfidlib_reader.RDR_CreateInvenParamSpecList();
            //rfidlib_aip_iso15693.ISO15693_CreateInvenParam(dnInvenParamList, (byte)0, (byte)0, (byte)0, (byte)0);
            rfidlib_aip_iso14443A.ISO14443A_CreateInvenParam(dnInvenParamList, (byte)0);
            iret = rfidlib_reader.RDR_TagInventory(hreader, rfidlib_def.AI_TYPE_NEW, 0, null, dnInvenParamList);
            if (iret != 0)
            {
                return;
            }

            UIntPtr TagDataReport = UIntPtr.Zero;
            TagDataReport = rfidlib_reader.RDR_GetTagDataReport(hreader, rfidlib_def.RFID_SEEK_FIRST); //first
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
                iret = rfidlib_aip_iso14443A.ISO14443A_ParseTagDataReport(TagDataReport, ref aip_id, ref tag_id, ref ant_id, uid, ref uidLen);
                if (iret == 0)
                {
                    strUid = BitConverter.ToString(uid, 0, uidLen).Replace("-", string.Empty);
                    tag.uid = strUid;
                    tag.aip_id = aip_id;
                    tag.tag_id = tag_id;
                    inventoryList.Add(tag);
                }


                TagDataReport = rfidlib_reader.RDR_GetTagDataReport(hreader, rfidlib_def.RFID_SEEK_NEXT); //next
            }


            foreach (tagInfo info in inventoryList)
            {
                comboBox8.Items.Add(info.uid);

            }

            if (inventoryList.Count > 0)
            {
                comboBox8.SelectedIndex = 0;
                button4.Enabled = true;
                button5.Enabled = false;
            }
            else
            {
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
            }



            if (dnInvenParamList != UIntPtr.Zero)
            {
                rfidlib_reader.DNODE_Destroy(dnInvenParamList);
            }
        }

        /// <summary>
        /// 连接标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            int iret;
            string suid;
            if (hTag != UIntPtr.Zero)
            {
                MessageBox.Show("please disconnect tag first");
                return;
            }
            if (comboBox8.Text == "")
            {
                MessageBox.Show("please input a uid");
                return;
            }
            suid = comboBox8.Text;
            byte[] uid = StringToByteArrayFastest(suid);
            iret = rfidlib_aip_iso14443A.ISO14443A_Connect(hreader, 0, uid, (byte)uid.Length, ref hTag);
            if (iret == 0)
            {
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = true;
                button6.Enabled = true;
            }
            else
            {
                MessageBox.Show("fail");
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
                //
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }
        /// <summary>
        /// 给定字符获取对应十六进制值
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        /// <summary>
        /// 断开标签连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            rfidlib_reader.RDR_TagDisconnect(hreader, hTag);
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = false;
            button6.Enabled = false;
            hTag = UIntPtr.Zero;
        }

        private void button6_Click(object sender, EventArgs e)
        {

            byte[] recvDatas = new byte[255];
            uint nSize = (uint)255;
            String str = richTextBox1.Text.Trim().Replace("\r", "").Replace("\n", "");

            byte[] reqdata = StringToByteArrayFastest(str);

            int iret = rfidlib_aip_iso14443A.ISO14443Ap4_Transceive(hreader,hTag,reqdata,(uint)reqdata.Length,recvDatas ,ref nSize);
            //int iret = rfidlib_aip_iso14443A.ISO14443Ap3_Transceive(hreader, hTag, txcrc, rxcrc, reqdata, (uint)reqdata.Length, willToReceiveNum, recvDatas, ref nSize, waitime);
            if (iret == -17)
            {
                int errorCode = rfidlib_reader.RDR_GetReaderLastReturnError(hreader);
                textBox5.Text = errorCode.ToString();
            }
            if (iret != 0)
            {
                MessageBox.Show("error:"+iret);
            }
            
            else
            {
                String strRecv = BitConverter.ToString(recvDatas, 0, (int)nSize).Replace("-", string.Empty);
                richTextBox2.Text = strRecv;
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
