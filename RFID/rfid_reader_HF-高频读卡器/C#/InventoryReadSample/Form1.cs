using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Collections;

namespace RPANSample
{
    public partial class Form1 : Form
    {
        private UIntPtr hreader = UIntPtr.Zero;
        Thread m_thread = null;
        bool b_threadRun = false;
        public ArrayList readerDriverInfoList;
        public Form1()
        {
            InitializeComponent();
            RFIDLIB.rfidlib_reader.RDR_LoadReaderDrivers("\\Drivers");
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

            for (int i = 0; i < 28; i++)
            {
                string ss;
                ss = i + "";
                comboBoxBlockAddress.Items.Add(i + "");
                ss = string.Format("{0}", i + 1);
                comboBoxBlockCnt.Items.Add(ss);
            }
            comboBoxBlockAddress.SelectedIndex = 0;
            comboBoxBlockCnt.SelectedIndex = 0;


            buttonOpen.Enabled = true;
            buttonClose.Enabled = false;
            comboBoxCOM.Enabled = true;
            buttonStartRecord.Enabled = false;
            buttonStopRecord.Enabled = false;

            readerDriverInfoList = new ArrayList();

            comboBoxCOM.SelectedIndex = comboBoxCOM.Items.Count - 1;
            start_block.SelectedIndex = 0;
            comboBoxBlockNum.SelectedIndex = 0;

            comboBoxMode.SelectedIndex = 1;
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            int iret = 0;

            string readerDriverName = "RD5100";
            string connstr = "";

            Int32 idx = comboBoxCOM.SelectedIndex;
            if (idx == -1)
            {
                MessageBox.Show("Open reader failed!");
            }

            connstr = RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                      RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_COM + ";" +
                      RFIDLIB.rfidlib_def.CONNSTR_NAME_COMNAME + "=" + comboBoxCOM.Text + ";" +
                      RFIDLIB.rfidlib_def.CONNSTR_NAME_COMBARUD + "=" + "38400" + ";" +
                      RFIDLIB.rfidlib_def.CONNSTR_NAME_COMFRAME + "=" + "8E1" + ";" +
                      RFIDLIB.rfidlib_def.CONNSTR_NAME_BUSADDR + "=" + "255";

            


            iret = RFIDLIB.rfidlib_reader.RDR_Open(connstr, ref hreader);
            if (iret != 0)
            {
                MessageBox.Show("Open reader failed!");
                return;
            }
            UInt32 antennaCount = RFIDLIB.rfidlib_reader.RDR_GetAntennaInterfaceCount(hreader);
            for (int i = 0; i < antennaCount; i++)
            {
                int iAnt;
                iAnt = i + 1;
                checkedListBoxAntennaList.Items.Add("Antenna#" + iAnt.ToString());
            }
            buttonOpen.Enabled = false;
            buttonClose.Enabled = true;
            comboBoxCOM.Enabled = false;
            buttonStartRecord.Enabled = true;
            buttonStopRecord.Enabled = false;


        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            if (b_threadRun)
            {
                MessageBox.Show("Please stop the thread first!");
                return;
            }
            if (hreader == UIntPtr.Zero)
            {
                return;
            }
            RFIDLIB.rfidlib_reader.RDR_Close(hreader);
            hreader = UIntPtr.Zero;

            buttonOpen.Enabled = true;
            buttonClose.Enabled = false;
            comboBoxCOM.Enabled = true;
            buttonStartRecord.Enabled = false;
            buttonStopRecord.Enabled = false;
            checkedListBoxAntennaList.Items.Clear();
        }

        private delegate void delegate_addRecord(string uid, string recordData);
        private void addRecord(string uid, string recordData)
        {
            dataGridViewRecord.Rows.Add();
            dataGridViewRecord[0, dataGridViewRecord.RowCount - 1].Value = uid;
            dataGridViewRecord[1, dataGridViewRecord.RowCount - 1].Value = recordData;
        }

