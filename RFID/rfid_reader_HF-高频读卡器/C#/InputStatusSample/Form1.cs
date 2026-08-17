using System.Windows.Forms;
using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
namespace InputStatusSample
{
    public partial class Form1 : Form
    {
        private const int WM_DEVICECHANGE = 0x219; //设备改变
        IntPtr DBT_DEVICEARRIVAL = (IntPtr)0x8000; //检测到新设备
        IntPtr DBT_DEVICEREMOVECOMPLETE = (IntPtr)0x8004;//设备移除
        IntPtr DBT_DEVNODES_CHANGED = (IntPtr)0x0007;//向系统添加或删除设备时
        public ArrayList readerDriverInfoList;
        public UIntPtr hreader = (UIntPtr)0;
        CReaderDriverInf driver=null;
        public Form1()
        {
            InitializeComponent();
            readerDriverInfoList = new ArrayList();
            comboBox2.SelectedIndex = 0;
            comboBox4.SelectedIndex = 1;
            comboBox5.SelectedIndex = 0;
            comboBox6.Items.Add("None addressed");
            comboBox6.Items.Add("Serial number");
            comboBox6.SelectedIndex = 0;
            button2.Enabled = false;
            button3.Enabled = false;

        }
        

        private void Form1_Load(object sender, EventArgs e)
        {
            #region 初始化com口
            int cnt = rfidlib_reader.RDR_LoadReaderDrivers("\\Drivers");
            enumCOMPort();
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Enabled = false;
            comboBox4.Enabled = false;
            comboBox5.Enabled = false;
            comboBox6.Enabled = false;
            comboBox7.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            if (comboBox2.SelectedIndex == 0)
            {
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
                enumCOMPort();
            }
            else if (comboBox2.SelectedIndex == 1)
            {
                comboBox6.Enabled = true;
                comboBox7.Enabled = true;
                enumSerialNumber();
            }
            else
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;
            }
        }

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Byte usbOpenType = 0;
            usbOpenType = (Byte)comboBox6.SelectedIndex;


            Byte readerType = (Byte)comboBox1.SelectedIndex;

            int iret = 0;


            /*
             * Try to open communcation layer for specified reader 
             */
            int connTypeIdx = comboBox2.SelectedIndex;
            string readerDriverName = ((CReaderDriverInf)(readerDriverInfoList[readerType])).m_name;
            string connstr = "";
            // Build serial communication connection string
            if (connTypeIdx == 0)
            {
                connstr = rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                          rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + rfidlib_def.CONNSTR_NAME_COMMTYPE_COM + ";" +
                          rfidlib_def.CONNSTR_NAME_COMNAME + "=" + comboBox3.Text + ";" +
                          rfidlib_def.CONNSTR_NAME_COMBARUD + "=" + comboBox4.Text + ";" +
                          rfidlib_def.CONNSTR_NAME_COMFRAME + "=" + comboBox5.Text + ";" +
                          rfidlib_def.CONNSTR_NAME_BUSADDR + "=" + "255";
            }
            // Build USBHID communication connection string
            else if (connTypeIdx == 1)
            {
                connstr = rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                          rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + rfidlib_def.CONNSTR_NAME_COMMTYPE_USB + ";" +
                          rfidlib_def.CONNSTR_NAME_HIDADDRMODE + "=" + usbOpenType.ToString() + ";" +
                          rfidlib_def.CONNSTR_NAME_HIDSERNUM + "=" + comboBox7.Text;
            }
            // Build network communication connection string
            else if (connTypeIdx == 2)
            {
                string ipAddr;
                UInt16 port;
                ipAddr = textBox1.Text;
                port = (UInt16)int.Parse(textBox2.Text);
                connstr = rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                          rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + rfidlib_def.CONNSTR_NAME_COMMTYPE_NET + ";" +
                          rfidlib_def.CONNSTR_NAME_REMOTEIP + "=" + ipAddr + ";" +
                          rfidlib_def.CONNSTR_NAME_REMOTEPORT + "=" + port.ToString() + ";" +
                          rfidlib_def.CONNSTR_NAME_LOCALIP + "=" + "";
            }

