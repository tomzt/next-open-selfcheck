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

        public Dictionary<String, TagInfo> tagInfos = new Dictionary<string, TagInfo>();

        Thread m_cmdThread = null;

        Boolean noInitBuff = false;


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


            buttonOpen.Enabled = true;
            buttonClose.Enabled = false;
            comboBoxCOM.Enabled = true;
            buttonStartRecord.Enabled = false;
            buttonStopRecord.Enabled = false;

            readerDriverInfoList = new ArrayList();

            comboBoxCOM.SelectedIndex = comboBoxCOM.Items.Count - 1;

            comboBox1.SelectedIndex = 0;


            ckNoInitBuff.Checked = noInitBuff; 
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            if (cbbCommType.SelectedIndex == -1)
            {
                MessageBox.Show("select communication type");
                return;
            }
            Byte usbOpenType = 0;
            usbOpenType = (Byte)cbbUsbType.SelectedIndex;

            int iret = 0;

            checkedListBoxAntennaList.Items.Clear();
            /*
             * Try to open communcation layer for specified reader 
             */
            int commTypeIdx = cbbCommType.SelectedIndex;
            string readerDriverName = "RD5200";
            string connstr = "";
            // Build serial communication connection string
            if (commTypeIdx == 0)
            {
                connstr = RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_COM + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMNAME + "=" + comboBoxCOM.Text + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMBARUD + "=" + cbbBaud.Text + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMFRAME + "=" + cbbFrame.Text + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_BUSADDR + "=" + "255";
            }
            // Build USBHID communication connection string
            else if (commTypeIdx == 1)
            {
                connstr = RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_USB + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_HIDADDRMODE + "=" + usbOpenType.ToString() + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_HIDSERNUM + "=" + cbbUsbSerial.Text;
            }
            // Build network communication connection string
            else if (commTypeIdx == 2)
            {
                string ipAddr;
                UInt16 port;
                ipAddr = textBox5.Text;
                port = (UInt16)int.Parse(textBox6.Text);
                connstr = RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_NET + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_REMOTEIP + "=" + ipAddr + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_REMOTEPORT + "=" + port.ToString() + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_LOCALIP + "=" + "";
            }
            // Call required to open reader driver
            iret = RFIDLIB.rfidlib_reader.RDR_Open(connstr, ref hreader);
            if (iret != 0)
            {
                /*
                 *  Open fail:
                 *  if you Encounter this error ,make sure you has called the API "RFIDLIB.rfidlib_reader.RDR_LoadReaderDrivers("\\Drivers")" 
                 *  when application load
                 */
                MessageBox.Show("fail");
                return;
            }
            else
            {
                UInt32 antcnt = RFIDLIB.rfidlib_reader.RDR_GetAntennaInterfaceCount(hreader);
                for (int i = 0; i < antcnt; i++)
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



        private void buttonStartRecord_Click(object sender, EventArgs e)
        {
            buttonClose.Enabled = false;
            buttonStartRecord.Enabled = false;
            buttonStopRecord.Enabled = true;
            buttonOpen.Enabled = false;
            checkedListBoxAntennaList.Enabled = false;
            TB_start_byte.Enabled = false;
            TB_num_bytes.Enabled = false;
            ckb_EnableRead.Enabled = false;
            listView1.Items.Clear();
            labelTagCnt.Text = "Tag:0";
            labelTime.Text = "Time:0";

            tagInfos.Clear();
            checkedListBox1.Items.Clear();
            m_thread = new Thread(InventoryProc);
            m_thread.Start();
        }

        private void InventoryProc()
        {
            int iret = 0;
            UIntPtr dnhReport = UIntPtr.Zero;
            b_threadRun = true;
            byte AIType = RFIDLIB.rfidlib_def.AI_TYPE_NEW;
            UInt32 StartByte = 0;
            UInt32 NumOfToReadBytes = 0;
            Byte[] AntennaSel = new Byte[64];
            Byte AntennaSelCount = 0;
            bool enableRead = false;
            bool disableEAS = false;
            bool enableEAS = false;
            bool modifyAFI = false;
            bool antidAsUniqueIdentifier = false;

            UIntPtr hDisableEAS;
            UIntPtr hEnableEAS;
            UIntPtr hWriteAFI;


         

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

                StartByte = (uint)int.Parse(TB_start_byte.Text);
                NumOfToReadBytes = (uint)int.Parse(TB_num_bytes.Text);
                enableRead = ckb_EnableRead.Checked;

                disableEAS = ckbWriteEASDisable.Checked;
                enableEAS = ckbWriteEASEnable.Checked;
                modifyAFI = ckbWriteModifyAFI.Checked;

                antidAsUniqueIdentifier = ckbAntAsUnique.Checked;

            }));

            byte[] afival = StringToByteArrayFastest(edtAFI.Text);

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
            /* 读数据块 */
            if (enableRead)
            {
                RFIDLIB.rfidlib_aip_iso15693.ISO15693_SetInventoryReadParam(hIso15693InvenParam, 0x00, 0x00);
                RFIDLIB.rfidlib_aip_iso15693.ISO15693_AddInventoryReadBlockArea(hIso15693InvenParam, StartByte, NumOfToReadBytes);
            }
            /* 写命令  */
            hDisableEAS = UIntPtr.Zero;
            hEnableEAS = UIntPtr.Zero;
            hWriteAFI = UIntPtr.Zero;
            if (disableEAS || modifyAFI||enableEAS)
            {
                if (disableEAS)
                {

                    hDisableEAS = RFIDLIB.rfidlib_aip_iso15693.NXPICODESLI_CreateTADisableEAS(UIntPtr.Zero);
                    if (hDisableEAS != UIntPtr.Zero) RFIDLIB.rfidlib_reader.RDR_AddTagAccessToInvenParam(hIso15693InvenParam, hDisableEAS);
                }
                if (enableEAS)
                {
                    hEnableEAS = RFIDLIB.rfidlib_aip_iso15693.NXPICODESLI_CreateTAEableEAS(UIntPtr.Zero);
                    if (hEnableEAS != UIntPtr.Zero) RFIDLIB.rfidlib_reader.RDR_AddTagAccessToInvenParam(hIso15693InvenParam, hEnableEAS);
                }


                if (modifyAFI)
                {
                    hWriteAFI = RFIDLIB.rfidlib_aip_iso15693.ISO15693_CreateTAWriteAFI(UIntPtr.Zero, afival[0]);
                    if (hWriteAFI != UIntPtr.Zero) RFIDLIB.rfidlib_reader.RDR_AddTagAccessToInvenParam(hIso15693InvenParam, hWriteAFI);
                }
            }
            /* 进入循环盘点  */
            while (b_threadRun)
            {
                if (noInitBuff)
                {
                    AIType = RFIDLIB.rfidlib_def.AI_TYPE_WITH_NOINIT_BUFF;
                }


                System.Diagnostics.Stopwatch tick = new System.Diagnostics.Stopwatch();
                tick.Start();
                iret = RFIDLIB.rfidlib_reader.RDR_TagInventory(hreader, AIType, AntennaSelCount, AntennaSel, m_hInvenParamSpecList);
                tick.Stop();
                if (iret == 0)
                {
                    dnhReport = RFIDLIB.rfidlib_reader.RDR_GetTagDataReport(hreader, RFIDLIB.rfidlib_def.RFID_SEEK_FIRST);
                    while (dnhReport != UIntPtr.Zero)
                    {
                        Byte[] uid = new Byte[8];
                        Byte[] Data = new Byte[64];
                        UInt32 nSize = (UInt32)Data.Length;
                        UInt32 aiptype = 0;
                        UInt32 tagtype = 0;
                        UInt32 antID = 0;
                        Byte dsfid = 0;
                        UInt16 rssi = 0;
                        UInt32 readcnt = 0;
                        Byte EAScmdRes = 0;
                        Byte EASEnableCmdRes=0;
                        Byte AFIcmdRes = 0;

                        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_ParseTagDataReportEx(dnhReport, ref aiptype, ref tagtype, ref antID, ref dsfid, ref rssi, ref readcnt, uid);
                        if (0 == iret)
                        {

                            if (tagtype == 0) tagtype = 1;


                            String strData = "";
                            String strUID = "";
                            strUID = BitConverter.ToString(uid, 0, 8).Replace("-", string.Empty);

                            /* 读到的数据块数据*/
                            if (enableRead)
                            {
                                iret = RFIDLIB.rfidlib_reader.RDR_ParseTagDataReportBlockData(dnhReport, Data, ref nSize);
                                if (iret == 0)
                                {
                                    strData = BitConverter.ToString(Data, 0, (int)nSize).Replace("-", string.Empty);
                                }
                            }
                            /* 解析写命令的结果  */
                            if (disableEAS)
                            {
                                RFIDLIB.rfidlib_reader.RDR_ParseTagDataReportWriteResult(dnhReport, hDisableEAS, ref EAScmdRes);
                            }

                            if (enableEAS)
                            {
                                RFIDLIB.rfidlib_reader.RDR_ParseTagDataReportWriteResult(dnhReport, hEnableEAS, ref EASEnableCmdRes);
                            }


                            if (modifyAFI)
                            {
                                RFIDLIB.rfidlib_reader.RDR_ParseTagDataReportWriteResult(dnhReport, hWriteAFI, ref AFIcmdRes);
                            }



                            /* 把标签加入到列表  */
                            this.Invoke((EventHandler)(delegate
                            {

                                bool found = false;
                                int i;
                                for (i = 0; i < listView1.Items.Count; i++)
                                {
                                    if (antidAsUniqueIdentifier)
                                    {
                                        if (listView1.Items[i].SubItems[1].Text == strUID && listView1.Items[i].SubItems[0].Text == antID.ToString())
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (listView1.Items[i].SubItems[1].Text == strUID)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }

                                }
                                if (!found)
                                {
                                    ListViewItem lvi = new ListViewItem();
                                    lvi.Text = antID.ToString();
                                    lvi.SubItems.Add(strUID);
                                    lvi.SubItems.Add(rssi.ToString());
                                    lvi.SubItems.Add(strData);
                                    lvi.SubItems.Add("1");
                                    lvi.SubItems.Add(EAScmdRes.ToString());
                                    lvi.SubItems.Add(EASEnableCmdRes.ToString());
                                    lvi.SubItems.Add(AFIcmdRes.ToString());
                                    listView1.Items.Add(lvi);

                                    TagInfo info = new TagInfo();


                                    
                             
                                    info.ant = (byte)antID;
                                    info.tag_type = tagtype;

                                    tagInfos.Add(strUID, info);

                                }
                                else
                                {


                                    String strCounter = listView1.Items[i].SubItems[4].Text;
                                    int counter;
                                    counter = int.Parse(strCounter);
                                    counter++;
                                    if (counter >= 100000)
                                    {
                                        counter = 1;
                                    }
                                    listView1.Items[i].SubItems[4].Text = counter.ToString();
                                    listView1.Items[i].SubItems[2].Text = rssi.ToString();
                                    if (listView1.Items[i].SubItems[3].Text == "")
                                    {
                                        listView1.Items[i].SubItems[3].Text = strData;
                                    }
                                    if (listView1.Items[i].SubItems[5].Text == "0")
                                    {
                                        listView1.Items[i].SubItems[5].Text = EAScmdRes.ToString();
                                    }
                                    if (listView1.Items[i].SubItems[6].Text == "0")
                                    {
                                        listView1.Items[i].SubItems[6].Text = EASEnableCmdRes.ToString();
                                       
                                    }
                                    if (listView1.Items[i].SubItems[7].Text == "0")
                                    {
                                        listView1.Items[i].SubItems[7].Text = AFIcmdRes.ToString();
                                    }

                                    if (tagInfos.ContainsKey(strUID))
                                    {
                                       
                                        tagInfos[strUID].ant = (byte)antID;
                                        tagInfos[strUID].tag_type = tagtype;
                                    }
                                }

                                found = false;
                                for (i = 0; i < checkedListBox1.Items.Count; i++)
                                {

                                    if (checkedListBox1.Items[i].ToString() == strUID)
                                    {
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                {
                                    checkedListBox1.Items.Add(strUID);
                                }

                            }));
                        }
                        dnhReport = RFIDLIB.rfidlib_reader.RDR_GetTagDataReport(hreader, RFIDLIB.rfidlib_def.RFID_SEEK_NEXT);
                    }

                    this.Invoke((EventHandler)(delegate
                    {
                        labelTagCnt.Text = "Tag:" + listView1.Items.Count;
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
                TB_start_byte.Enabled = true;
                TB_num_bytes.Enabled = true;
            }));
            RFIDLIB.rfidlib_reader.RDR_ResetCommuImmeTimeout(hreader);
        }

        private void buttonStopRecord_Click(object sender, EventArgs e)
        {
            b_threadRun = false;
            RFIDLIB.rfidlib_reader.RDR_SetCommuImmeTimeout(hreader);
            buttonStopRecord.Enabled = false;
            TB_start_byte.Enabled = true;
            TB_num_bytes.Enabled = true;
            ckb_EnableRead.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            uint i;
            /* 
             * Not call required,it can be Omitted in your own appliation
             * enum PC serial ports 
             */
            comboBoxCOM.Items.Clear();
            UInt32 nCOMCnt = RFIDLIB.rfidlib_reader.COMPort_Enum();
            for (i = 0; i < nCOMCnt; i++)
            {
                StringBuilder comName = new StringBuilder();
                comName.Append('\0', 64);
                RFIDLIB.rfidlib_reader.COMPort_GetEnumItem(i, comName, (UInt32)comName.Capacity);
                comboBoxCOM.Items.Add(comName);
            }
            if (comboBoxCOM.Items.Count > 0) comboBoxCOM.SelectedIndex = 0;
            comboBoxCOM.Enabled = true;
            cbbBaud.SelectedIndex = 1;
            cbbFrame.SelectedIndex = 0;

            cbbUsbType.Items.Add("None addressed");
            cbbUsbType.Items.Add("Serial number");
            cbbUsbType.SelectedIndex = 0;
            cbbCommType.SelectedIndex = 0;
        }

        private void buttonNormalInven_Click(object sender, EventArgs e)
        {

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

        }

        private void comboBoxMode_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void dataGridViewRecord_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (cbbCommType.SelectedIndex == -1)
            {
                MessageBox.Show("select communication type");
                return;
            }
            Byte usbOpenType = 0;
            usbOpenType = (Byte)cbbUsbType.SelectedIndex;

            int iret = 0;

            checkedListBoxAntennaList.Items.Clear();
            /*
             * Try to open communcation layer for specified reader 
             */
            int commTypeIdx = cbbCommType.SelectedIndex;
            string readerDriverName = "RD5200";
            string connstr = "";
            // Build serial communication connection string
            if (commTypeIdx == 0)
            {
                connstr = RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_COM + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMNAME + "=" + comboBoxCOM.Text + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMBARUD + "=" + cbbBaud.Text + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMFRAME + "=" + cbbFrame.Text + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_BUSADDR + "=" + "255";
            }
            // Build USBHID communication connection string
            else if (commTypeIdx == 1)
            {
                connstr = RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_USB + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_HIDADDRMODE + "=" + usbOpenType.ToString() + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_HIDSERNUM + "=" + cbbUsbSerial.Text;
            }
            // Build network communication connection string
            else if (commTypeIdx == 2)
            {
                string ipAddr;
                UInt16 port;
                ipAddr = textBox5.Text;
                port = (UInt16)int.Parse(textBox6.Text);
                connstr = RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_NET + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_REMOTEIP + "=" + ipAddr + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_REMOTEPORT + "=" + port.ToString() + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_LOCALIP + "=" + "";
            }
            // Call required to open reader driver
            iret = RFIDLIB.rfidlib_reader.RDR_Open(connstr, ref hreader);
            if (iret != 0)
            {
                /*
                 *  Open fail:
                 *  if you Encounter this error ,make sure you has called the API "RFIDLIB.rfidlib_reader.RDR_LoadReaderDrivers("\\Drivers")" 
                 *  when application load
                 */
                MessageBox.Show("fail");
                return;
            }
            else
            {
                UInt32 antcnt = RFIDLIB.rfidlib_reader.RDR_GetAntennaInterfaceCount(hreader);
                for (int i = 0; i < antcnt; i++)
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
        }

        private void button3_Click(object sender, EventArgs e)
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

        private void cbbUsbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbbUsbSerial.Items.Clear();
            UInt32 nCount = RFIDLIB.rfidlib_reader.HID_Enum("RD5200");
            int iret;
            int i;
            for (i = 0; i < nCount; i++)
            {
                StringBuilder sernum = new StringBuilder();
                sernum.Append('\0', 64);
                UInt32 nSize;
                nSize = (UInt32)sernum.Capacity;
                iret = RFIDLIB.rfidlib_reader.HID_GetEnumItem((UInt32)i, RFIDLIB.rfidlib_def.HID_ENUM_INF_TYPE_SERIALNUM, sernum, ref nSize);
                if (iret == 0)
                {
                    cbbUsbSerial.Items.Add(sernum.ToString());
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       


        public class TagInfo
        {
            public byte ant;
            public UInt32 tag_type;
        }

        UIntPtr pSet = UIntPtr.Zero;

        Boolean HasCmdReadData = false;
        Boolean HasCmdWriteData = false;
        Boolean HasCmdWriteAFI = false;
        Boolean HasCmdEas = false;

        Boolean CmdEnableEas = false;



        private void button2_Click_1(object sender, EventArgs e)
        {

            if (checkedListBox1.CheckedItems.Count != 0)
            {

                HasCmdReadData = checkBox1.Checked;
                HasCmdWriteData = checkBox2.Checked;
                HasCmdWriteAFI = checkBox3.Checked;
                HasCmdEas = checkBox4.Checked;


                string s = "";

                pSet = RFIDLIB.rfidlib_reader.CreateMultipleAccessTagSet(0, 0, 0, 100);

                Boolean inheritTagType = false;
                Boolean inheritAnt = false;
                Boolean inheritCmd = false;

                for (int x = 0; x < checkedListBox1.CheckedItems.Count; x++)
                {

                    String struid = checkedListBox1.CheckedItems[x].ToString();

                    if (x == 0)
                    {
                        inheritTagType = false;
                        inheritAnt = false;
                        inheritCmd = false;

                    }
                    else
                    {
                        inheritCmd = true;

                        if (tagInfos[struid].tag_type
                            == tagInfos[checkedListBox1.CheckedItems[x - 1].ToString()].tag_type)
                        {
                            inheritTagType = true;
                        }
                        else
                        {
                            inheritTagType = false;
                        }


                        if (tagInfos[struid].ant
                            == tagInfos[checkedListBox1.CheckedItems[x - 1].ToString()].ant)
                        {
                            inheritAnt = true;
                        }
                        else
                        {
                            inheritAnt = false;
                        }
                    }

                    byte[] uid = StringToByteArrayFastest(struid);
                    int iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_AddNewAccessTag(pSet, 1, uid, inheritTagType, inheritAnt, inheritCmd);

                    if (iret != 0)
                    {
                        richTextBox1.AppendText("ISO15693_AddNewAccessTag fail code:" + iret + "\r\n");
                        goto exit_fail;
                    }


                    if (!inheritTagType)
                    {
                        iret = RFIDLIB.rfidlib_reader.RDR_SetLastATagTagType(pSet, tagInfos[struid].tag_type);

                        if (iret != 0)
                        {
                            richTextBox1.AppendText("RDR_SetLastATagTagType fail code:" + iret + "\r\n");
                            goto exit_fail;
                        }

                    }
                    if (!inheritAnt)
                    {
                        byte[] ant = new byte[1];
                        ant[0] = tagInfos[struid].ant;
                        iret = RFIDLIB.rfidlib_reader.RDR_SetLastATagAntennas(pSet, ant, 1);

                        if (iret != 0)
                        {
                            richTextBox1.AppendText("RDR_SetLastATagAntennas fail code:" + iret + "\r\n");
                            goto exit_fail;
                        }

                    }

                    if (!inheritCmd)
                    {

                        if (HasCmdReadData)//读块
                        {
                            UIntPtr ptrCmd = RFIDLIB.rfidlib_aip_iso15693.ISO15693_CreateTAReadMultipleBlocks(UIntPtr.Zero, false, uint.Parse(textBox1.Text), uint.Parse(textBox2.Text));

                            if (ptrCmd != null)
                            {
                                iret = RFIDLIB.rfidlib_reader.RDR_AddLastATagAccessCommand(pSet, ptrCmd);

                                if (iret != 0)
                                {
                                    richTextBox1.AppendText("RDR_AddLastATagAccessCommand fail code:" + iret + "\r\n");
                                    goto exit_fail;
                                }

                                //hCmdRead = ptrCmd;
                            }
                            else
                            {
                                richTextBox1.AppendText("Create Command fail code:" + iret + "\r\n");
                                goto exit_fail;
                            }
                        }

                        if (HasCmdWriteData)
                        {
                            byte[] data = StringToByteArrayFastest(textBox7.Text.ToString());


                            UIntPtr ptrCmd = RFIDLIB.rfidlib_aip_iso15693.ISO15693_CreateTAWriteMultipleBlocks(UIntPtr.Zero, uint.Parse(textBox4.Text), uint.Parse(textBox3.Text), data, (uint)data.Length);

                            if (ptrCmd != null)
                            {
                                iret = RFIDLIB.rfidlib_reader.RDR_AddLastATagAccessCommand(pSet, ptrCmd);

                                if (iret != 0)
                                {
                                    richTextBox1.AppendText("RDR_AddLastATagAccessCommand fail code:" + iret + "\r\n");
                                    goto exit_fail;
                                }

                                //  hCmdWrite = ptrCmd;
                            }
                            else
                            {
                                richTextBox1.AppendText("Create Command fail code:" + iret + "\r\n");
                                goto exit_fail;
                            }
                        }

                        if (HasCmdWriteAFI)
                        {
                            byte val = byte.Parse(textBox10.Text, System.Globalization.NumberStyles.HexNumber);
                            UIntPtr ptrCmd = RFIDLIB.rfidlib_aip_iso15693.ISO15693_CreateTAWriteAFI(UIntPtr.Zero, val);

                            if (ptrCmd != null)
                            {
                                iret = RFIDLIB.rfidlib_reader.RDR_AddLastATagAccessCommand(pSet, ptrCmd);

                                if (iret != 0)
                                {
                                    richTextBox1.AppendText("RDR_AddLastATagAccessCommand fail code:" + iret + "\r\n");
                                    goto exit_fail;
                                }

                                // hCmdAFI = ptrCmd;
                            }
                            else
                            {
                                richTextBox1.AppendText("Create Command fail code:" + iret + "\r\n");
                                goto exit_fail;
                            }

                        }

                        if (HasCmdEas)
                        {
                            UIntPtr ptrCmd = UIntPtr.Zero;
                            if (comboBox1.SelectedIndex == 0)
                            {
                                ptrCmd = RFIDLIB.rfidlib_aip_iso15693.NXPICODESLI_CreateTADisableEAS(UIntPtr.Zero);
                                CmdEnableEas = false;
                            }
                            else
                            {
                                ptrCmd = RFIDLIB.rfidlib_aip_iso15693.NXPICODESLI_CreateTAEableEAS(UIntPtr.Zero);

                                CmdEnableEas = true;
                            }

                            if (ptrCmd != null)
                            {
                                iret = RFIDLIB.rfidlib_reader.RDR_AddLastATagAccessCommand(pSet, ptrCmd);
                                if (iret != 0)
                                {
                                    richTextBox1.AppendText("RDR_AddLastATagAccessCommand fail code:" + iret + "\r\n");
                                    goto exit_fail;
                                }

                                //   hCmdEas = ptrCmd;
                            }
                            else
                            {
                                richTextBox1.AppendText("Create Command fail code:" + iret + "\r\n");
                                goto exit_fail;
                            }
                        }

                    }
                }
            }

         
           m_cmdThread = new Thread(new ThreadStart(TagCommandProc));

           m_cmdThread.Start();

            return;

        exit_fail:
            if (pSet != UIntPtr.Zero)
            {
                RFIDLIB.rfidlib_reader.DNODE_Destroy(pSet);
                pSet = UIntPtr.Zero;
            }
        }


        private void TagCommandProc()
        {


            int iret = RFIDLIB.rfidlib_reader.RDR_AccessMultipleTags(hreader, pSet);//exec

            if (iret != 0)
            {
                this.Invoke((EventHandler)(delegate
                {
                    richTextBox1.AppendText("RDR_AccessMultipleTags fail code:" + iret + "\r\n");
                })
                );

            }
            else
            {

                int _iret = RFIDLIB.rfidlib_reader.RDR_SeekAccessTag(pSet, 0);
                if (iret != 0)
                {
                    goto exit_fail;
                }

                int tagCnt = 0;

                this.Invoke((EventHandler)(delegate
                {
                    tagCnt = checkedListBox1.CheckedItems.Count;
                }));


                for (int i = 0; i < tagCnt; i++)
                {
                    string result = String.Empty;
                    byte flag = RFIDLIB.rfidlib_def.RFID_SEEK_FIRST;


                    string uid=String.Empty;

                    this.Invoke((EventHandler)(delegate
                    {
                            uid = checkedListBox1.CheckedItems[i].ToString();
                    }));


                    if (HasCmdReadData)
                    {
                        UIntPtr ptrCmd = UIntPtr.Zero;

                        ptrCmd = RFIDLIB.rfidlib_reader.RDR_GetTagAccessCommand(pSet, flag);

                        flag = RFIDLIB.rfidlib_def.RFID_SEEK_NEXT;

                        if (ptrCmd == UIntPtr.Zero)
                            goto exit_fail;



                        UInt32 numofblock = 0;
                        byte[] bufBlocks = new byte[255];
                        UInt32 nSize = 255;

                        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_ParseReadMultiBlocksResult(ptrCmd, ref numofblock, bufBlocks, ref nSize);

                        if (iret == 0)
                        {
                            if (numofblock != 0)
                            {
                                String strBlockData = "";
                                strBlockData = BitConverter.ToString(bufBlocks, 0, (int)nSize).Replace("-", string.Empty);


                                result = "[" + uid + "]" + "块数量:" + numofblock + "  数据:" + strBlockData;
                            }
                        }
                        else
                        {
                            result = "[" + uid + "]" + "ISO15693_ParseReadMultiBlocksResult  error:" + iret;
                        }


                        

                        this.Invoke((EventHandler)(delegate
                        {
                                 richTextBox1.AppendText(result + "\r\n");
                            }));

                    }

                    if (HasCmdWriteData)
                    {
                        UIntPtr ptrCmd = UIntPtr.Zero;
                        ptrCmd = RFIDLIB.rfidlib_reader.RDR_GetTagAccessCommand(pSet, flag);
                        if (ptrCmd == UIntPtr.Zero)
                            goto exit_fail;


                        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_ParseWriteMultipleBlocksResult(ptrCmd);

                        if (iret == 0)
                        {
                            result = "[" + uid + "]" + "写数据成功";
                        }
                        else
                        {
                            result = "[" + uid + "]" + "写数据失败 error:"+iret;
                        }
                        this.Invoke((EventHandler)(delegate
                        {
                            richTextBox1.AppendText(result + "\r\n");
                        }));
                    }

                    if (HasCmdWriteAFI)
                    {
                        UIntPtr ptrCmd = UIntPtr.Zero;
                        ptrCmd = RFIDLIB.rfidlib_reader.RDR_GetTagAccessCommand(pSet, flag);
                        if (ptrCmd == UIntPtr.Zero)
                            goto exit_fail;

                        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_ParseWriteAFIResult(ptrCmd);

                        if (iret == 0)
                        {
                            result = "[" + uid + "]" + "写AFI成功";
                        }
                        else
                        {
                            result = "[" + uid + "]" + "写AFI失败 error:" + iret;
                        }
                        this.Invoke((EventHandler)(delegate
                        {
                            richTextBox1.AppendText(result + "\r\n");
                        }));
                    }

                    if (HasCmdEas)
                    {
                        UIntPtr ptrCmd = UIntPtr.Zero;
                        ptrCmd = RFIDLIB.rfidlib_reader.RDR_GetTagAccessCommand(pSet, flag);
                        if (ptrCmd == UIntPtr.Zero)
                            goto exit_fail;


                        if (!CmdEnableEas)
                        {
                            iret = RFIDLIB.rfidlib_aip_iso15693.NXPICODESLI_ParseDisableEASResult(ptrCmd);

                            if (iret == 0)
                            {
                                result = "[" + uid + "]" + "Disable EAS成功";
                            }
                            else
                            {
                                result = "[" + uid + "]" + "Disable EAS失败 error:" + iret;
                            }

                        }
                        else
                        {
                            iret = RFIDLIB.rfidlib_aip_iso15693.NXPICODESLI_ParseEableEASResult(ptrCmd);

                            if (iret == 0)
                            {
                                result = "[" + uid + "]" + "Enable EAS成功";
                            }
                            else
                            {
                                result = "[" + uid + "]" + "Enable EAS失败 error:" + iret;
                            }
                        }
                        this.Invoke((EventHandler)(delegate
                        {
                            richTextBox1.AppendText(result + "\r\n");
                        }));

                    }

                    _iret = RFIDLIB.rfidlib_reader.RDR_SeekAccessTag(pSet, (ushort)(i + 1));

                }
            }


        exit_fail:

            RFIDLIB.rfidlib_reader.DNODE_Destroy(pSet);
            pSet = UIntPtr.Zero;

            return;


        }

        private void ckbWriteEASDisable_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbWriteEASDisable.Checked)
            {
                ckbWriteEASEnable.Checked = false;
            }
        }

        private void ckbWriteEASEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbWriteEASEnable.Checked)
            {
                ckbWriteEASDisable.Checked = false;
            }
        }

        private void ckNoInitBuff_CheckedChanged(object sender, EventArgs e)
        {
            noInitBuff = ckNoInitBuff.Checked; 



        }





    }
}