        private void buttonStartRecord_Click(object sender, EventArgs e)
        {
            buttonClose.Enabled = false;
            buttonStartRecord.Enabled = false;
            buttonStopRecord.Enabled = true;
            buttonOpen.Enabled = false;
            checkedListBoxAntennaList.Enabled = false;
            start_block.Enabled = false;
            comboBoxBlockNum.Enabled = false;
            dataGridViewRecord.Rows.Clear();
            labelTagCnt.Text = "Tag:0";
            labelTime.Text = "Time:0";
            m_thread = new Thread(InventoryProc);
            m_thread.Start();
        }

        private void InventoryProc()
        {
            int iret = 0;
            UIntPtr dnhReport = UIntPtr.Zero;
            b_threadRun = true;
            byte AIType = RFIDLIB.rfidlib_def.AI_TYPE_NEW;
            UInt32 StartAddr = 0;
            UInt32 NumOfToReadBlocks = 0;
            Byte[] AntennaSel = new Byte[64];
            Byte AntennaSelCount = 0;
            Byte m_mode = 0;

            this.Invoke((EventHandler)(delegate
            {
                for (int i = 0; i < checkedListBoxAntennaList.Items.Count; i++)
                {
                    if (checkedListBoxAntennaList.GetItemChecked(i))
                    {
                        AntennaSel[AntennaSelCount] = (Byte)(i + 1);
                        AntennaSelCount++;
                    }
                }
                StartAddr = (UInt32)start_block.SelectedIndex;
                NumOfToReadBlocks = (UInt32)comboBoxBlockNum.SelectedIndex + 1;

                if (comboBoxMode.SelectedIndex == 0)
                {
                    m_mode = 0x01;
                }
                else
                {
                    m_mode = 0x02;
                }
            }));

            UIntPtr m_hInvenParamSpecList = RFIDLIB.rfidlib_reader.RDR_CreateInvenParamSpecList();
            if (UIntPtr.Zero == m_hInvenParamSpecList)
            {
                return;
            }
            UIntPtr hIso15693InvenParam = RFIDLIB.rfidlib_aip_iso15693.ISO15693_CreateInvenParam(m_hInvenParamSpecList, 0, 0, 0, 0);
            if (UIntPtr.Zero == hIso15693InvenParam)
            {
                return;
            }

            RFIDLIB.rfidlib_aip_iso15693.ISO15693_SetInventoryReadParam(hIso15693InvenParam, m_mode, 0x07);
            RFIDLIB.rfidlib_aip_iso15693.ISO15693_AddInventoryReadBlockArea(hIso15693InvenParam, StartAddr, NumOfToReadBlocks);

            while (b_threadRun)
            {
                System.Diagnostics.Stopwatch tick = new System.Diagnostics.Stopwatch();
                tick.Start();
                iret = RFIDLIB.rfidlib_reader.RDR_TagInventory(hreader, AIType, AntennaSelCount, AntennaSel, m_hInvenParamSpecList);
                tick.Stop();
                if (iret == 0)
                {
                    dnhReport = RFIDLIB.rfidlib_reader.RDR_GetTagDataReport(hreader, RFIDLIB.rfidlib_def.RFID_SEEK_FIRST);
                    while (dnhReport != UIntPtr.Zero)
                    {
                        Byte[] uid = new Byte[64];
                        Byte[] Data = new Byte[40];
                        UInt32 NumOfBlocks = 8;
                        UInt32 nSize = (UInt32)Data.Length;
                        Byte mAntenna = 0;
                        Byte dsfid = 0;

                        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_ParseInvenReadReport(dnhReport, ref mAntenna, ref dsfid, uid, ref NumOfBlocks, Data, ref nSize);
                        if (0 == iret)
                        {
                            String strData = "";
                            String strUID = "";
                            strData = BitConverter.ToString(Data, 0, (int)nSize).Replace("-", string.Empty);
                            strUID = BitConverter.ToString(uid, 0, 8).Replace("-", string.Empty);
                            this.Invoke((EventHandler)(delegate
                            {
                                bool bTagFind = false;
                                for (int j = 0; j < dataGridViewRecord.RowCount; j++)
                                {
                                    if (dataGridViewRecord[0, j].Value.ToString() == strUID)
                                    {
                                        int tagCnt = int.Parse(dataGridViewRecord[2, j].Value.ToString());
                                        tagCnt++;
                                        dataGridViewRecord[2, j].Value = tagCnt.ToString();
                                        bTagFind = true;
                                        break;
                                    }
                                }
                                if (!bTagFind)
                                {
                                    dataGridViewRecord.Rows.Add();
                                    dataGridViewRecord[0, dataGridViewRecord.RowCount - 1].Value = strUID;
                                    dataGridViewRecord[1, dataGridViewRecord.RowCount - 1].Value = strData;
                                    dataGridViewRecord[2, dataGridViewRecord.RowCount - 1].Value = "0";
                                }
                            }));
                        }
                        dnhReport = RFIDLIB.rfidlib_reader.RDR_GetTagDataReport(hreader, RFIDLIB.rfidlib_def.RFID_SEEK_NEXT);
                    }

                    this.Invoke((EventHandler)(delegate
                    {
                        labelTagCnt.Text = "Tag:" + dataGridViewRecord.RowCount;
                        labelTime.Text = "Time:" + tick.ElapsedMilliseconds;
                    }));

                }
            }

            RFIDLIB.rfidlib_reader.DNODE_Destroy(m_hInvenParamSpecList);
            this.Invoke((EventHandler)(delegate
            {
                buttonClose.Enabled = true;
                buttonStartRecord.Enabled = true;
                buttonStopRecord.Enabled = false;
                buttonClose.Enabled = true;
                checkedListBoxAntennaList.Enabled = true;
                start_block.Enabled = true;
                comboBoxBlockNum.Enabled = true;
            }));
            RFIDLIB.rfidlib_reader.RDR_ResetCommuImmeTimeout(hreader);
        }

