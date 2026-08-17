using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace M24LR04
{
    public partial class Form1 : Form
    {

        public UIntPtr hreader;
        public UIntPtr hTag;

        List<CReaderDriverInf> readerDriverInfoList = new List<CReaderDriverInf>();
        List<String> m_blueAddrList = new List<string>();

        public Byte readerType;

        Thread inventoryThrd = null;
        Thread searchThrd = null;

        private BindingSource bindingSource= new BindingSource();

        byte[] ant = null;
        bool runInventory;
        bool inventoryContinuous = false;
        bool runSearch;
        bool searchContinuous;

        public Form1()
        {
            InitializeComponent();

            hreader = (UIntPtr)0;
            hTag = (UIntPtr)0;

            comboBox8.Items.Add("None addressed");
            comboBox8.Items.Add("Serial number");
            comboBox8.SelectedIndex = 0;
            comboBox8.Enabled = false;
            comboBox9.Enabled = false;

            comboBox14.SelectedIndex = 1;
            comboBox15.SelectedIndex = 0;

            dataGridView1.Columns.Clear();

            DataGridViewCheckBoxColumn selectColumns = new DataGridViewCheckBoxColumn();
            selectColumns.Name = "Select";
            selectColumns.DataPropertyName = "Select";
            dataGridView1.Columns.Add(selectColumns);

            DataGridViewTextBoxColumn uidColumns = new DataGridViewTextBoxColumn();
            uidColumns.Name = "UID";
            uidColumns.DataPropertyName = "UID";

            dataGridView1.Columns.Add(uidColumns);

            DataGridViewTextBoxColumn cntColumns = new DataGridViewTextBoxColumn();
            cntColumns.Name = "Find Count";
            cntColumns.DataPropertyName = "Count";
            dataGridView1.Columns.Add(cntColumns);

            DataGridViewTextBoxColumn tipsColumns = new DataGridViewTextBoxColumn();
            tipsColumns.Name = "Tips";
            tipsColumns.DataPropertyName = "Tips";

            dataGridView1.Columns.Add(tipsColumns);

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSize = false;
           
            dataGridView1.DataSource = bindingSource;

        
            button1.Enabled = false;
            button4.Enabled = false;
            groupBox1.Enabled = false;
            comboBox33.SelectedIndex = 0;
            comboBox32.SelectedIndex = 0;
            comboBox37.SelectedIndex = 0;
            comboBox34.SelectedIndex = 0;
            comboBox37.SelectedIndex = 0;
            comboBox35.SelectedIndex = 0;
            comboBox36.SelectedIndex = 0;
            comboBox31.SelectedIndex = 0;

            btnSetLedOn.Enabled = false;
            btnSetLedOff.Enabled = false;
            button6.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            /* 
             *  Call required, when application load ,this API just only need to load once
             *  Load all reader driver dll from drivers directory, like "rfidlib_ANRD201.dll"  
             */
            RFIDLIB.rfidlib_reader.RDR_LoadReaderDrivers("\\Drivers");

            /*
             * Not call required,it can be Omitted in your own appliation
             * enum and show loaded reader driver 
             */
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
                comboBox6.Items.Add(drv.m_name);
            }

            if (comboBox6.Items.Count > 0) comboBox6.SelectedIndex = 0;

            /* 
           * Not call required,it can be Omitted in your own appliation
           * enum PC serial ports 
           */
            comboBox1.Items.Clear();
            UInt32 nCOMCnt = RFIDLIB.rfidlib_reader.COMPort_Enum();
            for (i = 0; i < nCOMCnt; i++)
            {
                StringBuilder comName = new StringBuilder();
                comName.Append('\0', 64);
                RFIDLIB.rfidlib_reader.COMPort_GetEnumItem(i, comName, (UInt32)comName.Capacity);
                comboBox1.Items.Add(comName);
            }
            if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
            comboBox1.Enabled = true;
            comboBox10.SelectedIndex = 0;


            /*
             * Not call required,it can be Omitted in your own appliation
             * enum matched bluetooth 
             */
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
                cbbBluetoothName.Items.Add(nameBuf);
                m_blueAddrList.Add(addrBuf.ToString());
            }
            if (cbbBluetoothName.Items.Count > 0)
            {
                cbbBluetoothName.SelectedIndex = 0;
            }



        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox6.SelectedIndex == -1)
            {
                MessageBox.Show("select reader driver type");
                return;
            }
            if (comboBox10.SelectedIndex == -1)
            {
                MessageBox.Show("select communication type");
                return;
            }
            Byte usbOpenType = 0;
            usbOpenType = (Byte)comboBox8.SelectedIndex;

            readerType = (Byte)comboBox6.SelectedIndex;

            /*
            * Try to open communcation layer for specified reader 
            */
            int commTypeIdx = comboBox10.SelectedIndex;
            string readerDriverName = ((CReaderDriverInf)(readerDriverInfoList[readerType])).m_name;
            string connstr = "";
            // Build serial communication connection string
            if (commTypeIdx == 0)
            {
                connstr = RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_COM + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMNAME + "=" + comboBox1.Text + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMBARUD + "=" + comboBox14.Text + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMFRAME + "=" + comboBox15.Text + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_BUSADDR + "=" + "255";
            }
            // Build USBHID communication connection string
            else if (commTypeIdx == 1)
            {
                connstr = RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_USB + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_HIDADDRMODE + "=" + usbOpenType.ToString() + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_HIDSERNUM + "=" + comboBox9.Text;
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
            // Build blueTooth communication connection string
            else if (commTypeIdx == 3)
            {
                if (txbBluetoothSN.Text == "")
                {
                    MessageBox.Show("The address of the bluetooth can not be null!");
                    return;
                }
                connstr = RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                         RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_BLUETOOTH + ";" +
                         RFIDLIB.rfidlib_def.CONNSTR_NAME_BLUETOOTH_SN + "=" + txbBluetoothSN.Text;
            }
            // Call required to open reader driver
            int iret = RFIDLIB.rfidlib_reader.RDR_Open(connstr, ref hreader);

            if (iret != 0)
            {
                /*
                *  Open fail:
                *  if you Encounter this error ,make sure you has called the API "RFIDLIB.rfidlib_reader.RDR_LoadReaderDrivers("\\Drivers")" 
                *  when application load
                */
                MessageBox.Show("fail");
                checkedListBox1.Enabled = true;
                button2.Enabled = true;
            }
            else
            {
                /*
                * Open Ok and try to get some information from driver ,and assign value to the correspondding control 
                */

                // this API is not required in your own application
                // Get antenna count
                uint antennaCount = RFIDLIB.rfidlib_reader.RDR_GetAntennaInterfaceCount(hreader);
                int i;
                checkedListBox1.Items.Clear();
                for (i = 0; i < antennaCount; i++)
                {
                    int iAnt;
                    iAnt = i + 1;

                    checkedListBox1.Items.Add("Antenna#" + iAnt.ToString());
                }

                if (antennaCount > 1)
                {
                    checkedListBox1.Enabled = true;
                }

                if (checkedListBox1.Items.Count > 0)
                {
                    checkedListBox1.SetItemChecked(0, true);
                }


                button2.Enabled = false;
                button3.Enabled = true;
                button1.Enabled = true;
                button4.Enabled = true;
                groupBox1.Enabled = true;
            }


        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            CReaderDriverInf driver = (CReaderDriverInf)readerDriverInfoList[comboBox6.SelectedIndex];

            if ((driver.m_commTypeSupported & RFIDLIB.rfidlib_def.COMMTYPE_USB_EN) > 0)
            {
                comboBox9.Items.Clear();
                UInt32 nCount = RFIDLIB.rfidlib_reader.HID_Enum(driver.m_name);
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
                        comboBox9.Items.Add(sernum.ToString());
                    }
                }
            }
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox8.SelectedIndex == 0)
            {
                comboBox9.Enabled = false;
            }
            else
            {
                comboBox9.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (inventoryThrd != null)
            {
                runInventory = false;

                RFIDLIB.rfidlib_reader.RDR_SetCommuImmeTimeout(hreader); //set all reader's api timeout and quickly quit;
                return;
            }


            if (checkedListBox1.CheckedItems.Count > 0)
            {
                ant = new byte[checkedListBox1.CheckedItems.Count];

                for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                {
                    ant[i] =(byte) (checkedListBox1.CheckedIndices[i]+1);
                }
            }

            inventoryContinuous = checkBox3.Checked;

            if (inventoryContinuous)
            {
                button1.Enabled = true;
                button1.Text = "Stop";
            }
            else
            {
                button1.Enabled = false;
            }

            button4.Enabled = false;
            comboBox2.Items.Clear();
            checkBox3.Enabled = false;
            groupBox1.Enabled = false;
            bindingSource.Clear();
            inventoryThrd = new Thread(ThreadInventory);
            inventoryThrd.Start();
        }


        int loop = 1;
        long spenttime;
        void ThreadInventory()
        {
            runInventory = true;
            loop = 0;
            while (runInventory)
            {
                 UIntPtr hInvenParamSecpList= RFIDLIB.rfidlib_reader.RDR_CreateInvenParamSpecList();
                    RFIDLIB.rfidlib_aip_iso15693.ISO15693_CreateInvenParam(hInvenParamSecpList, 0, 0, 0, 1);


                Byte antCnt =(byte) (ant == null ? 0 : ant.Length);

                System.DateTime last = DateTime.Now;

                int iret=RFIDLIB.rfidlib_reader.RDR_TagInventory(hreader, 1, antCnt, ant, hInvenParamSecpList);

                spenttime = (long)(DateTime.Now - last).TotalMilliseconds;

                loop++;
                if (iret != 0)
                {
                    break;
                }
                else
                {
                    UIntPtr hTagReport = RFIDLIB.rfidlib_reader.RDR_GetTagDataReport(hreader, RFIDLIB.rfidlib_def.RFID_SEEK_FIRST);//get frist tag's report

                    while (hTagReport != UIntPtr.Zero)
                    {
                        UInt32 aip_id = 0;
                        UInt32 tag_id = 0;
                        UInt32 ant_id = 0;
                        Byte dsfid = 0;
                        Byte uidlen = 0;
                        Byte[] uid = new Byte[16];


                        //parse  data of a tag, obtain the tag's uid and antenna id 
                        iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_ParseTagDataReport(hTagReport, ref aip_id, ref tag_id, ref ant_id, ref dsfid, uid);
                        if (iret == 0)
                        {
                            uidlen = 8;
                            object[] pList = { aip_id, tag_id, ant_id, uid, (int)uidlen };

                            Invoke(new EventHandler(delegate
                            {
                                UpdateTagList(aip_id, tag_id, ant_id, uid, (int)uidlen);
                            }));

                            //Invoke(new Action<UInt32, UInt32, UInt32, Byte[], int>(UpdateTagList),pList);//update ui
                               
                           
                        }

                        hTagReport = RFIDLIB.rfidlib_reader.RDR_GetTagDataReport(hreader, RFIDLIB.rfidlib_def.RFID_SEEK_NEXT);//get next tag's report
                    }

                }

                RFIDLIB.rfidlib_reader.DNODE_Destroy(hInvenParamSecpList);//free memory


                if (!inventoryContinuous)
                    break;
            }
            RFIDLIB.rfidlib_reader.RDR_ResetCommuImmeTimeout(hreader);//when we have call RDR_SetCommuImmeTimeout, we must call RDR_ResetCommuImmeTimeout to recover 

            Invoke(new EventHandler(delegate (object sender, EventArgs e)
            {

                button1.Enabled = true;
                button4.Enabled = true;
                checkBox3.Enabled = true;
                groupBox1.Enabled =true;
                button1.Text = "Inventory";

            }));

            inventoryThrd = null;
        }

        private void UpdateTagList(UInt32 aip_id, UInt32 tag_id, UInt32 ant_id, Byte[] uid , int uidlen)
        {
            
          

            String uidStr = System.BitConverter.ToString(uid, 0, uidlen).Replace("-",String.Empty);

            RFIDUIModel existMode = null;
            foreach (RFIDUIModel model in bindingSource)
            {
                if (model.UID == uidStr)
                {
                    existMode = model;
                    break;  
                }
            }

            if (existMode == null)
            {
                existMode = new RFIDUIModel(false, uidStr, 1, "");
                bindingSource.Add(existMode);
                comboBox2.Items.Add(uidStr);

                if (comboBox2.Items.Count > 0)
                    comboBox2.SelectedIndex = 0;

            }
            else
            {
                existMode.Count++;
            }

            existMode.SetFindTheTagInTheAnt((byte)ant_id);
            dataGridView1.Invalidate();

            label2.Text ="Tag:"+bindingSource.Count;
            label3.Text = "Loop:" + loop;
            label4.Text = "Elapse:" + spenttime + "ms";

        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = false;
            comboBox8.Enabled = false;
            comboBox9.Enabled = false;
            textBox5.Enabled = false;
            textBox6.Enabled = false;
            comboBox14.Enabled = false;
            comboBox15.Enabled = false;
            if (comboBox10.SelectedIndex == 0)
            {
                comboBox1.Enabled = true;
                comboBox14.Enabled = true;
                comboBox15.Enabled = true;
            }
            else if (comboBox10.SelectedIndex == 1)
            {
                comboBox8.Enabled = true;
                comboBox9.Enabled = true;
            }
            else if (comboBox10.SelectedIndex == 2)
            {
                textBox5.Enabled = true;
                textBox6.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (hTag != UIntPtr.Zero)
            {
                MessageBox.Show("Please disconnect tag");
                return;
            }

            if (searchThrd != null)
            {
                MessageBox.Show("Please stop search tag");
                return;
            }

            if (inventoryThrd != null)
            {
                MessageBox.Show("Please stop inventory");
                return;
            }


            if (hreader != UIntPtr.Zero)
            {
                RFIDLIB.rfidlib_reader.RDR_Close(hreader);

                button2.Enabled = true;
                button3.Enabled = false;
                button4.Enabled = false;
                button1.Enabled = false;
                groupBox1.Enabled = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            bool enable = checkBox1.Checked;
         
            foreach (RFIDUIModel model in bindingSource)
            {
                model.Select = enable;
            }
            dataGridView1.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (searchThrd != null)
            {
                runSearch = false;
                RFIDLIB.rfidlib_reader.RDR_SetCommuImmeTimeout(hreader);
                return;
            }

            if (checkedListBox1.CheckedItems.Count > 0)
            {
                ant = new byte[checkedListBox1.CheckedItems.Count];

                for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                {
                    ant[i] = (byte)(checkedListBox1.CheckedIndices[i] + 1);
                }
            }

            if (ant == null)
            {
                MessageBox.Show("Please select antenna to find tag");
                return;
            }

            searchContinuous = checkBox2.Checked;

            if (searchContinuous)
            {
                button4.Enabled = true;
                button4.Text = "Stop";
            }
            else
            {
                button4.Enabled = false;
            }
            checkBox1.Enabled = false;
            button1.Enabled = false;
            checkBox2.Enabled = false;
            groupBox1.Enabled = false;

            searchThrd = new Thread(ThrdSearch);
            searchThrd.Start();
        }


        public void ThrdSearch()
        {
            runSearch = true;
            while (runSearch)
            {
                for(int i=0;i<ant.Length;i++)
                {
                    int mSuccessCnt = 0;

                     int irets=    RFIDLIB.rfidlib_reader.RDR_SetAcessAntenna(hreader, ant[i]);

                    foreach (RFIDUIModel rfidTag in bindingSource)
                    {

                        if (rfidTag.Select)
                        {
                            UIntPtr hTag = UIntPtr.Zero;

                            byte[] uid = StringToByteArrayFastest(rfidTag.UID);

                            int iret= RFIDLIB.rfidlib_aip_iso15693.ISO15693_Connect(hreader, RFIDLIB.rfidlib_def.RFID_ISO15693_PICC_ST_M24LR04E_ID, 1, uid,ref hTag);

                            if (iret != 0)
                                continue;

                                
                            iret= ControlLedOnOff(hreader, uid, true);

                            if (iret == 0)
                                mSuccessCnt++;

                            RFIDLIB.rfidlib_reader.RDR_TagDisconnect(hreader, hTag);
                        }
                    }

                    if (mSuccessCnt > 0)
                    {

                        Thread.Sleep(4000);
                    }
                }


                RFIDLIB.rfidlib_reader.RDR_CloseRFTransmitter(hreader);


                for (int i = 0; i < ant.Length; i++)
                {
                    int mSuccessCnt = 0;

                    RFIDLIB.rfidlib_reader.RDR_SetAcessAntenna(hreader, ant[i]);

                    foreach (RFIDUIModel rfidTag in bindingSource)
                    {

                        if (rfidTag.Select)
                        {
                            UIntPtr hTag = UIntPtr.Zero;

                            byte[] uid = StringToByteArrayFastest(rfidTag.UID);

                            int iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_Connect(hreader, RFIDLIB.rfidlib_def.RFID_ISO15693_PICC_ST_M24LR04E_ID, 1, uid, ref hTag);

                            if (iret != 0)
                                continue;


                            iret = ControlLedOnOff(hreader, uid, false);

                            if (iret == 0)
                                mSuccessCnt++;

                            RFIDLIB.rfidlib_reader.RDR_TagDisconnect(hreader, hTag);
                        }
                    }

                    if (mSuccessCnt > 0)
                    {

                        Thread.Sleep(1000);
                    }
                }

                if (!searchContinuous)
                    break;
            }
            RFIDLIB.rfidlib_reader.RDR_ResetCommuImmeTimeout(hreader);

            Invoke(new EventHandler(delegate (object sender, EventArgs e)
            {
                button4.Enabled = true;
                button1.Enabled = true;
                checkBox2.Enabled = true;
                button4.Text = "Search";
                groupBox1.Enabled = true;
                checkBox1.Enabled = true;
            }));

            searchThrd = null;
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

        public int ControlLedOnOff(UIntPtr hreader, byte[] uid,bool on)
        {
           

            Array.Reverse(uid);

            byte[] cmd = new byte[4 + uid.Length];

            cmd[0] = 0x23;
            cmd[1] = 0xa2;
            cmd[2] = 0x02;
            System.Array.Copy(uid, 0, cmd, 3, uid.Length);

            if(on)
            cmd[cmd.Length - 1] = 0x01;
            else
            cmd[cmd.Length - 1] = 0x00;

            byte[] receive = new byte[4];
            uint nsize = 4;

            int iret=RFIDLIB.rfidlib_aip_iso15693.ISO15693_TransparentTransceive(hreader, 0x00, 0x01, 0x01, cmd, (uint)cmd.Length, 4, receive, ref nsize, 0);


            return iret;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex < 0)
            {
                return;
            }
            byte[] uid = StringToByteArrayFastest(comboBox2.SelectedItem.ToString());
            int iret = RFIDLIB.rfidlib_aip_iso15693.ISO15693_Connect(hreader, RFIDLIB.rfidlib_def.RFID_ISO15693_PICC_ST_M24LR04E_ID, 1, uid, ref hTag);

            if (iret != 0)
            {
                MessageBox.Show("Fail");
            }
            else
            {
                button5.Enabled = false;
                button6.Enabled = true;

                button62.Enabled = true;
                button61.Enabled = true;
                button67.Enabled = true;
                button69.Enabled = true;
                btnSetLedOn.Enabled = true;
                button60.Enabled = true;
                btnSetLedOff.Enabled = true;
                button66.Enabled = true;
                button63.Enabled = true;
                button64.Enabled = true;

                button1.Enabled = false;
                button4.Enabled = false;

               

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            RFIDLIB.rfidlib_reader.RDR_TagDisconnect(hreader, hTag);


            button5.Enabled = true;
            button6.Enabled = false;
            button62.Enabled = false;
            button61.Enabled = false;
            button67.Enabled = false;
            button69.Enabled = false;
            btnSetLedOn.Enabled = false;
            button60.Enabled = false;
            btnSetLedOff.Enabled = false;
            button66.Enabled = false;
            button63.Enabled = false;
            button64.Enabled = false;

            button1.Enabled = true;


            button4.Enabled = true;
            hTag = UIntPtr.Zero;
        }

        private void button62_Click(object sender, EventArgs e)
        {
            int iret=-1;
            int pwdIdx;
            UInt32 blockToRead;
            UInt32 blocksRead = 0;
            pwdIdx = comboBox32.SelectedIndex;
            if (pwdIdx < 0)
            {
                MessageBox.Show("please select password number");
                return;
            }
            if (textBox19.Text.Length != 8)
            {
                MessageBox.Show("Invalid password format");
                return;
            }
            byte[] pwd = StringToByteArrayFastest(textBox19.Text);
            UInt32 uPwd = (UInt32)((pwd[0] & 0xff) | (pwd[1] << 8 & 0xff00) | (pwd[2] << 16 & 0xff0000) | (pwd[3] << 24 & 0xff000000));

            RFIDUIModel tag = null;
            foreach (RFIDUIModel rfid_Tag in bindingSource)
            {
                if(rfid_Tag.UID== comboBox2.SelectedItem.ToString())
                {
                    tag = rfid_Tag;
                    break;
                }
            }

            for (int i = 0; i < tag.AntFoundTheTag.Count; i++)
            {
                byte ant = tag.AntFoundTheTag[i];

                RFIDLIB.rfidlib_reader.RDR_SetAcessAntenna(hreader,ant);//

                int   iret1 = RFIDLIB.rfidlib_aip_iso15693.STM24LR_PresentSectorPassword(hreader, hTag, (Byte)(pwdIdx + 1), uPwd);

                if (iret1 == 0)
                {
                    iret = 0;    
                    break;
                }
            }

            if (iret == 0)
            {
                MessageBox.Show("Present pwd ok");
            }
            else
            {
                MessageBox.Show("fail");
            }
        }

        private void button61_Click(object sender, EventArgs e)
        {
            int iret=-1;
            int idx;
            UInt32 blockAddr;
            UInt32 blockToRead;
            UInt32 blocksRead = 0;
            idx = comboBox32.SelectedIndex;
            if (idx < 0)
            {
                MessageBox.Show("please select block address");
                return;
            }
            blockAddr = (UInt32)idx;
            idx = comboBox31.SelectedIndex;
            if (idx < 0)
            {
                MessageBox.Show("please select number of blocks");
                return;
            }
            blockToRead = (UInt32)(idx + 1);
            UInt32 nSize;
            Byte[] BlockBuffer = new Byte[40];


            nSize = (UInt32)BlockBuffer.Length;
            UInt32 bytesRead = 0;



            RFIDUIModel tag = null;
            foreach (RFIDUIModel rfid_Tag in bindingSource)
            {
                if (rfid_Tag.UID == comboBox2.SelectedItem.ToString())
                {
                    tag = rfid_Tag;
                    break;
                }
            }

            for (int i = 0; i < tag.AntFoundTheTag.Count; i++)
            {
                byte ant = tag.AntFoundTheTag[i];

                RFIDLIB.rfidlib_reader.RDR_SetAcessAntenna(hreader, ant);//

                int iret1 =  RFIDLIB.rfidlib_aip_iso15693.ISO15693_ReadMultiBlocks(hreader, hTag, 0, blockAddr, blockToRead, ref blocksRead, BlockBuffer, nSize, ref bytesRead);

                if (iret1 == 0)
                {
                    iret = 0;
                    break;
                }
            }

            if (iret == 0)
            {
                //blocksRead: blocks read 
                textBox18.Text = BitConverter.ToString(BlockBuffer, 0, (int)bytesRead).Replace("-", string.Empty);
                MessageBox.Show("OK");
            }
            else
            {
                MessageBox.Show("fail");
            }
        }

        private void button67_Click(object sender, EventArgs e)
        {
            int iret=-1;
            int idx;
            UInt32 blockAddr;
            UInt32 blockToRead;
            idx = comboBox32.SelectedIndex;
            if (idx < 0)
            {
                MessageBox.Show("please select block address");
                return;
            }
            blockAddr = (UInt32)idx;
            idx = comboBox31.SelectedIndex;
            if (idx < 0)
            {
                MessageBox.Show("please select number of blocks");
                return;
            }
            blockToRead = (UInt32)(idx + 1);
            Byte[] buffer = new Byte[blockToRead];
            UInt32 nSize = (UInt32)buffer.GetLength(0);
            UInt32 bytesRead = 0;


            RFIDUIModel tag = null;
            foreach (RFIDUIModel rfid_Tag in bindingSource)
            {
                if (rfid_Tag.UID == comboBox2.SelectedItem.ToString())
                {
                    tag = rfid_Tag;
                    break;
                }
            }

            for (int i = 0; i < tag.AntFoundTheTag.Count; i++)
            {
                byte ant = tag.AntFoundTheTag[i];

                RFIDLIB.rfidlib_reader.RDR_SetAcessAntenna(hreader, ant);//

                int  iret1 = RFIDLIB.rfidlib_aip_iso15693.ISO15693_GetBlockSecStatus(hreader, hTag, blockAddr, blockToRead, buffer, nSize, ref bytesRead);

                if (iret1 == 0)
                {
                    iret = 0;
                    break;
                }
            }


            
            if (iret == 0)
            {
                textBox18.Text = BitConverter.ToString(buffer).Replace("-", string.Empty);

            }
            else
            {
                MessageBox.Show("fail");
            }
        }

        private void button69_Click(object sender, EventArgs e)
        {
            int iret=-1;
            int idx;
            UInt32 blkAddr;
            UInt32 numOfBlks;
            idx = comboBox32.SelectedIndex;
            if (idx < 0)
            {
                MessageBox.Show("please select block address");
                return;
            }
            blkAddr = (UInt32)idx;
            idx = comboBox31.SelectedIndex;
            if (idx < 0)
            {
                MessageBox.Show("please select number of blocks");
                return;
            }
            numOfBlks = (UInt32)(idx + 1);
            byte[] newBlksData = StringToByteArrayFastest(textBox18.Text);


            RFIDUIModel tag = null;
            foreach (RFIDUIModel rfid_Tag in bindingSource)
            {
                if (rfid_Tag.UID == comboBox2.SelectedItem.ToString())
                {
                    tag = rfid_Tag;
                    break;
                }
            }

            for (int i = 0; i < tag.AntFoundTheTag.Count; i++)
            {
                byte ant = tag.AntFoundTheTag[i];

                RFIDLIB.rfidlib_reader.RDR_SetAcessAntenna(hreader, ant);//

                int iret1 = RFIDLIB.rfidlib_aip_iso15693.ISO15693_WriteMultipleBlocks(hreader, hTag, blkAddr, numOfBlks, newBlksData, (uint)newBlksData.Length);

                if (iret1 == 0)
                {
                    iret = 0;
                    break;
                }
            }


            if (iret == 0)
            {
                MessageBox.Show("Write ok!");
            }
            else
            {
                MessageBox.Show("Write failed!err = " + iret);
            }
        }

        private void btnSetLedOn_Click(object sender, EventArgs e)
        {
            int iret=-1;
         

            RFIDUIModel tag = null;
            foreach (RFIDUIModel rfid_Tag in bindingSource)
            {
                if (rfid_Tag.UID == comboBox2.SelectedItem.ToString())
                {
                    tag = rfid_Tag;
                    break;
                }
            }

            for (int i = 0; i < tag.AntFoundTheTag.Count; i++)
            {
                byte ant = tag.AntFoundTheTag[i];

                RFIDLIB.rfidlib_reader.RDR_SetAcessAntenna(hreader, ant);//

                int iret1 = ControlLedOnOff(hreader,StringToByteArrayFastest(tag.UID),true);

                if (iret1 == 0)
                {
                    iret = 0;
                    break;
                }
            }


            if (iret == 0)
            {
                MessageBox.Show("ok!");
            }
            else
            {
                MessageBox.Show("Fail");
            }
        }

        private void button60_Click(object sender, EventArgs e)
        {
            int iret=-1;
            byte[] afi = StringToByteArrayFastest(textBox17.Text);
         

            RFIDUIModel tag = null;
            foreach (RFIDUIModel rfid_Tag in bindingSource)
            {
                if (rfid_Tag.UID == comboBox2.SelectedItem.ToString())
                {
                    tag = rfid_Tag;
                    break;
                }
            }

            for (int i = 0; i < tag.AntFoundTheTag.Count; i++)
            {
                byte ant = tag.AntFoundTheTag[i];

                RFIDLIB.rfidlib_reader.RDR_SetAcessAntenna(hreader, ant);//

                int iret1 = RFIDLIB.rfidlib_aip_iso15693.ISO15693_WriteAFI(hreader, hTag, afi[0]);

                if (iret1 == 0)
                {
                    iret = 0;
                    break;
                }
            }



            if (iret == 0)
            {
                MessageBox.Show("ok");
            }
            else
            {
                MessageBox.Show("fail");
            }
        }



        private void button66_Click(object sender, EventArgs e)
        {
            int iret=-1;
            byte[] dsfid = StringToByteArrayFastest(textBox16.Text);

            RFIDUIModel tag = null;
            foreach (RFIDUIModel rfid_Tag in bindingSource)
            {
                if (rfid_Tag.UID == comboBox2.SelectedItem.ToString())
                {
                    tag = rfid_Tag;
                    break;
                }
            }

            for (int i = 0; i < tag.AntFoundTheTag.Count; i++)
            {
                byte ant = tag.AntFoundTheTag[i];

                RFIDLIB.rfidlib_reader.RDR_SetAcessAntenna(hreader, ant);//

                int iret1 = RFIDLIB.rfidlib_aip_iso15693.ISO15693_WriteDSFID(hreader, hTag, dsfid[0]);

                if (iret1 == 0)
                {
                    iret = 0;
                    break;
                }
            }


            if (iret == 0)
            {
                MessageBox.Show("ok");
            }
            else
            {
                MessageBox.Show("fail");
            }
        }

        private void button63_Click(object sender, EventArgs e)
        {
            int iret=-1;
            int pwdIdx;
        
            pwdIdx = comboBox37.SelectedIndex;
            if (pwdIdx < 0)
            {
                MessageBox.Show("please select password number");
                return;
            }
            if (textBox20.Text.Length != 8)
            {
                MessageBox.Show("Invalid password format");
                return;
            }
            byte[] pwd = StringToByteArrayFastest(textBox20.Text);
            UInt32 uPwd = (UInt32)((pwd[0] & 0xff) | (pwd[1] << 8 & 0xff00) | (pwd[2] << 16 & 0xff0000) | (pwd[3] << 24 & 0xff000000));


            RFIDUIModel tag = null;
            foreach (RFIDUIModel rfid_Tag in bindingSource)
            {
                if (rfid_Tag.UID == comboBox2.SelectedItem.ToString())
                {
                    tag = rfid_Tag;
                    break;
                }
            }

            for (int i = 0; i < tag.AntFoundTheTag.Count; i++)
            {
                byte ant = tag.AntFoundTheTag[i];

                RFIDLIB.rfidlib_reader.RDR_SetAcessAntenna(hreader, ant);//

                int iret1 = RFIDLIB.rfidlib_aip_iso15693.STM24LR_WriteSectorPassword(hreader, hTag, (Byte)(pwdIdx + 1), uPwd);

                if (iret1 == 0)
                {
                    iret = 0;
                    break;
                }
            }


            if (iret == 0)
            {
                MessageBox.Show("Write pwd ok");
            }
            else
            {
                MessageBox.Show("fail");
            }
        }

        private void button64_Click(object sender, EventArgs e)
        {
            int iret=-1;
            int sectorIdx;
            UInt32 blockAddr;
            UInt32 blockToRead;
            UInt32 blocksRead = 0;
            sectorIdx = comboBox34.SelectedIndex;
            if (sectorIdx < 0)
            {
                MessageBox.Show("please select sector number");
                return;
            }
            int pwdIdx;
            pwdIdx = comboBox35.SelectedIndex;
            if (pwdIdx < 0)
            {
                MessageBox.Show("please select password number");
                return;
            }
            int accessIdx;
            accessIdx = comboBox36.SelectedIndex;
            if (accessIdx < 0)
            {
                MessageBox.Show("please select access");
                return;
            }
            RFIDUIModel tag = null;
            foreach (RFIDUIModel rfid_Tag in bindingSource)
            {
                if (rfid_Tag.UID == comboBox2.SelectedItem.ToString())
                {
                    tag = rfid_Tag;
                    break;
                }
            }

            for (int i = 0; i < tag.AntFoundTheTag.Count; i++)
            {
                byte ant = tag.AntFoundTheTag[i];

                RFIDLIB.rfidlib_reader.RDR_SetAcessAntenna(hreader, ant);//

                int iret1 = RFIDLIB.rfidlib_aip_iso15693.STM24LR_LockSector(hreader, hTag, (Byte)sectorIdx, (Byte)accessIdx, (Byte)(pwdIdx + 1));

                if (iret1 == 0)
                {
                    iret = 0;
                    break;
                }
            }
          
            if (iret == 0)
            {
                MessageBox.Show("present pwd ok");
            }
            else
            {
                MessageBox.Show("fail");
            }
        }

        private void btnSetLedOff_Click(object sender, EventArgs e)
        {
            int iret = -1;


            RFIDUIModel tag = null;
            foreach (RFIDUIModel rfid_Tag in bindingSource)
            {
                if (rfid_Tag.UID == comboBox2.SelectedItem.ToString())
                {
                    tag = rfid_Tag;
                    break;
                }
            }

            for (int i = 0; i < tag.AntFoundTheTag.Count; i++)
            {
                byte ant = tag.AntFoundTheTag[i];

                RFIDLIB.rfidlib_reader.RDR_SetAcessAntenna(hreader, ant);//

                int iret1 = ControlLedOnOff(hreader, StringToByteArrayFastest(tag.UID), false);

                if (iret1 == 0)
                {
                    iret = 0;
                    break;
                }
            }


            if (iret == 0)
            {
                MessageBox.Show("ok!");
            }
            else
            {
                MessageBox.Show("Fail");
            }


        }
    }
}
