namespace RS485Sample_cs
{
    partial class MainFrm
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbDevType = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbFrame = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbBaud = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbComName = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkedListNode = new System.Windows.Forms.CheckedListBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.GrouBoxOpen = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.listViewInventory = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.GrouBoxOpen.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(69, 45);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Device type:";
            // 
            // cmbDevType
            // 
            this.cmbDevType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDevType.FormattingEnabled = true;
            this.cmbDevType.Location = new System.Drawing.Point(194, 40);
            this.cmbDevType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbDevType.Name = "cmbDevType";
            this.cmbDevType.Size = new System.Drawing.Size(138, 26);
            this.cmbDevType.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbFrame);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cmbBaud);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmbComName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(72, 105);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(300, 150);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "COM Parameters";
            // 
            // cmbFrame
            // 
            this.cmbFrame.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFrame.FormattingEnabled = true;
            this.cmbFrame.Items.AddRange(new object[] {
            "8E1",
            "8N1",
            "8O1"});
            this.cmbFrame.Location = new System.Drawing.Point(124, 108);
            this.cmbFrame.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbFrame.Name = "cmbFrame";
            this.cmbFrame.Size = new System.Drawing.Size(138, 26);
            this.cmbFrame.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 112);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 18);
            this.label4.TabIndex = 0;
            this.label4.Text = "COM name:";
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
            this.cmbBaud.Location = new System.Drawing.Point(124, 69);
            this.cmbBaud.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbBaud.Name = "cmbBaud";
            this.cmbBaud.Size = new System.Drawing.Size(138, 26);
            this.cmbBaud.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 74);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "Baud:";
            // 
            // cmbComName
            // 
            this.cmbComName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbComName.FormattingEnabled = true;
            this.cmbComName.Location = new System.Drawing.Point(124, 30);
            this.cmbComName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmbComName.Name = "cmbComName";
            this.cmbComName.Size = new System.Drawing.Size(138, 26);
            this.cmbComName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 34);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 18);
            this.label2.TabIndex = 0;
            this.label2.Text = "COM name:";
            // 
            // checkedListNode
            // 
            this.checkedListNode.FormattingEnabled = true;
            this.checkedListNode.Items.AddRange(new object[] {
            "RS485 Node#1",
            "RS485 Node#2",
            "RS485 Node#3",
            "RS485 Node#4",
            "RS485 Node#5",
            "RS485 Node#4",
            "RS485 Node#7",
            "RS485 Node#8",
            "RS485 Node#9",
            "RS485 Node#10"});
            this.checkedListNode.Location = new System.Drawing.Point(428, 30);
            this.checkedListNode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkedListNode.Name = "checkedListNode";
            this.checkedListNode.Size = new System.Drawing.Size(228, 229);
            this.checkedListNode.TabIndex = 3;
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(710, 74);
            this.btnOpen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(158, 57);
            this.btnOpen.TabIndex = 4;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(710, 159);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(158, 57);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // GrouBoxOpen
            // 
            this.GrouBoxOpen.Controls.Add(this.label1);
            this.GrouBoxOpen.Controls.Add(this.btnOpen);
            this.GrouBoxOpen.Controls.Add(this.btnClose);
            this.GrouBoxOpen.Controls.Add(this.cmbDevType);
            this.GrouBoxOpen.Controls.Add(this.groupBox1);
            this.GrouBoxOpen.Controls.Add(this.checkedListNode);
            this.GrouBoxOpen.Location = new System.Drawing.Point(18, 18);
            this.GrouBoxOpen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GrouBoxOpen.Name = "GrouBoxOpen";
            this.GrouBoxOpen.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GrouBoxOpen.Size = new System.Drawing.Size(908, 303);
            this.GrouBoxOpen.TabIndex = 5;
            this.GrouBoxOpen.TabStop = false;
            this.GrouBoxOpen.Text = "Open Device";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnStop);
            this.groupBox2.Controls.Add(this.btnStart);
            this.groupBox2.Controls.Add(this.listViewInventory);
            this.groupBox2.Location = new System.Drawing.Point(18, 344);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(908, 363);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Inventory";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(714, 168);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(148, 51);
            this.btnStop.TabIndex = 9;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(714, 62);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(152, 56);
            this.btnStart.TabIndex = 8;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // listViewInventory
            // 
            this.listViewInventory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader4,
            this.columnHeader1,
            this.columnHeader5});
            this.listViewInventory.GridLines = true;
            this.listViewInventory.HideSelection = false;
            this.listViewInventory.Location = new System.Drawing.Point(24, 30);
            this.listViewInventory.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listViewInventory.Name = "listViewInventory";
            this.listViewInventory.Size = new System.Drawing.Size(649, 318);
            this.listViewInventory.TabIndex = 7;
            this.listViewInventory.UseCompatibleStateImageBehavior = false;
            this.listViewInventory.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Tag type";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Serial num";
            this.columnHeader4.Width = 120;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Read Cnt";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Antenna No";
            this.columnHeader5.Width = 80;
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 724);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.GrouBoxOpen);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MainFrm";
            this.Text = "RS485Sample_CS V2.0";
            this.Load += new System.EventHandler(this.MainFrm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.GrouBoxOpen.ResumeLayout(false);
            this.GrouBoxOpen.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbDevType;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbComName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbFrame;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbBaud;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckedListBox checkedListNode;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox GrouBoxOpen;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView listViewInventory;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
    }
}

