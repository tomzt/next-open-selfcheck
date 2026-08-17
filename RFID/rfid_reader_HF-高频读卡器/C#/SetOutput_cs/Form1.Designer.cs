namespace SetOutput_cs
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cmbCommuncateType = new System.Windows.Forms.ComboBox();
            this.label35 = new System.Windows.Forms.Label();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.textPort = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.textIp = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbFrame = new System.Windows.Forms.ComboBox();
            this.cmbBaud = new System.Windows.Forms.ComboBox();
            this.cmbComName = new System.Windows.Forms.ComboBox();
            this.cmbDevType = new System.Windows.Forms.ComboBox();
            this.label29 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.btnSetOutput = new System.Windows.Forms.Button();
            this.textStopTime = new System.Windows.Forms.TextBox();
            this.cmbFrequency = new System.Windows.Forms.ComboBox();
            this.textActTime = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.checkedListOutput = new System.Windows.Forms.CheckedListBox();
            this.groupBox4.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cmbCommuncateType);
            this.groupBox4.Controls.Add(this.label35);
            this.groupBox4.Controls.Add(this.groupBox9);
            this.groupBox4.Controls.Add(this.groupBox7);
            this.groupBox4.Controls.Add(this.cmbDevType);
            this.groupBox4.Controls.Add(this.label29);
            this.groupBox4.Controls.Add(this.btnClose);
            this.groupBox4.Controls.Add(this.btnOpen);
            this.groupBox4.Location = new System.Drawing.Point(18, 18);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Size = new System.Drawing.Size(729, 300);
            this.groupBox4.TabIndex = 43;
            this.groupBox4.TabStop = false;
            // 
            // cmbCommuncateType
            // 
            this.cmbCommuncateType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCommuncateType.FormattingEnabled = true;
            this.cmbCommuncateType.Location = new System.Drawing.Point(202, 63);
            this.cmbCommuncateType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbCommuncateType.Name = "cmbCommuncateType";
            this.cmbCommuncateType.Size = new System.Drawing.Size(148, 26);
            this.cmbCommuncateType.TabIndex = 46;
            this.cmbCommuncateType.SelectedIndexChanged += new System.EventHandler(this.cmbCommuncateType_SelectedIndexChanged);
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(21, 68);
            this.label35.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(179, 18);
            this.label35.TabIndex = 45;
            this.label35.Text = "Communication type:";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.textPort);
            this.groupBox9.Controls.Add(this.label34);
            this.groupBox9.Controls.Add(this.textIp);
            this.groupBox9.Controls.Add(this.label33);
            this.groupBox9.Location = new System.Drawing.Point(375, 110);
            this.groupBox9.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox9.Size = new System.Drawing.Size(288, 141);
            this.groupBox9.TabIndex = 44;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "TCP Option";
            // 
            // textPort
            // 
            this.textPort.Enabled = false;
            this.textPort.Location = new System.Drawing.Point(126, 84);
            this.textPort.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textPort.Name = "textPort";
            this.textPort.Size = new System.Drawing.Size(82, 28);
            this.textPort.TabIndex = 3;
            this.textPort.Text = "9909";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(52, 90);
            this.label34.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(53, 18);
            this.label34.TabIndex = 2;
            this.label34.Text = "Port:";
            // 
            // textIp
            // 
            this.textIp.Enabled = false;
            this.textIp.Location = new System.Drawing.Point(126, 30);
            this.textIp.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textIp.Name = "textIp";
            this.textIp.Size = new System.Drawing.Size(140, 28);
            this.textIp.TabIndex = 1;
            this.textIp.Text = "192.168.0.222";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(30, 36);
            this.label33.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(80, 18);
            this.label33.TabIndex = 0;
            this.label33.Text = "IP Addr:";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label3);
            this.groupBox7.Controls.Add(this.label2);
            this.groupBox7.Controls.Add(this.label1);
            this.groupBox7.Controls.Add(this.cmbFrame);
            this.groupBox7.Controls.Add(this.cmbBaud);
            this.groupBox7.Controls.Add(this.cmbComName);
            this.groupBox7.Location = new System.Drawing.Point(18, 110);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox7.Size = new System.Drawing.Size(274, 165);
            this.groupBox7.TabIndex = 42;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Serial interface";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 112);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 18);
            this.label3.TabIndex = 39;
            this.label3.Text = "Frame:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 74);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 18);
            this.label2.TabIndex = 39;
            this.label2.Text = "Baud:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 34);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 18);
            this.label1.TabIndex = 39;
            this.label1.Text = "COM name:";
            // 
            // cmbFrame
            // 
            this.cmbFrame.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFrame.FormattingEnabled = true;
            this.cmbFrame.Items.AddRange(new object[] {
            "8E1",
            "8N1",
            "8O1"});
            this.cmbFrame.Location = new System.Drawing.Point(116, 108);
            this.cmbFrame.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbFrame.Name = "cmbFrame";
            this.cmbFrame.Size = new System.Drawing.Size(138, 26);
            this.cmbFrame.TabIndex = 38;
            // 
            // cmbBaud
            // 
            this.cmbBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBaud.FormattingEnabled = true;
            this.cmbBaud.Items.AddRange(new object[] {
            "9600",
            "38400",
            "57600",
            "115200"});
            this.cmbBaud.Location = new System.Drawing.Point(116, 69);
            this.cmbBaud.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbBaud.Name = "cmbBaud";
            this.cmbBaud.Size = new System.Drawing.Size(138, 26);
            this.cmbBaud.TabIndex = 38;
            // 
            // cmbComName
            // 
            this.cmbComName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbComName.FormattingEnabled = true;
            this.cmbComName.Location = new System.Drawing.Point(116, 30);
            this.cmbComName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbComName.Name = "cmbComName";
            this.cmbComName.Size = new System.Drawing.Size(138, 26);
            this.cmbComName.TabIndex = 38;
            // 
            // cmbDevType
            // 
            this.cmbDevType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDevType.FormattingEnabled = true;
            this.cmbDevType.Location = new System.Drawing.Point(202, 26);
            this.cmbDevType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbDevType.Name = "cmbDevType";
            this.cmbDevType.Size = new System.Drawing.Size(150, 26);
            this.cmbDevType.TabIndex = 41;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(20, 28);
            this.label29.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(116, 18);
            this.label29.TabIndex = 40;
            this.label29.Text = "Reader Type:";
            // 
            // btnClose
            // 
            this.btnClose.Enabled = false;
            this.btnClose.Location = new System.Drawing.Point(573, 39);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(130, 40);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "close reader";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(422, 38);
            this.btnOpen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(130, 40);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "open reader";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.btnSetOutput);
            this.groupBox1.Controls.Add(this.textStopTime);
            this.groupBox1.Controls.Add(this.cmbFrequency);
            this.groupBox1.Controls.Add(this.textActTime);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.checkedListOutput);
            this.groupBox1.Location = new System.Drawing.Point(18, 354);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(766, 274);
            this.groupBox1.TabIndex = 44;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "GPIO output";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(566, 156);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(71, 18);
            this.label16.TabIndex = 11;
            this.label16.Text = "* 100ms";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(584, 80);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(71, 18);
            this.label15.TabIndex = 10;
            this.label15.Text = "* 100ms";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label14.Location = new System.Drawing.Point(202, 26);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(521, 18);
            this.label14.TabIndex = 9;
            this.label14.Text = "Note:Pause duration will be Effective only when number >1";
            // 
            // btnSetOutput
            // 
            this.btnSetOutput.Enabled = false;
            this.btnSetOutput.Location = new System.Drawing.Point(402, 207);
            this.btnSetOutput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSetOutput.Name = "btnSetOutput";
            this.btnSetOutput.Size = new System.Drawing.Size(138, 46);
            this.btnSetOutput.TabIndex = 8;
            this.btnSetOutput.Text = "Set Output";
            this.btnSetOutput.UseVisualStyleBackColor = true;
            this.btnSetOutput.Click += new System.EventHandler(this.btnSetOutput_Click);
            // 
            // textStopTime
            // 
            this.textStopTime.Location = new System.Drawing.Point(420, 153);
            this.textStopTime.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textStopTime.Name = "textStopTime";
            this.textStopTime.Size = new System.Drawing.Size(136, 28);
            this.textStopTime.TabIndex = 7;
            this.textStopTime.Text = "1";
            // 
            // cmbFrequency
            // 
            this.cmbFrequency.FormattingEnabled = true;
            this.cmbFrequency.Location = new System.Drawing.Point(420, 114);
            this.cmbFrequency.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbFrequency.Name = "cmbFrequency";
            this.cmbFrequency.Size = new System.Drawing.Size(138, 26);
            this.cmbFrequency.TabIndex = 6;
            // 
            // textActTime
            // 
            this.textActTime.Location = new System.Drawing.Point(420, 74);
            this.textActTime.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textActTime.Name = "textActTime";
            this.textActTime.Size = new System.Drawing.Size(136, 28);
            this.textActTime.TabIndex = 5;
            this.textActTime.Text = "1";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(306, 156);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(98, 18);
            this.label13.TabIndex = 4;
            this.label13.Text = "Stop time:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(306, 118);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(98, 18);
            this.label12.TabIndex = 3;
            this.label12.Text = "Frequency:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(306, 78);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(116, 18);
            this.label11.TabIndex = 2;
            this.label11.Text = "Active time:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(30, 45);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(125, 18);
            this.label10.TabIndex = 1;
            this.label10.Text = "Warning Port:";
            // 
            // checkedListOutput
            // 
            this.checkedListOutput.FormattingEnabled = true;
            this.checkedListOutput.Location = new System.Drawing.Point(26, 74);
            this.checkedListOutput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkedListOutput.Name = "checkedListOutput";
            this.checkedListOutput.Size = new System.Drawing.Size(194, 154);
            this.checkedListOutput.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 674);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox4);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "Set Output V2.0";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox cmbCommuncateType;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.TextBox textPort;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.TextBox textIp;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbComName;
        private System.Windows.Forms.ComboBox cmbDevType;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbFrame;
        private System.Windows.Forms.ComboBox cmbBaud;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btnSetOutput;
        private System.Windows.Forms.TextBox textStopTime;
        private System.Windows.Forms.ComboBox cmbFrequency;
        private System.Windows.Forms.TextBox textActTime;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckedListBox checkedListOutput;
    }
}

