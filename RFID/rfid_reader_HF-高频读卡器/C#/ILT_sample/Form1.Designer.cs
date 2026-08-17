namespace ILT_sample
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
            this.bntOpen = new System.Windows.Forms.Button();
            this.bntClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.bntWrite = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bntInventory = new System.Windows.Forms.Button();
            this.labTotal = new System.Windows.Forms.Label();
            this.lstbTagFound = new System.Windows.Forms.ListBox();
            this.cmbTagsFound = new System.Windows.Forms.ComboBox();
            this.bntConnect = new System.Windows.Forms.Button();
            this.bntDisconnect = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbMemBank = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbStartWord = new System.Windows.Forms.ComboBox();
            this.ckbSetPwd = new System.Windows.Forms.CheckBox();
            this.txbPwd = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.bntRead = new System.Windows.Forms.Button();
            this.txbData = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbWordCnt = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bntLock = new System.Windows.Forms.Button();
            this.cmbKillPwd = new System.Windows.Forms.ComboBox();
            this.ckbKillPwd = new System.Windows.Forms.CheckBox();
            this.cmbAccessPwd = new System.Windows.Forms.ComboBox();
            this.ckbAccessPwd = new System.Windows.Forms.CheckBox();
            this.cmbEPC = new System.Windows.Forms.ComboBox();
            this.ckbEPC = new System.Windows.Forms.CheckBox();
            this.cmbTID = new System.Windows.Forms.ComboBox();
            this.ckbTID = new System.Windows.Forms.CheckBox();
            this.cmbUser = new System.Windows.Forms.ComboBox();
            this.ckbUser = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.ckbRecomLSB3 = new System.Windows.Forms.CheckBox();
            this.ckbRecomLSB2 = new System.Windows.Forms.CheckBox();
            this.ckbRecomLSB = new System.Windows.Forms.CheckBox();
            this.txbKillPwd = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.cmbEPCLen = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // bntOpen
            // 
            this.bntOpen.Location = new System.Drawing.Point(18, 38);
            this.bntOpen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bntOpen.Name = "bntOpen";
            this.bntOpen.Size = new System.Drawing.Size(144, 54);
            this.bntOpen.TabIndex = 0;
            this.bntOpen.Text = "Open";
            this.bntOpen.UseVisualStyleBackColor = true;
            this.bntOpen.Click += new System.EventHandler(this.button1_Click);
            // 
            // bntClose
            // 
            this.bntClose.Enabled = false;
            this.bntClose.Location = new System.Drawing.Point(192, 38);
            this.bntClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bntClose.Name = "bntClose";
            this.bntClose.Size = new System.Drawing.Size(144, 54);
            this.bntClose.TabIndex = 1;
            this.bntClose.Text = "Close";
            this.bntClose.UseVisualStyleBackColor = true;
            this.bntClose.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(604, 195);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "EPC：";
            // 
            // bntWrite
            // 
            this.bntWrite.Location = new System.Drawing.Point(423, 171);
            this.bntWrite.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bntWrite.Name = "bntWrite";
            this.bntWrite.Size = new System.Drawing.Size(120, 50);
            this.bntWrite.TabIndex = 5;
            this.bntWrite.Text = "Write";
            this.bntWrite.UseVisualStyleBackColor = true;
            this.bntWrite.Click += new System.EventHandler(this.button3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1024, 159);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 18);
            this.label2.TabIndex = 6;
            this.label2.Text = "Hex";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bntInventory);
            this.groupBox1.Controls.Add(this.labTotal);
            this.groupBox1.Controls.Add(this.lstbTagFound);
            this.groupBox1.Location = new System.Drawing.Point(21, 124);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(514, 642);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            // 
            // bntInventory
            // 
            this.bntInventory.Location = new System.Drawing.Point(357, 84);
            this.bntInventory.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bntInventory.Name = "bntInventory";
            this.bntInventory.Size = new System.Drawing.Size(141, 68);
            this.bntInventory.TabIndex = 2;
            this.bntInventory.Text = "Inventory";
            this.bntInventory.UseVisualStyleBackColor = true;
            this.bntInventory.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // labTotal
            // 
            this.labTotal.AutoSize = true;
            this.labTotal.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labTotal.Location = new System.Drawing.Point(16, 30);
            this.labTotal.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labTotal.Name = "labTotal";
            this.labTotal.Size = new System.Drawing.Size(221, 29);
            this.labTotal.TabIndex = 1;
            this.labTotal.Text = "Tags Found: 0";
            // 
            // lstbTagFound
            // 
            this.lstbTagFound.FormattingEnabled = true;
            this.lstbTagFound.HorizontalScrollbar = true;
            this.lstbTagFound.ItemHeight = 18;
            this.lstbTagFound.Location = new System.Drawing.Point(9, 84);
            this.lstbTagFound.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lstbTagFound.Name = "lstbTagFound";
            this.lstbTagFound.Size = new System.Drawing.Size(337, 544);
            this.lstbTagFound.TabIndex = 0;
            // 
            // cmbTagsFound
            // 
            this.cmbTagsFound.FormattingEnabled = true;
            this.cmbTagsFound.Location = new System.Drawing.Point(651, 190);
            this.cmbTagsFound.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbTagsFound.Name = "cmbTagsFound";
            this.cmbTagsFound.Size = new System.Drawing.Size(376, 26);
            this.cmbTagsFound.TabIndex = 11;
            // 
            // bntConnect
            // 
            this.bntConnect.Location = new System.Drawing.Point(1038, 195);
            this.bntConnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bntConnect.Name = "bntConnect";
            this.bntConnect.Size = new System.Drawing.Size(87, 33);
            this.bntConnect.TabIndex = 12;
            this.bntConnect.Text = "Connect";
            this.bntConnect.UseVisualStyleBackColor = true;
            this.bntConnect.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // bntDisconnect
            // 
            this.bntDisconnect.Enabled = false;
            this.bntDisconnect.Location = new System.Drawing.Point(1132, 195);
            this.bntDisconnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bntDisconnect.Name = "bntDisconnect";
            this.bntDisconnect.Size = new System.Drawing.Size(116, 33);
            this.bntDisconnect.TabIndex = 13;
            this.bntDisconnect.Text = "Disconnect";
            this.bntDisconnect.UseVisualStyleBackColor = true;
            this.bntDisconnect.Click += new System.EventHandler(this.bntDisconnect_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 34);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 18);
            this.label3.TabIndex = 14;
            this.label3.Text = "Mem Bank:";
            // 
            // cmbMemBank
            // 
            this.cmbMemBank.FormattingEnabled = true;
            this.cmbMemBank.Items.AddRange(new object[] {
            "Reserved",
            "EPC",
            "TID",
            "USER"});
            this.cmbMemBank.Location = new System.Drawing.Point(122, 27);
            this.cmbMemBank.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbMemBank.Name = "cmbMemBank";
            this.cmbMemBank.Size = new System.Drawing.Size(152, 26);
            this.cmbMemBank.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 76);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 18);
            this.label4.TabIndex = 16;
            this.label4.Text = "Start Word:";
            // 
            // cmbStartWord
            // 
            this.cmbStartWord.FormattingEnabled = true;
            this.cmbStartWord.Location = new System.Drawing.Point(122, 72);
            this.cmbStartWord.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbStartWord.Name = "cmbStartWord";
            this.cmbStartWord.Size = new System.Drawing.Size(162, 26);
            this.cmbStartWord.TabIndex = 18;
            // 
            // ckbSetPwd
            // 
            this.ckbSetPwd.AutoSize = true;
            this.ckbSetPwd.Location = new System.Drawing.Point(651, 158);
            this.ckbSetPwd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbSetPwd.Name = "ckbSetPwd";
            this.ckbSetPwd.Size = new System.Drawing.Size(160, 22);
            this.ckbSetPwd.TabIndex = 20;
            this.ckbSetPwd.Text = "Set password ?";
            this.ckbSetPwd.UseVisualStyleBackColor = true;
            // 
            // txbPwd
            // 
            this.txbPwd.Location = new System.Drawing.Point(910, 154);
            this.txbPwd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txbPwd.MaxLength = 8;
            this.txbPwd.Name = "txbPwd";
            this.txbPwd.Size = new System.Drawing.Size(97, 28);
            this.txbPwd.TabIndex = 21;
            this.txbPwd.Text = "00000000";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(822, 164);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 18);
            this.label6.TabIndex = 22;
            this.label6.Text = "Password:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(60, 130);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 18);
            this.label7.TabIndex = 23;
            this.label7.Text = "Data:";
            // 
            // bntRead
            // 
            this.bntRead.Location = new System.Drawing.Point(285, 166);
            this.bntRead.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bntRead.Name = "bntRead";
            this.bntRead.Size = new System.Drawing.Size(108, 50);
            this.bntRead.TabIndex = 24;
            this.bntRead.Text = "Read";
            this.bntRead.UseVisualStyleBackColor = true;
            this.bntRead.Click += new System.EventHandler(this.bntRead_Click);
            // 
            // txbData
            // 
            this.txbData.Location = new System.Drawing.Point(122, 117);
            this.txbData.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txbData.Name = "txbData";
            this.txbData.Size = new System.Drawing.Size(463, 28);
            this.txbData.TabIndex = 25;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 182);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 18);
            this.label5.TabIndex = 26;
            this.label5.Text = "Word Cnt:";
            // 
            // cmbWordCnt
            // 
            this.cmbWordCnt.FormattingEnabled = true;
            this.cmbWordCnt.Location = new System.Drawing.Point(122, 177);
            this.cmbWordCnt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbWordCnt.Name = "cmbWordCnt";
            this.cmbWordCnt.Size = new System.Drawing.Size(124, 26);
            this.cmbWordCnt.TabIndex = 27;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(123, 158);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(125, 18);
            this.label8.TabIndex = 28;
            this.label8.Text = "Only for read";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.cmbWordCnt);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txbData);
            this.groupBox2.Controls.Add(this.bntRead);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.cmbStartWord);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.cmbMemBank);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.bntWrite);
            this.groupBox2.Location = new System.Drawing.Point(568, 237);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(678, 236);
            this.groupBox2.TabIndex = 29;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Read/Write";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.bntLock);
            this.groupBox3.Controls.Add(this.cmbKillPwd);
            this.groupBox3.Controls.Add(this.ckbKillPwd);
            this.groupBox3.Controls.Add(this.cmbAccessPwd);
            this.groupBox3.Controls.Add(this.ckbAccessPwd);
            this.groupBox3.Controls.Add(this.cmbEPC);
            this.groupBox3.Controls.Add(this.ckbEPC);
            this.groupBox3.Controls.Add(this.cmbTID);
            this.groupBox3.Controls.Add(this.ckbTID);
            this.groupBox3.Controls.Add(this.cmbUser);
            this.groupBox3.Controls.Add(this.ckbUser);
            this.groupBox3.Location = new System.Drawing.Point(567, 494);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Size = new System.Drawing.Size(681, 324);
            this.groupBox3.TabIndex = 30;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Lock";
            // 
            // bntLock
            // 
            this.bntLock.Location = new System.Drawing.Point(153, 276);
            this.bntLock.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bntLock.Name = "bntLock";
            this.bntLock.Size = new System.Drawing.Size(124, 38);
            this.bntLock.TabIndex = 10;
            this.bntLock.Text = "Lock";
            this.bntLock.UseVisualStyleBackColor = true;
            this.bntLock.Click += new System.EventHandler(this.bntLock_Click);
            // 
            // cmbKillPwd
            // 
            this.cmbKillPwd.FormattingEnabled = true;
            this.cmbKillPwd.Items.AddRange(new object[] {
            "b00: readable and writable from either open or secured states",
            "b01: readable and writable from either open or secured states and states never be" +
                " changed",
            "b10: readable and writable only from secured state",
            "b11: not readable and writable from any states"});
            this.cmbKillPwd.Location = new System.Drawing.Point(156, 222);
            this.cmbKillPwd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbKillPwd.Name = "cmbKillPwd";
            this.cmbKillPwd.Size = new System.Drawing.Size(493, 26);
            this.cmbKillPwd.TabIndex = 9;
            // 
            // ckbKillPwd
            // 
            this.ckbKillPwd.AutoSize = true;
            this.ckbKillPwd.Location = new System.Drawing.Point(22, 220);
            this.ckbKillPwd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbKillPwd.Name = "ckbKillPwd";
            this.ckbKillPwd.Size = new System.Drawing.Size(115, 22);
            this.ckbKillPwd.TabIndex = 8;
            this.ckbKillPwd.Text = "Kill Pwd:";
            this.ckbKillPwd.UseVisualStyleBackColor = true;
            // 
            // cmbAccessPwd
            // 
            this.cmbAccessPwd.FormattingEnabled = true;
            this.cmbAccessPwd.Items.AddRange(new object[] {
            "b00: readable and writable from either open or secured states",
            "b01: readable and writable from either open or secured states and states never be" +
                " changed",
            "b10: readable and writable only from secured state",
            "b11: not readable and writable from any states"});
            this.cmbAccessPwd.Location = new System.Drawing.Point(154, 170);
            this.cmbAccessPwd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbAccessPwd.Name = "cmbAccessPwd";
            this.cmbAccessPwd.Size = new System.Drawing.Size(494, 26);
            this.cmbAccessPwd.TabIndex = 7;
            // 
            // ckbAccessPwd
            // 
            this.ckbAccessPwd.AutoSize = true;
            this.ckbAccessPwd.Location = new System.Drawing.Point(21, 171);
            this.ckbAccessPwd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbAccessPwd.Name = "ckbAccessPwd";
            this.ckbAccessPwd.Size = new System.Drawing.Size(133, 22);
            this.ckbAccessPwd.TabIndex = 6;
            this.ckbAccessPwd.Text = "Access Pwd:";
            this.ckbAccessPwd.UseVisualStyleBackColor = true;
            // 
            // cmbEPC
            // 
            this.cmbEPC.FormattingEnabled = true;
            this.cmbEPC.Items.AddRange(new object[] {
            "b00: writable from either open or secured states",
            "b01: writable from either open or secured states and states never be changed",
            "b10: writable only from secured state",
            "b11: not writable from any states"});
            this.cmbEPC.Location = new System.Drawing.Point(99, 124);
            this.cmbEPC.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbEPC.Name = "cmbEPC";
            this.cmbEPC.Size = new System.Drawing.Size(550, 26);
            this.cmbEPC.TabIndex = 5;
            // 
            // ckbEPC
            // 
            this.ckbEPC.AutoSize = true;
            this.ckbEPC.Location = new System.Drawing.Point(21, 126);
            this.ckbEPC.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbEPC.Name = "ckbEPC";
            this.ckbEPC.Size = new System.Drawing.Size(70, 22);
            this.ckbEPC.TabIndex = 4;
            this.ckbEPC.Text = "EPC:";
            this.ckbEPC.UseVisualStyleBackColor = true;
            // 
            // cmbTID
            // 
            this.cmbTID.FormattingEnabled = true;
            this.cmbTID.Items.AddRange(new object[] {
            "b00: writable from either open or secured states",
            "b01: writable from either open or secured states and states never be changed",
            "b10: writable only from secured state",
            "b11: not writable from any states"});
            this.cmbTID.Location = new System.Drawing.Point(99, 80);
            this.cmbTID.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbTID.Name = "cmbTID";
            this.cmbTID.Size = new System.Drawing.Size(550, 26);
            this.cmbTID.TabIndex = 3;
            // 
            // ckbTID
            // 
            this.ckbTID.AutoSize = true;
            this.ckbTID.Location = new System.Drawing.Point(20, 81);
            this.ckbTID.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbTID.Name = "ckbTID";
            this.ckbTID.Size = new System.Drawing.Size(70, 22);
            this.ckbTID.TabIndex = 2;
            this.ckbTID.Text = "TID:";
            this.ckbTID.UseVisualStyleBackColor = true;
            // 
            // cmbUser
            // 
            this.cmbUser.FormattingEnabled = true;
            this.cmbUser.Items.AddRange(new object[] {
            "b00: writable from either open or secured states",
            "b01: writable from either open or secured states and states never be changed",
            "b10: writable only from secured state",
            "b11: not writable from any states"});
            this.cmbUser.Location = new System.Drawing.Point(99, 36);
            this.cmbUser.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbUser.Name = "cmbUser";
            this.cmbUser.Size = new System.Drawing.Size(550, 26);
            this.cmbUser.TabIndex = 1;
            // 
            // ckbUser
            // 
            this.ckbUser.AutoSize = true;
            this.ckbUser.Location = new System.Drawing.Point(18, 39);
            this.ckbUser.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbUser.Name = "ckbUser";
            this.ckbUser.Size = new System.Drawing.Size(79, 22);
            this.ckbUser.TabIndex = 0;
            this.ckbUser.Text = "User:";
            this.ckbUser.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button1);
            this.groupBox4.Controls.Add(this.groupBox5);
            this.groupBox4.Controls.Add(this.txbKillPwd);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Location = new System.Drawing.Point(1260, 237);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Size = new System.Drawing.Size(220, 320);
            this.groupBox4.TabIndex = 31;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Kill";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(21, 256);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(136, 50);
            this.button1.TabIndex = 6;
            this.button1.Text = "Kill";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_3);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.ckbRecomLSB3);
            this.groupBox5.Controls.Add(this.ckbRecomLSB2);
            this.groupBox5.Controls.Add(this.ckbRecomLSB);
            this.groupBox5.Location = new System.Drawing.Point(10, 117);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox5.Size = new System.Drawing.Size(166, 118);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Recommission";
            // 
            // ckbRecomLSB3
            // 
            this.ckbRecomLSB3.AutoSize = true;
            this.ckbRecomLSB3.Location = new System.Drawing.Point(10, 84);
            this.ckbRecomLSB3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbRecomLSB3.Name = "ckbRecomLSB3";
            this.ckbRecomLSB3.Size = new System.Drawing.Size(70, 22);
            this.ckbRecomLSB3.TabIndex = 2;
            this.ckbRecomLSB3.Text = "3LSB";
            this.ckbRecomLSB3.UseVisualStyleBackColor = true;
            // 
            // ckbRecomLSB2
            // 
            this.ckbRecomLSB2.AutoSize = true;
            this.ckbRecomLSB2.Location = new System.Drawing.Point(10, 56);
            this.ckbRecomLSB2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbRecomLSB2.Name = "ckbRecomLSB2";
            this.ckbRecomLSB2.Size = new System.Drawing.Size(70, 22);
            this.ckbRecomLSB2.TabIndex = 1;
            this.ckbRecomLSB2.Text = "2LSB";
            this.ckbRecomLSB2.UseVisualStyleBackColor = true;
            // 
            // ckbRecomLSB
            // 
            this.ckbRecomLSB.AutoSize = true;
            this.ckbRecomLSB.Location = new System.Drawing.Point(10, 26);
            this.ckbRecomLSB.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbRecomLSB.Name = "ckbRecomLSB";
            this.ckbRecomLSB.Size = new System.Drawing.Size(61, 22);
            this.ckbRecomLSB.TabIndex = 0;
            this.ckbRecomLSB.Text = "LSB";
            this.ckbRecomLSB.UseVisualStyleBackColor = true;
            // 
            // txbKillPwd
            // 
            this.txbKillPwd.Location = new System.Drawing.Point(9, 63);
            this.txbKillPwd.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txbKillPwd.MaxLength = 8;
            this.txbKillPwd.Name = "txbKillPwd";
            this.txbKillPwd.Size = new System.Drawing.Size(166, 28);
            this.txbKillPwd.TabIndex = 4;
            this.txbKillPwd.Text = "00000000";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 39);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(134, 18);
            this.label9.TabIndex = 3;
            this.label9.Text = "kill password:";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.cmbEPCLen);
            this.groupBox6.Controls.Add(this.button3);
            this.groupBox6.Controls.Add(this.button2);
            this.groupBox6.Controls.Add(this.label10);
            this.groupBox6.Location = new System.Drawing.Point(1260, 566);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox6.Size = new System.Drawing.Size(219, 156);
            this.groupBox6.TabIndex = 32;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Change EPC Length";
            // 
            // cmbEPCLen
            // 
            this.cmbEPCLen.FormattingEnabled = true;
            this.cmbEPCLen.Location = new System.Drawing.Point(104, 40);
            this.cmbEPCLen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbEPCLen.Name = "cmbEPCLen";
            this.cmbEPCLen.Size = new System.Drawing.Size(104, 26);
            this.cmbEPCLen.TabIndex = 5;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(123, 86);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(72, 42);
            this.button3.TabIndex = 4;
            this.button3.Text = "Write";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(21, 86);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(72, 42);
            this.button2.TabIndex = 3;
            this.button2.Text = "Read";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 45);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(89, 18);
            this.label10.TabIndex = 0;
            this.label10.Text = "Word Cnt:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1498, 825);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txbPwd);
            this.Controls.Add(this.ckbSetPwd);
            this.Controls.Add(this.bntDisconnect);
            this.Controls.Add(this.bntConnect);
            this.Controls.Add(this.cmbTagsFound);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bntClose);
            this.Controls.Add(this.bntOpen);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "EPC write sample V2.0";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bntOpen;
        private System.Windows.Forms.Button bntClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bntWrite;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bntInventory;
        private System.Windows.Forms.Label labTotal;
        private System.Windows.Forms.ListBox lstbTagFound;
        private System.Windows.Forms.ComboBox cmbTagsFound;
        private System.Windows.Forms.Button bntConnect;
        private System.Windows.Forms.Button bntDisconnect;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbMemBank;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbStartWord;
        private System.Windows.Forms.CheckBox ckbSetPwd;
        private System.Windows.Forms.TextBox txbPwd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button bntRead;
        private System.Windows.Forms.TextBox txbData;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbWordCnt;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox ckbUser;
        private System.Windows.Forms.ComboBox cmbKillPwd;
        private System.Windows.Forms.CheckBox ckbKillPwd;
        private System.Windows.Forms.ComboBox cmbAccessPwd;
        private System.Windows.Forms.CheckBox ckbAccessPwd;
        private System.Windows.Forms.ComboBox cmbEPC;
        private System.Windows.Forms.CheckBox ckbEPC;
        private System.Windows.Forms.ComboBox cmbTID;
        private System.Windows.Forms.CheckBox ckbTID;
        private System.Windows.Forms.ComboBox cmbUser;
        private System.Windows.Forms.Button bntLock;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox ckbRecomLSB3;
        private System.Windows.Forms.CheckBox ckbRecomLSB2;
        private System.Windows.Forms.CheckBox ckbRecomLSB;
        private System.Windows.Forms.TextBox txbKillPwd;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.ComboBox cmbEPCLen;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label10;
    }
}