        private void buttonStopRecord_Click(object sender, EventArgs e)
        {
            b_threadRun = false;
            RFIDLIB.rfidlib_reader.RDR_SetCommuImmeTimeout(hreader);
            buttonStopRecord.Enabled = false;
            start_block.Enabled = true;
            comboBoxBlockNum.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void buttonNormalInven_Click(object sender, EventArgs e)
        {
            UIntPtr m_hInvenParamSpecList = RFIDLIB.rfidlib_reader.RDR_CreateInvenParamSpecList();
            UIntPtr hIso15693InvenParam = RFIDLIB.rfidlib_aip_iso15693.ISO15693_CreateInvenParam(m_hInvenParamSpecList, 0, 0, 0, 0);
            UIntPtr dnhReport = UIntPtr.Zero;
            int iret = 0;
            Byte AntennaSelCount = 0;
            Byte[] AntennaSel = new Byte[32];
            List<String> uidlist = new List<String>();

            checkedListBoxUIDs.Items.Clear();

            for (int i = 0; i < checkedListBoxAntennaList.Items.Count; i++)
            {
                if (checkedListBoxAntennaList.GetItemChecked(i))
                {
                    AntennaSel[AntennaSelCount] = (Byte)(i + 1);
                    AntennaSelCount++;
                }
            }

            iret = RFIDLIB.rfidlib_reader.RDR_TagInventory(hreader, RFIDLIB.rfidlib_def.AI_TYPE_NEW, AntennaSelCount, AntennaSel, m_hInvenParamSpecList);
            if (iret != 0)
            {
                goto exit_fun;
            }

            dnhReport = RFIDLIB.rfidlib_reader.RDR_GetTagDataReport(hreader, RFIDLIB.rfidlib_def.RFID_SEEK_FIRST);
            while (dnhReport != UIntPtr.Zero)
            {
                Byte[] uid = new Byte[8];
                Byte[] Data = new Byte[40];
                Byte dsfid = 0;
                UInt32 aip_id = 0;
                UInt32 tag_id = 0;
                UInt32 ant_id = 0;

                iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_ParseTagDataReport(dnhReport, ref aip_id, ref tag_id, ref ant_id, ref dsfid, uid);
                if (0 == iret)
                {
                    String strUID = "";
                    strUID = BitConverter.ToString(uid, 0, 8).Replace("-", string.Empty);
                    if (!uidlist.Contains(strUID))
                    {
                        uidlist.Add(strUID);
                    }
                }
                dnhReport = RFIDLIB.rfidlib_reader.RDR_GetTagDataReport(hreader, RFIDLIB.rfidlib_def.RFID_SEEK_NEXT);
            }

            foreach (String ss in uidlist)
            {
                checkedListBoxUIDs.Items.Add(ss);
            }
            for (int i = 0; i < checkedListBoxUIDs.Items.Count; i++)
            {
                checkedListBoxUIDs.SetItemChecked(i, true);
            }

        exit_fun:
            if (m_hInvenParamSpecList != null)
            {
                RFIDLIB.rfidlib_reader.DNODE_Destroy(m_hInvenParamSpecList);
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

        private void buttonWrite_Click(object sender, EventArgs e)
        {
            Byte AntennaSelCount = 0;
            Byte[] AntennaSel = new Byte[32];
            UInt32 blkaddress = 0;
            UInt32 blkcnt = 0;
            Byte[] blkdata = null;
            int tagcnt = 0;
            Byte[] result = null;
            UInt32 nSize = 0;
            int iret = 0;
            int m_success = 0;
            int m_fail = 0;

            blkaddress = (UInt32)comboBoxBlockAddress.SelectedIndex;
            blkcnt = (UInt32)(comboBoxBlockCnt.SelectedIndex + 1);
            blkdata = StringToByteArrayFastest(textBoxBlockData.Text);
            
            for (int i = 0; i < checkedListBoxAntennaList.Items.Count; i++)
            {
                if (checkedListBoxAntennaList.GetItemChecked(i))
                {
                    AntennaSel[AntennaSelCount] = (Byte)(i + 1);
                    AntennaSelCount++;
                }
            }

           UIntPtr tagAccess = RFIDLIB.rfidlib_aip_iso15693.ISO15693_CreateTAWriteMultipleTags(0x00, AntennaSelCount, AntennaSel);
           for (int i = 0; i < checkedListBoxUIDs.Items.Count; i++)
           {
               if (checkedListBoxUIDs.GetItemChecked(i))
               {
                   String strUID = checkedListBoxUIDs.Items[i].ToString();
                   Byte[] uid = StringToByteArrayFastest(strUID);

                   RFIDLIB.rfidlib_aip_iso15693.ISO15693_AddOneTagToWriteMultipleTagsAccess(tagAccess, uid);
                   tagcnt++;
               }
           }

           if (tagcnt == 0)
           {
               MessageBox.Show("Please select some tags.");
               goto exit_fun;
            }

            result = new Byte[tagcnt];
            nSize = (UInt32)tagcnt;
           RFIDLIB.rfidlib_aip_iso15693.ISO15693_AddAreaToWriteMultipleTagsAccess(tagAccess, blkaddress, blkcnt, blkdata,(UInt32) blkdata.Length);

           iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_WriteMultipleTags(hreader, tagAccess, result, ref nSize);
           if (iret != 0)
           {
               MessageBox.Show("It's failure in the operation.");
               goto exit_fun;
            }

            for (int j = 0; j < nSize; j++)
            {
                if (result[j] == 0x01)
                {
                    m_success++;
                }
                else
                {
                    m_fail++;
                }
            }

            MessageBox.Show(String.Format("Success:{0}  Failure:{1}",m_success,m_fail));

       exit_fun:
           if (tagAccess != UIntPtr.Zero)
           {
               RFIDLIB.rfidlib_reader.DNODE_Destroy(tagAccess);
           }
        }
    }
}