namespace RPANSample
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.comboBoxCOM = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.checkedListBoxAntennaList = new System.Windows.Forms.CheckedListBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxBlockNum = new System.Windows.Forms.ComboBox();
            this.start_block = new System.Windows.Forms.ComboBox();
            this.dataGridViewRecord = new System.Windows.Forms.DataGridView();
            this.uid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonStartRecord = new System.Windows.Forms.Button();
            this.tabControlInvRead = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelTagCnt = new System.Windows.Forms.Label();
            this.comboBoxMode = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonStopRecord = new System.Windows.Forms.Button();
            this.tabPageMultipleTagsWrite = new System.Windows.Forms.TabPage();
            this.buttonWrite = new System.Windows.Forms.Button();
            this.textBoxBlockData = new System.Windows.Forms.TextBox();
            this.checkedListBoxUIDs = new System.Windows.Forms.CheckedListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonNormalInven = new System.Windows.Forms.Button();
            this.comboBoxBlockCnt = new System.Windows.Forms.ComboBox();
            this.comboBoxBlockAddress = new System.Windows.Forms.ComboBox();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRecord)).BeginInit();
            this.tabControlInvRead.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPageMultipleTagsWrite.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.comboBoxCOM);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.checkedListBoxAntennaList);
            this.groupBox4.Controls.Add(this.buttonClose);
            this.groupBox4.Controls.Add(this.buttonOpen);
            this.groupBox4.Location = new System.Drawing.Point(18, 18);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Size = new System.Drawing.Size(1126, 138);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Reader";
            // 
            // comboBoxCOM
            // 
            this.comboBoxCOM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCOM.FormattingEnabled = true;
            this.comboBoxCOM.Location = new System.Drawing.Point(120, 56);
            this.comboBoxCOM.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBoxCOM.Name = "comboBoxCOM";
            this.comboBoxCOM.Size = new System.Drawing.Size(152, 26);
            this.comboBoxCOM.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(68, 62);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(44, 18);
            this.label11.TabIndex = 0;
            this.label11.Text = "COM:";
            // 
            // checkedListBoxAntennaList
            // 
            this.checkedListBoxAntennaList.FormattingEnabled = true;
            this.checkedListBoxAntennaList.Location = new System.Drawing.Point(878, 20);
            this.checkedListBoxAntennaList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkedListBoxAntennaList.Name = "checkedListBoxAntennaList";
            this.checkedListBoxAntennaList.Size = new System.Drawing.Size(232, 96);
            this.checkedListBoxAntennaList.TabIndex = 18;
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(488, 50);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(123, 39);
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(332, 50);
            this.buttonOpen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(123, 39);
            this.buttonOpen.TabIndex = 5;
            this.buttonOpen.Text = "Open";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(880, 156);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 18);
            this.label2.TabIndex = 12;
            this.label2.Text = "Num of blocks:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(880, 86);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 18);
            this.label1.TabIndex = 11;
            this.label1.Text = "Start block:";
            // 
            // comboBoxBlockNum
            // 
            this.comboBoxBlockNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBlockNum.FormattingEnabled = true;
            this.comboBoxBlockNum.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8"});
            this.comboBoxBlockNum.Location = new System.Drawing.Point(884, 182);
            this.comboBoxBlockNum.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBoxBlockNum.Name = "comboBoxBlockNum";
            this.comboBoxBlockNum.Size = new System.Drawing.Size(220, 26);
            this.comboBoxBlockNum.TabIndex = 10;
            // 
            // start_block
            // 
            this.start_block.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.start_block.FormattingEnabled = true;
            this.start_block.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27"});
            this.start_block.Location = new System.Drawing.Point(884, 111);
            this.start_block.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.start_block.Name = "start_block";
            this.start_block.Size = new System.Drawing.Size(220, 26);
            this.start_block.TabIndex = 9;
            // 
            // dataGridViewRecord
            // 
            this.dataGridViewRecord.AllowUserToAddRows = false;
            this.dataGridViewRecord.AllowUserToDeleteRows = false;
            this.dataGridViewRecord.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewRecord.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewRecord.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRecord.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.uid,
            this.Column1,
            this.Column2});
            this.dataGridViewRecord.Location = new System.Drawing.Point(4, 9);
            this.dataGridViewRecord.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dataGridViewRecord.Name = "dataGridViewRecord";
            this.dataGridViewRecord.ReadOnly = true;
            this.dataGridViewRecord.RowHeadersVisible = false;
            this.dataGridViewRecord.RowTemplate.Height = 23;
            this.dataGridViewRecord.Size = new System.Drawing.Size(867, 676);
            this.dataGridViewRecord.TabIndex = 12;
            // 
            // uid
            // 
            this.uid.FillWeight = 60F;
            this.uid.HeaderText = "UID";
            this.uid.Name = "uid";
            this.uid.ReadOnly = true;
            // 
            // Column1
            // 
            this.Column1.FillWeight = 159.3909F;
            this.Column1.HeaderText = "Block Data";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.FillWeight = 60F;
            this.Column2.HeaderText = "Count";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // buttonStartRecord
            // 
            this.buttonStartRecord.Location = new System.Drawing.Point(884, 236);
            this.buttonStartRecord.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonStartRecord.Name = "buttonStartRecord";
            this.buttonStartRecord.Size = new System.Drawing.Size(222, 54);
            this.buttonStartRecord.TabIndex = 13;
            this.buttonStartRecord.Text = "Start";
            this.buttonStartRecord.UseVisualStyleBackColor = true;
            this.buttonStartRecord.Click += new System.EventHandler(this.buttonStartRecord_Click);
            // 
            // tabControlInvRead
            // 
            this.tabControlInvRead.Controls.Add(this.tabPage1);
            this.tabControlInvRead.Controls.Add(this.tabPageMultipleTagsWrite);
            this.tabControlInvRead.Location = new System.Drawing.Point(18, 188);
            this.tabControlInvRead.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControlInvRead.Name = "tabControlInvRead";
            this.tabControlInvRead.SelectedIndex = 0;
            this.tabControlInvRead.Size = new System.Drawing.Size(1126, 686);
            this.tabControlInvRead.TabIndex = 15;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.labelTime);
            this.tabPage1.Controls.Add(this.labelTagCnt);
            this.tabPage1.Controls.Add(this.comboBoxMode);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.buttonStopRecord);
            this.tabPage1.Controls.Add(this.buttonStartRecord);
            this.tabPage1.Controls.Add(this.comboBoxBlockNum);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.dataGridViewRecord);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.start_block);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Size = new System.Drawing.Size(1118, 654);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Inventory Read";
            // 
            // labelTime
            // 
            this.labelTime.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelTime.Location = new System.Drawing.Point(880, 483);
            this.labelTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(150, 51);
            this.labelTime.TabIndex = 21;
            this.labelTime.Text = "Time:0";
            this.labelTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelTagCnt
            // 
            this.labelTagCnt.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelTagCnt.Location = new System.Drawing.Point(880, 405);
            this.labelTagCnt.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTagCnt.Name = "labelTagCnt";
            this.labelTagCnt.Size = new System.Drawing.Size(150, 51);
            this.labelTagCnt.TabIndex = 21;
            this.labelTagCnt.Text = "Tag:0";
            this.labelTagCnt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxMode
            // 
            this.comboBoxMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMode.FormattingEnabled = true;
            this.comboBoxMode.Items.AddRange(new object[] {
            "Without Buffer",
            "With Buffer"});
            this.comboBoxMode.Location = new System.Drawing.Point(884, 39);
            this.comboBoxMode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBoxMode.Name = "comboBoxMode";
            this.comboBoxMode.Size = new System.Drawing.Size(220, 26);
            this.comboBoxMode.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(880, 12);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 18);
            this.label4.TabIndex = 19;
            this.label4.Text = "Mode:";
            // 
            // buttonStopRecord
            // 
            this.buttonStopRecord.Location = new System.Drawing.Point(884, 302);
            this.buttonStopRecord.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonStopRecord.Name = "buttonStopRecord";
            this.buttonStopRecord.Size = new System.Drawing.Size(222, 54);
            this.buttonStopRecord.TabIndex = 13;
            this.buttonStopRecord.Text = "Stop";
            this.buttonStopRecord.UseVisualStyleBackColor = true;
            this.buttonStopRecord.Click += new System.EventHandler(this.buttonStopRecord_Click);
            // 
            // tabPageMultipleTagsWrite
            // 
            this.tabPageMultipleTagsWrite.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageMultipleTagsWrite.Controls.Add(this.buttonWrite);
            this.tabPageMultipleTagsWrite.Controls.Add(this.textBoxBlockData);
            this.tabPageMultipleTagsWrite.Controls.Add(this.checkedListBoxUIDs);
            this.tabPageMultipleTagsWrite.Controls.Add(this.label7);
            this.tabPageMultipleTagsWrite.Controls.Add(this.label6);
            this.tabPageMultipleTagsWrite.Controls.Add(this.label5);
            this.tabPageMultipleTagsWrite.Controls.Add(this.buttonNormalInven);
            this.tabPageMultipleTagsWrite.Controls.Add(this.comboBoxBlockCnt);
            this.tabPageMultipleTagsWrite.Controls.Add(this.comboBoxBlockAddress);
            this.tabPageMultipleTagsWrite.Location = new System.Drawing.Point(4, 28);
            this.tabPageMultipleTagsWrite.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageMultipleTagsWrite.Name = "tabPageMultipleTagsWrite";
            this.tabPageMultipleTagsWrite.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageMultipleTagsWrite.Size = new System.Drawing.Size(1118, 654);
            this.tabPageMultipleTagsWrite.TabIndex = 1;
            this.tabPageMultipleTagsWrite.Text = "Write Multiple Tags";
            // 
            // buttonWrite
            // 
            this.buttonWrite.Location = new System.Drawing.Point(172, 362);
            this.buttonWrite.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonWrite.Name = "buttonWrite";
            this.buttonWrite.Size = new System.Drawing.Size(225, 51);
            this.buttonWrite.TabIndex = 5;
            this.buttonWrite.Text = "Write";
            this.buttonWrite.UseVisualStyleBackColor = true;
            this.buttonWrite.Click += new System.EventHandler(this.buttonWrite_Click);
            // 
            // textBoxBlockData
            // 
            this.textBoxBlockData.Location = new System.Drawing.Point(172, 290);
            this.textBoxBlockData.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxBlockData.Name = "textBoxBlockData";
            this.textBoxBlockData.Size = new System.Drawing.Size(530, 28);
            this.textBoxBlockData.TabIndex = 4;
            this.textBoxBlockData.Text = "00000000";
            // 
            // checkedListBoxUIDs
            // 
            this.checkedListBoxUIDs.FormattingEnabled = true;
            this.checkedListBoxUIDs.Location = new System.Drawing.Point(33, 9);
            this.checkedListBoxUIDs.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkedListBoxUIDs.Name = "checkedListBoxUIDs";
            this.checkedListBoxUIDs.Size = new System.Drawing.Size(388, 142);
            this.checkedListBoxUIDs.TabIndex = 18;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 294);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(152, 18);
            this.label7.TabIndex = 3;
            this.label7.Text = "Block Data(hex):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(48, 242);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 18);
            this.label6.TabIndex = 3;
            this.label6.Text = "Block Count:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 186);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(134, 18);
            this.label5.TabIndex = 3;
            this.label5.Text = "Block Address:";
            // 
            // buttonNormalInven
            // 
            this.buttonNormalInven.Location = new System.Drawing.Point(450, 9);
            this.buttonNormalInven.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonNormalInven.Name = "buttonNormalInven";
            this.buttonNormalInven.Size = new System.Drawing.Size(154, 42);
            this.buttonNormalInven.TabIndex = 2;
            this.buttonNormalInven.Text = "Inventory";
            this.buttonNormalInven.UseVisualStyleBackColor = true;
            this.buttonNormalInven.Click += new System.EventHandler(this.buttonNormalInven_Click);
            // 
            // comboBoxBlockCnt
            // 
            this.comboBoxBlockCnt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBlockCnt.FormattingEnabled = true;
            this.comboBoxBlockCnt.Location = new System.Drawing.Point(172, 237);
            this.comboBoxBlockCnt.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBoxBlockCnt.Name = "comboBoxBlockCnt";
            this.comboBoxBlockCnt.Size = new System.Drawing.Size(248, 26);
            this.comboBoxBlockCnt.TabIndex = 1;
            // 
            // comboBoxBlockAddress
            // 
            this.comboBoxBlockAddress.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBlockAddress.FormattingEnabled = true;
            this.comboBoxBlockAddress.Location = new System.Drawing.Point(172, 182);
            this.comboBoxBlockAddress.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBoxBlockAddress.Name = "comboBoxBlockAddress";
            this.comboBoxBlockAddress.Size = new System.Drawing.Size(248, 26);
            this.comboBoxBlockAddress.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1158, 891);
            this.Controls.Add(this.tabControlInvRead);
            this.Controls.Add(this.groupBox4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InventoryReadSample V2.0";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRecord)).EndInit();
            this.tabControlInvRead.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPageMultipleTagsWrite.ResumeLayout(false);
            this.tabPageMultipleTagsWrite.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox comboBoxCOM;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.DataGridView dataGridViewRecord;
        private System.Windows.Forms.Button buttonStartRecord;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxBlockNum;
        private System.Windows.Forms.ComboBox start_block;
        private System.Windows.Forms.CheckedListBox checkedListBoxAntennaList;
        private System.Windows.Forms.DataGridViewTextBoxColumn uid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.TabControl tabControlInvRead;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPageMultipleTagsWrite;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxMode;
        private System.Windows.Forms.Button buttonStopRecord;
        private System.Windows.Forms.Button buttonNormalInven;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxBlockAddress;
        private System.Windows.Forms.ComboBox comboBoxBlockCnt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxBlockData;
        private System.Windows.Forms.Button buttonWrite;
        private System.Windows.Forms.CheckedListBox checkedListBoxUIDs;
        private System.Windows.Forms.Label labelTagCnt;
        private System.Windows.Forms.Label labelTime;
    }
}