            // Call required to open reader driver
            iret = rfidlib_reader.RDR_Open(connstr, ref hreader);
            if (iret != 0)
            {
                /*
                 *  Open fail:
                 *  if you Encounter this error ,make sure you has called the API "rfidlib_reader.RDR_LoadReaderDrivers("\\Drivers")" 
                 *  when application load
                 */
                MessageBox.Show("fail");
                button1.Enabled = true;
                return;
            }
            button1.Enabled = false;
            button3.Enabled = true;
            button2.Enabled = true;
        }

        public class CReaderDriverInf
        {
            public string m_catalog;
            public string m_name;
            public string m_productType;
            public UInt32 m_commTypeSupported;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int iret = 0;
            /*
             *  Close reader driver ,this API is required
             */
            iret = rfidlib_reader.RDR_Close(hreader);
            if (iret == 0)
            {
                comboBox2.SelectedIndex = 0;
                button1.Enabled = true;
                button3.Enabled = false;
                button2.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            byte[] OutputIDs = new byte[255];
            UInt32 OutputSize = (UInt32)OutputIDs.Length;
            byte[] InputIDs = new byte[255];
            UInt32 InputSize = (UInt32)InputIDs.Length;
            byte[] status = new byte[255];
            List<byte> lstOutPutId = new List<byte>();
            List<byte> lstInputID = new List<byte>();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            int iret = 0;

            //只需要获取输入端口状态
            try
            {
                iret = rfidlib_reader.RDR_GetIOPortIDs(hreader, OutputIDs, ref OutputSize, InputIDs, ref InputSize);
                if (iret != 0)
                {
                    MessageBox.Show("fail");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Confirm Reader had open !");
            }
            /*
            for (int i = 0; i < OutputSize; i++)
            {
                //lstOutPutId.Add(OutputIDs[i]);
                if (OutputIDs[i] == 5)
                {
                    dic.Add($"继电器 {OutputIDs[i]}", "");
                }
                else if (OutputIDs[i] == 6)
                {
                    dic.Add($"MOS {OutputIDs[i]}", "");
                }
                else
                {
                    dic.Add($"输出端口 {OutputIDs[i]}", "");
                }
            }*/

            byte[] ids = new byte[InputSize];
            UInt32 nSize = (UInt32)ids.Length;
            for (int i = 0; i < InputSize; i++)
            {
                //lstInputID.Add(InputIDs[i]);
                if (OutputIDs[i] == 5)
                {
                    dic.Add("光耦"+OutputIDs[i], "");
                }
                else
                {
                    dic.Add("输入端口"+ InputIDs[i], "");
                }

                ids[i] = InputIDs[i];
            }

            iret = rfidlib_reader.RDR_GetInputStatus(hreader, ids, status, ref nSize);
            for (int i = 0; i < nSize; i++)
            {
                string state = string.Empty;
                if (status[i] == 0)
                {
                    state = "低电平";
                }
                else
                {
                    state = "高电平";
                }
                if (!dic.ContainsKey("输入端口 "+ids[i]) && ids[i] != 5)
                {
                    dic.Add("输入端口 "+ids[i], state);
                }
                else
                {
                    if (ids[i] == 5)
                    {
                        dic["光耦"+ids[i]]=state;
                    }
                    else
                    {
                        dic["输入端口"+ids[i]] = state;
                    }
                }

            }

            int serial = 1;
            foreach (KeyValuePair<string,string> item in dic)
            {
                ListViewItem lvi = new ListViewItem();

                lvi.Text = (serial++).ToString();
                lvi.SubItems.Add(item.Key);
                lvi.SubItems.Add(item.Value);
                listView1.Items.Add(lvi);
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            driver = (CReaderDriverInf)readerDriverInfoList[comboBox1.SelectedIndex];
            enumSerialNumber();
        }

        private void enumSerialNumber()
        {
            if ((driver.m_commTypeSupported & rfidlib_def.COMMTYPE_USB_EN) > 0)
            {
                comboBox7.Items.Clear();
                UInt32 nCount = rfidlib_reader.HID_Enum(driver.m_name);
                int iret;
                int i;
                for (i = 0; i < nCount; i++)
                {
                    StringBuilder sernum = new StringBuilder();
                    sernum.Append('\0', 64);
                    UInt32 nSize;
                    nSize = (UInt32)sernum.Capacity;
                    iret = rfidlib_reader.HID_GetEnumItem((UInt32)i, rfidlib_def.HID_ENUM_INF_TYPE_SERIALNUM, sernum, ref nSize);
                    if (iret == 0)
                    {
                        comboBox7.Items.Add(sernum.ToString());
                    }
                }
            }
        }

        private void enumCOMPort()
        {
            comboBox3.Items.Clear();
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
        }

        /// <summary>
        /// 设备管理消息
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if(m.Msg == WM_DEVICECHANGE)
            {
                if(m.WParam == DBT_DEVNODES_CHANGED)
                {
                    enumSerialNumber();
                    enumCOMPort();
                }
                if(m.WParam == DBT_DEVICEARRIVAL)
                {
                    enumSerialNumber();
                    enumCOMPort();
                }
                if (m.WParam == DBT_DEVICEREMOVECOMPLETE)
                {
                    enumSerialNumber();
                    enumCOMPort();
                }
            }
        }

    }
}
