namespace GlucoseMeterReader
{
    partial class GlucoseMeterReader
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GlucoseMeterReader));
            this.textConsole = new System.Windows.Forms.TextBox();
            this.cbComports = new System.Windows.Forms.ComboBox();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.scLeftRight = new System.Windows.Forms.SplitContainer();
            this.txtDevicesDetected = new System.Windows.Forms.TextBox();
            this.btnDetect = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnRead = new System.Windows.Forms.Button();
            this.cbDetectedDevices = new System.Windows.Forms.ComboBox();
            this.btnRocheGetData = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbMeterType = new System.Windows.Forms.ComboBox();
            this.scInnerTopBottom = new System.Windows.Forms.SplitContainer();
            this.scTopBottom = new System.Windows.Forms.SplitContainer();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblNumSamplesOnMeter = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblSerialNumber = new System.Windows.Forms.Label();
            this.statusStrip3 = new System.Windows.Forms.StatusStrip();
            this.lblDebugStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblReadTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblPerSecond = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnContourUSB = new System.Windows.Forms.Button();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scLeftRight)).BeginInit();
            this.scLeftRight.Panel1.SuspendLayout();
            this.scLeftRight.Panel2.SuspendLayout();
            this.scLeftRight.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scInnerTopBottom)).BeginInit();
            this.scInnerTopBottom.Panel1.SuspendLayout();
            this.scInnerTopBottom.Panel2.SuspendLayout();
            this.scInnerTopBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scTopBottom)).BeginInit();
            this.scTopBottom.Panel1.SuspendLayout();
            this.scTopBottom.Panel2.SuspendLayout();
            this.scTopBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.statusStrip3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textConsole
            // 
            this.textConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textConsole.Location = new System.Drawing.Point(0, 0);
            this.textConsole.MaxLength = 1000000;
            this.textConsole.Multiline = true;
            this.textConsole.Name = "textConsole";
            this.textConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textConsole.Size = new System.Drawing.Size(414, 96);
            this.textConsole.TabIndex = 1;
            // 
            // cbComports
            // 
            this.cbComports.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbComports.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbComports.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbComports.FormattingEnabled = true;
            this.cbComports.Location = new System.Drawing.Point(14, 84);
            this.cbComports.Name = "cbComports";
            this.cbComports.Size = new System.Drawing.Size(161, 24);
            this.cbComports.TabIndex = 2;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.scLeftRight);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(614, 494);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(614, 518);
            this.toolStripContainer1.TabIndex = 3;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.menuStrip1);
            // 
            // scLeftRight
            // 
            this.scLeftRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.scLeftRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scLeftRight.IsSplitterFixed = true;
            this.scLeftRight.Location = new System.Drawing.Point(0, 0);
            this.scLeftRight.Name = "scLeftRight";
            // 
            // scLeftRight.Panel1
            // 
            this.scLeftRight.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(224)))), ((int)(((byte)(255)))));
            this.scLeftRight.Panel1.Controls.Add(this.btnContourUSB);
            this.scLeftRight.Panel1.Controls.Add(this.txtDevicesDetected);
            this.scLeftRight.Panel1.Controls.Add(this.btnDetect);
            this.scLeftRight.Panel1.Controls.Add(this.statusStrip1);
            this.scLeftRight.Panel1.Controls.Add(this.btnRead);
            this.scLeftRight.Panel1.Controls.Add(this.cbDetectedDevices);
            this.scLeftRight.Panel1.Controls.Add(this.btnRocheGetData);
            this.scLeftRight.Panel1.Controls.Add(this.label4);
            this.scLeftRight.Panel1.Controls.Add(this.label3);
            this.scLeftRight.Panel1.Controls.Add(this.cbMeterType);
            this.scLeftRight.Panel1.Controls.Add(this.cbComports);
            // 
            // scLeftRight.Panel2
            // 
            this.scLeftRight.Panel2.Controls.Add(this.scInnerTopBottom);
            this.scLeftRight.Size = new System.Drawing.Size(614, 494);
            this.scLeftRight.SplitterDistance = 195;
            this.scLeftRight.SplitterWidth = 1;
            this.scLeftRight.TabIndex = 0;
            // 
            // txtDevicesDetected
            // 
            this.txtDevicesDetected.Location = new System.Drawing.Point(3, 274);
            this.txtDevicesDetected.Multiline = true;
            this.txtDevicesDetected.Name = "txtDevicesDetected";
            this.txtDevicesDetected.Size = new System.Drawing.Size(187, 49);
            this.txtDevicesDetected.TabIndex = 6;
            // 
            // btnDetect
            // 
            this.btnDetect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDetect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDetect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(131)))), ((int)(((byte)(215)))));
            this.btnDetect.Location = new System.Drawing.Point(39, 224);
            this.btnDetect.Name = "btnDetect";
            this.btnDetect.Size = new System.Drawing.Size(114, 44);
            this.btnDetect.TabIndex = 13;
            this.btnDetect.Text = "Detect Device";
            this.btnDetect.UseVisualStyleBackColor = true;
            this.btnDetect.Click += new System.EventHandler(this.btnDetect_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 470);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(193, 22);
            this.statusStrip1.TabIndex = 12;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.SystemColors.Control;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // btnRead
            // 
            this.btnRead.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRead.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRead.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(131)))), ((int)(((byte)(215)))));
            this.btnRead.Location = new System.Drawing.Point(39, 140);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(114, 44);
            this.btnRead.TabIndex = 9;
            this.btnRead.Text = "Start Read";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // cbDetectedDevices
            // 
            this.cbDetectedDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDetectedDevices.FormattingEnabled = true;
            this.cbDetectedDevices.Location = new System.Drawing.Point(25, 344);
            this.cbDetectedDevices.Name = "cbDetectedDevices";
            this.cbDetectedDevices.Size = new System.Drawing.Size(121, 21);
            this.cbDetectedDevices.TabIndex = 8;
            // 
            // btnRocheGetData
            // 
            this.btnRocheGetData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRocheGetData.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(131)))), ((int)(((byte)(215)))));
            this.btnRocheGetData.Location = new System.Drawing.Point(11, 371);
            this.btnRocheGetData.Name = "btnRocheGetData";
            this.btnRocheGetData.Size = new System.Drawing.Size(99, 44);
            this.btnRocheGetData.TabIndex = 7;
            this.btnRocheGetData.Text = "Roche Get Data";
            this.btnRocheGetData.UseVisualStyleBackColor = true;
            this.btnRocheGetData.Click += new System.EventHandler(this.btnRocheGetData_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(131)))), ((int)(((byte)(215)))));
            this.label4.Location = new System.Drawing.Point(12, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "COM Port:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(131)))), ((int)(((byte)(215)))));
            this.label3.Location = new System.Drawing.Point(12, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Meter Type:";
            // 
            // cbMeterType
            // 
            this.cbMeterType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMeterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMeterType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbMeterType.FormattingEnabled = true;
            this.cbMeterType.Location = new System.Drawing.Point(14, 30);
            this.cbMeterType.Name = "cbMeterType";
            this.cbMeterType.Size = new System.Drawing.Size(161, 24);
            this.cbMeterType.TabIndex = 3;
            // 
            // scInnerTopBottom
            // 
            this.scInnerTopBottom.BackColor = System.Drawing.Color.White;
            this.scInnerTopBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scInnerTopBottom.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.scInnerTopBottom.IsSplitterFixed = true;
            this.scInnerTopBottom.Location = new System.Drawing.Point(0, 0);
            this.scInnerTopBottom.Name = "scInnerTopBottom";
            this.scInnerTopBottom.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scInnerTopBottom.Panel1
            // 
            this.scInnerTopBottom.Panel1.Controls.Add(this.scTopBottom);
            // 
            // scInnerTopBottom.Panel2
            // 
            this.scInnerTopBottom.Panel2.Controls.Add(this.statusStrip3);
            this.scInnerTopBottom.Panel2MinSize = 22;
            this.scInnerTopBottom.Size = new System.Drawing.Size(416, 492);
            this.scInnerTopBottom.SplitterDistance = 466;
            this.scInnerTopBottom.SplitterWidth = 1;
            this.scInnerTopBottom.TabIndex = 7;
            // 
            // scTopBottom
            // 
            this.scTopBottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.scTopBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scTopBottom.Location = new System.Drawing.Point(0, 0);
            this.scTopBottom.Name = "scTopBottom";
            this.scTopBottom.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scTopBottom.Panel1
            // 
            this.scTopBottom.Panel1.Controls.Add(this.dgvData);
            this.scTopBottom.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // scTopBottom.Panel2
            // 
            this.scTopBottom.Panel2.Controls.Add(this.textConsole);
            this.scTopBottom.Panel2MinSize = 0;
            this.scTopBottom.Size = new System.Drawing.Size(416, 466);
            this.scTopBottom.SplitterDistance = 366;
            this.scTopBottom.SplitterWidth = 2;
            this.scTopBottom.TabIndex = 2;
            // 
            // dgvData
            // 
            this.dgvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvData.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(224)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvData.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvData.Location = new System.Drawing.Point(2, 38);
            this.dgvData.Name = "dgvData";
            this.dgvData.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvData.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvData.RowHeadersVisible = false;
            this.dgvData.Size = new System.Drawing.Size(408, 323);
            this.dgvData.TabIndex = 5;
            this.dgvData.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblNumSamplesOnMeter, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblSerialNumber, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(330, 32);
            this.tableLayoutPanel1.TabIndex = 4;
            this.tableLayoutPanel1.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Serial Number:";
            // 
            // lblNumSamplesOnMeter
            // 
            this.lblNumSamplesOnMeter.AutoSize = true;
            this.lblNumSamplesOnMeter.Location = new System.Drawing.Point(156, 16);
            this.lblNumSamplesOnMeter.Name = "lblNumSamplesOnMeter";
            this.lblNumSamplesOnMeter.Size = new System.Drawing.Size(13, 13);
            this.lblNumSamplesOnMeter.TabIndex = 3;
            this.lblNumSamplesOnMeter.Text = "?";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(147, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Number of Samples on Meter:";
            // 
            // lblSerialNumber
            // 
            this.lblSerialNumber.AutoSize = true;
            this.lblSerialNumber.Location = new System.Drawing.Point(156, 0);
            this.lblSerialNumber.Name = "lblSerialNumber";
            this.lblSerialNumber.Size = new System.Drawing.Size(13, 13);
            this.lblSerialNumber.TabIndex = 1;
            this.lblSerialNumber.Text = "?";
            // 
            // statusStrip3
            // 
            this.statusStrip3.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblDebugStatus,
            this.lblReadTime,
            this.lblPerSecond});
            this.statusStrip3.Location = new System.Drawing.Point(0, 3);
            this.statusStrip3.Name = "statusStrip3";
            this.statusStrip3.Size = new System.Drawing.Size(416, 22);
            this.statusStrip3.TabIndex = 6;
            this.statusStrip3.Text = "statusStrip3";
            // 
            // lblDebugStatus
            // 
            this.lblDebugStatus.Name = "lblDebugStatus";
            this.lblDebugStatus.Size = new System.Drawing.Size(276, 17);
            this.lblDebugStatus.Spring = true;
            // 
            // lblReadTime
            // 
            this.lblReadTime.AutoSize = false;
            this.lblReadTime.Name = "lblReadTime";
            this.lblReadTime.Size = new System.Drawing.Size(125, 17);
            // 
            // lblPerSecond
            // 
            this.lblPerSecond.Name = "lblPerSecond";
            this.lblPerSecond.Size = new System.Drawing.Size(0, 17);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(194)))), ((int)(((byte)(224)))), ((int)(((byte)(255)))));
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(614, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // btnContourUSB
            // 
            this.btnContourUSB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnContourUSB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(131)))), ((int)(((byte)(215)))));
            this.btnContourUSB.Location = new System.Drawing.Point(11, 421);
            this.btnContourUSB.Name = "btnContourUSB";
            this.btnContourUSB.Size = new System.Drawing.Size(99, 44);
            this.btnContourUSB.TabIndex = 14;
            this.btnContourUSB.Text = "Contour USB Get Data";
            this.btnContourUSB.UseVisualStyleBackColor = true;
            this.btnContourUSB.Click += new System.EventHandler(this.btnContourUSB_Click);
            // 
            // GlucoseMeterReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 518);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "GlucoseMeterReader";
            this.Text = "Glucose Meter Reader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GlucoseMeterReader_Closing);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.scLeftRight.Panel1.ResumeLayout(false);
            this.scLeftRight.Panel1.PerformLayout();
            this.scLeftRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scLeftRight)).EndInit();
            this.scLeftRight.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.scInnerTopBottom.Panel1.ResumeLayout(false);
            this.scInnerTopBottom.Panel2.ResumeLayout(false);
            this.scInnerTopBottom.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scInnerTopBottom)).EndInit();
            this.scInnerTopBottom.ResumeLayout(false);
            this.scTopBottom.Panel1.ResumeLayout(false);
            this.scTopBottom.Panel2.ResumeLayout(false);
            this.scTopBottom.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scTopBottom)).EndInit();
            this.scTopBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.statusStrip3.ResumeLayout(false);
            this.statusStrip3.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textConsole;
        private System.Windows.Forms.ComboBox cbComports;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.SplitContainer scLeftRight;
        private System.Windows.Forms.SplitContainer scTopBottom;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblNumSamplesOnMeter;
        private System.Windows.Forms.Label lblSerialNumber;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.ComboBox cbMeterType;
        private System.Windows.Forms.SplitContainer scInnerTopBottom;
        private System.Windows.Forms.StatusStrip statusStrip3;
        private System.Windows.Forms.ToolStripStatusLabel lblDebugStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblReadTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRocheGetData;
        private System.Windows.Forms.ComboBox cbDetectedDevices;
        private System.Windows.Forms.ToolStripStatusLabel lblPerSecond;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.TextBox txtDevicesDetected;
        private System.Windows.Forms.Button btnDetect;
        private System.Windows.Forms.Button btnContourUSB;
    }
}

