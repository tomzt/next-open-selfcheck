namespace UserSignature
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.cbbCommType = new System.Windows.Forms.ComboBox();
            this.label35 = new System.Windows.Forms.Label();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.portText = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.ipAddressText = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.cbbUsbSerial = new System.Windows.Forms.ComboBox();
            this.label32 = new System.Windows.Forms.Label();
            this.cbbUsbType = new System.Windows.Forms.ComboBox();
            this.label31 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.comboBoxCOM = new System.Windows.Forms.ComboBox();
            this.cbbFrame = new System.Windows.Forms.ComboBox();
            this.cbbBaud = new System.Windows.Forms.ComboBox();
            this.label43 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonOpen);
            this.groupBox1.Controls.Add(this.buttonClose);
            this.groupBox1.Controls.Add(this.cbbCommType);
            this.groupBox1.Controls.Add(this.label35);
            this.groupBox1.Controls.Add(this.groupBox9);
            this.groupBox1.Controls.Add(this.groupBox8);
            this.groupBox1.Controls.Add(this.groupBox7);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(747, 161);
            this.groupBox1.TabIndex = 44;
            this.groupBox1.TabStop = false;
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(265, 16);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(82, 26);
            this.buttonOpen.TabIndex = 5;
            this.buttonOpen.Text = "Open";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(371, 16);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(82, 26);
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // cbbCommType
            // 
            this.cbbCommType.FormattingEnabled = true;
            this.cbbCommType.Items.AddRange(new object[] {
            "COM",
            "USB",
            "TCP"});
            this.cbbCommType.Location = new System.Drawing.Point(132, 16);
            this.cbbCommType.Name = "cbbCommType";
            this.cbbCommType.Size = new System.Drawing.Size(100, 20);
            this.cbbCommType.TabIndex = 46;
            this.cbbCommType.SelectedIndexChanged += new System.EventHandler(this.cbbCommType_SelectedIndexChanged);
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(20, 19);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(119, 12);
            this.label35.TabIndex = 45;
            this.label35.Text = "Communication type:";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.portText);
            this.groupBox9.Controls.Add(this.label34);
            this.groupBox9.Controls.Add(this.ipAddressText);
            this.groupBox9.Controls.Add(this.label33);
            this.groupBox9.Location = new System.Drawing.Point(476, 50);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(244, 47);
            this.groupBox9.TabIndex = 44;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "TCP Option";
            // 
            // portText
            // 
            this.portText.Location = new System.Drawing.Point(199, 20);
            this.portText.Name = "portText";
            this.portText.Size = new System.Drawing.Size(36, 21);
            this.portText.TabIndex = 3;
            this.portText.Text = "9909";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(163, 24);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(35, 12);
            this.label34.TabIndex = 2;
            this.label34.Text = "Port:";
            // 
            // ipAddressText
            // 
            this.ipAddressText.Location = new System.Drawing.Point(63, 20);
            this.ipAddressText.Name = "ipAddressText";
            this.ipAddressText.Size = new System.Drawing.Size(95, 21);
            this.ipAddressText.TabIndex = 1;
            this.ipAddressText.Text = "10.168.1.222";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(10, 24);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(53, 12);
            this.label33.TabIndex = 0;
            this.label33.Text = "IP Addr:";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.cbbUsbSerial);
            this.groupBox8.Controls.Add(this.label32);
            this.groupBox8.Controls.Add(this.cbbUsbType);
            this.groupBox8.Controls.Add(this.label31);
            this.groupBox8.Location = new System.Drawing.Point(209, 51);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(262, 100);
            this.groupBox8.TabIndex = 43;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "USB interface";
            // 
            // cbbUsbSerial
            // 
            this.cbbUsbSerial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbUsbSerial.FormattingEnabled = true;
            this.cbbUsbSerial.Location = new System.Drawing.Point(109, 63);
            this.cbbUsbSerial.Name = "cbbUsbSerial";
            this.cbbUsbSerial.Size = new System.Drawing.Size(122, 20);
            this.cbbUsbSerial.TabIndex = 3;
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(15, 63);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(89, 12);
            this.label32.TabIndex = 2;
            this.label32.Text = "Serial Number:";
            // 
            // cbbUsbType
            // 
            this.cbbUsbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbUsbType.FormattingEnabled = true;
            this.cbbUsbType.Location = new System.Drawing.Point(109, 22);
            this.cbbUsbType.Name = "cbbUsbType";
            this.cbbUsbType.Size = new System.Drawing.Size(122, 20);
            this.cbbUsbType.TabIndex = 1;
            this.cbbUsbType.SelectedIndexChanged += new System.EventHandler(this.cbbUsbType_SelectedIndexChanged);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(16, 24);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(89, 12);
            this.label31.TabIndex = 0;
            this.label31.Text = "USB Open type:";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.comboBoxCOM);
            this.groupBox7.Controls.Add(this.cbbFrame);
            this.groupBox7.Controls.Add(this.cbbBaud);
            this.groupBox7.Controls.Add(this.label43);
            this.groupBox7.Controls.Add(this.label42);
            this.groupBox7.Controls.Add(this.label5);
            this.groupBox7.Location = new System.Drawing.Point(18, 48);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(183, 100);
            this.groupBox7.TabIndex = 42;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Serial interface";
            // 
            // comboBoxCOM
            // 
            this.comboBoxCOM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCOM.FormattingEnabled = true;
            this.comboBoxCOM.Location = new System.Drawing.Point(77, 19);
            this.comboBoxCOM.Name = "comboBoxCOM";
            this.comboBoxCOM.Size = new System.Drawing.Size(93, 20);
            this.comboBoxCOM.TabIndex = 1;
            // 
            // cbbFrame
            // 
            this.cbbFrame.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbFrame.FormattingEnabled = true;
            this.cbbFrame.Items.AddRange(new object[] {
            "8E1",
            "8N1",
            "8O1"});
            this.cbbFrame.Location = new System.Drawing.Point(77, 73);
            this.cbbFrame.Name = "cbbFrame";
            this.cbbFrame.Size = new System.Drawing.Size(93, 20);
            this.cbbFrame.TabIndex = 43;
            // 
            // cbbBaud
            // 
            this.cbbBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbBaud.FormattingEnabled = true;
            this.cbbBaud.Items.AddRange(new object[] {
            "9600",
            "38400",
            "57600",
            "115200"});
            this.cbbBaud.Location = new System.Drawing.Point(77, 47);
            this.cbbBaud.Name = "cbbBaud";
            this.cbbBaud.Size = new System.Drawing.Size(93, 20);
            this.cbbBaud.TabIndex = 42;
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(30, 76);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(41, 12);
            this.label43.TabIndex = 41;
            this.label43.Text = "Frame:";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(36, 50);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(35, 12);
            this.label42.TabIndex = 40;
            this.label42.Text = "Baud:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 39;
            this.label5.Text = "COM name:";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(18, 208);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(235, 98);
            this.richTextBox1.TabIndex = 45;
            this.richTextBox1.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 193);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 46;
            this.label1.Text = "Unique ID:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(171, 312);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(82, 35);
            this.button1.TabIndex = 47;
            this.button1.Text = "Read";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(323, 190);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 48;
            this.label2.Text = "Signature:";
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(319, 205);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(246, 163);
            this.richTextBox2.TabIndex = 49;
            this.richTextBox2.Text = "";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(571, 205);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(82, 35);
            this.button2.TabIndex = 50;
            this.button2.Text = "Read";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(571, 253);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(82, 35);
            this.button3.TabIndex = 51;
            this.button3.Text = "Update";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 380);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.richTextBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.ComboBox cbbCommType;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.TextBox portText;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox ipAddressText;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.ComboBox cbbUsbSerial;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.ComboBox cbbUsbType;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.ComboBox comboBoxCOM;
        private System.Windows.Forms.ComboBox cbbFrame;
        private System.Windows.Forms.ComboBox cbbBaud;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

