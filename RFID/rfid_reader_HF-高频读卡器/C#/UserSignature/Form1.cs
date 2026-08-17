using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RFIDLIB;
namespace UserSignature
{
    public partial class Form1 : Form
    {

        UIntPtr hreader = UIntPtr.Zero;


        public Form1()
        {
            InitializeComponent();


            rfidlib_reader.RDR_LoadReaderDrivers("\\Drivers");


            cbbCommType.SelectedIndex = 0;
            cbbUsbType.Items.Add("None addressed");
            cbbUsbType.Items.Add("Serial number");
            cbbUsbType.SelectedIndex = 0;


            uint cnt=rfidlib_reader.COMPort_Enum();

            comboBoxCOM.Items.Clear();
            UInt32 nCOMCnt = RFIDLIB.rfidlib_reader.COMPort_Enum();
            for (uint i = 0; i < nCOMCnt; i++)
            {
                StringBuilder comName = new StringBuilder();
                comName.Append('\0', 64);
                RFIDLIB.rfidlib_reader.COMPort_GetEnumItem(i, comName, (UInt32)comName.Capacity);
                comboBoxCOM.Items.Add(comName);
            }

            if (comboBoxCOM.Items.Count > 0)
                comboBoxCOM.SelectedIndex = 0;

            buttonOpen.Enabled = true;
            buttonClose.Enabled = false;

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;

            cbbBaud.SelectedIndex = 1;
            cbbFrame.SelectedIndex = 0;

            cbbUsbType.SelectedIndex = 0;

        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            if (cbbCommType.SelectedIndex == -1)
            {
                MessageBox.Show("select communication type");
                return;
            }


            buttonOpen.Enabled = false;

            Byte usbOpenType = 0;
            usbOpenType = (Byte)cbbUsbType.SelectedIndex;


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
                ipAddr = ipAddressText.Text;
                port = (UInt16)int.Parse(portText.Text);
                connstr = RFIDLIB.rfidlib_def.CONNSTR_NAME_RDTYPE + "=" + readerDriverName + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE + "=" + RFIDLIB.rfidlib_def.CONNSTR_NAME_COMMTYPE_NET + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_REMOTEIP + "=" + ipAddr + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_REMOTEPORT + "=" + port.ToString() + ";" +
                          RFIDLIB.rfidlib_def.CONNSTR_NAME_LOCALIP + "=" + "";
            }
          
            // Call required to open reader driver
          int  iret = RFIDLIB.rfidlib_reader.RDR_Open(connstr, ref hreader);

            if (iret != 0)
            {

                buttonOpen.Enabled =true;

            }
            else
            {
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                buttonOpen.Enabled = false;
                buttonClose.Enabled = true;
            }



        }

        private void cbbUsbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbUsbType.SelectedIndex == 0)
            {
                cbbUsbSerial.Enabled = false;
            }
            else
            {
                cbbUsbSerial.Enabled = true;
            }


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

            if (cbbUsbSerial.Items.Count>0)     
            cbbUsbSerial.SelectedIndex = 0;

        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            int iret=  rfidlib_reader.RDR_Close(hreader);

            buttonOpen.Enabled = true;
            buttonClose.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            hreader = UIntPtr.Zero;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";


            byte[] data=new byte[255];
            byte size=255;
            int iret= rfidlib_reader.RDR_GetDeviceUniqueID(hreader, data, ref size);

            if (iret == 0)
            {
                for (int i = 0; i < size / 4; i++)
                {
                    int startindex = i * 4;

                    int cnt = 4;

                    if (startindex + cnt > size)
                    {
                        cnt = size - startindex;
                    }

                    for (int j = startindex; j < startindex + cnt; j++)
                    {
                        richTextBox1.Text += data[j].ToString("X2");
                    }

                    richTextBox1.Text += "\r\n";
                }

            }
            else
            {
                

                richTextBox1.Text = "Fail!";
            }
        }

        private void cbbCommType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

            richTextBox2.Text = "";

            byte[] data = new byte[255];
            byte size = 255;
            int iret = rfidlib_reader.RDR_ReadUserSignature(hreader, data, ref size);

            if (iret == 0)
            {
                for (int i = 0; i < size / 4; i++)
                {
                    int startindex = i * 4;

                    int cnt = 4;

                    if (startindex + cnt > size)
                    {
                        cnt = size - startindex;
                    }

                    for (int j = startindex; j < startindex + cnt; j++)
                    {
                        richTextBox2.Text += data[j].ToString("X2");
                    }

                    richTextBox2.Text += "\r\n";
                }
            }
            else
            {
           

             MessageBox.Show("Fail");
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

        private void button3_Click(object sender, EventArgs e)
        {

            String str =richTextBox2.Text.Replace("\r", "").Replace("\n","").Trim();


            byte[] src=   StringToByteArrayFastest(str);
            byte[] data = new byte[32];
            byte size = 32;


            System.Array.Copy(src, data, data.Length > src.Length ? src.Length : data.Length);


            int iret = rfidlib_reader.RDR_UpdateUserSignature(hreader, data,  size);

            if (iret == 0)
            {
                MessageBox.Show("Success!");
            }
            else
            {
                MessageBox.Show("Fail!");
            }
        }



    }
}