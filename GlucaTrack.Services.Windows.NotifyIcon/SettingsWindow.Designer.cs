namespace GlucaTrack.Services.Windows
{
    partial class formSettings
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formSettings));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuNotifyIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItem_Version = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.label1 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkAutoUpload = new System.Windows.Forms.CheckBox();
            this.tStartService = new System.Windows.Forms.Timer(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuItem_Settings = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_Website = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNotifyIcon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.menuNotifyIcon;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "GlucaTrack Detector";
            this.notifyIcon1.Visible = true;
            // 
            // menuNotifyIcon
            // 
            this.menuNotifyIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_Version,
            this.toolStripSeparator1,
            this.menuItem_Settings,
            this.menuItem_Website,
            this.toolStripSeparator2,
            this.menuItem_Exit});
            this.menuNotifyIcon.Name = "menuNotifyIcon";
            this.menuNotifyIcon.Size = new System.Drawing.Size(183, 104);
            // 
            // menuItem_Version
            // 
            this.menuItem_Version.Enabled = false;
            this.menuItem_Version.Name = "menuItem_Version";
            this.menuItem_Version.Size = new System.Drawing.Size(182, 22);
            this.menuItem_Version.Text = "[Service Not Started]";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(179, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(179, 6);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Username";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(73, 54);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(100, 20);
            this.txtUsername.TabIndex = 2;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(73, 76);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(100, 20);
            this.txtPassword.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Password";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(98, 128);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(17, 128);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkAutoUpload
            // 
            this.chkAutoUpload.AutoSize = true;
            this.chkAutoUpload.Location = new System.Drawing.Point(73, 105);
            this.chkAutoUpload.Name = "chkAutoUpload";
            this.chkAutoUpload.Size = new System.Drawing.Size(112, 17);
            this.chkAutoUpload.TabIndex = 8;
            this.chkAutoUpload.Text = "Ask before upload";
            this.chkAutoUpload.UseVisualStyleBackColor = true;
            // 
            // tStartService
            // 
            this.tStartService.Interval = 60000;
            this.tStartService.Tick += new System.EventHandler(this.tStartService_Tick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::GlucaTrack_ServiceNotifyIcon.Properties.Resources.titleidea4;
            this.pictureBox1.Location = new System.Drawing.Point(37, 7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(136, 37);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // menuItem_Settings
            // 
            this.menuItem_Settings.Image = global::GlucaTrack_ServiceNotifyIcon.Properties.Resources.Gear_icon;
            this.menuItem_Settings.Name = "menuItem_Settings";
            this.menuItem_Settings.Size = new System.Drawing.Size(182, 22);
            this.menuItem_Settings.Text = "Settings";
            this.menuItem_Settings.Click += new System.EventHandler(this.menuItem_Settings_Click);
            // 
            // menuItem_Website
            // 
            this.menuItem_Website.Image = global::GlucaTrack_ServiceNotifyIcon.Properties.Resources.Web2_icon;
            this.menuItem_Website.Name = "menuItem_Website";
            this.menuItem_Website.Size = new System.Drawing.Size(182, 22);
            this.menuItem_Website.Text = "Website";
            this.menuItem_Website.Click += new System.EventHandler(this.menuItem_Website_Click);
            // 
            // menuItem_Exit
            // 
            this.menuItem_Exit.Image = global::GlucaTrack_ServiceNotifyIcon.Properties.Resources.Exit_icon;
            this.menuItem_Exit.ImageTransparentColor = System.Drawing.Color.White;
            this.menuItem_Exit.Name = "menuItem_Exit";
            this.menuItem_Exit.Size = new System.Drawing.Size(182, 22);
            this.menuItem_Exit.Text = "Exit";
            this.menuItem_Exit.Click += new System.EventHandler(this.menuItem_Exit_Click);
            // 
            // formSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(184, 154);
            this.ControlBox = false;
            this.Controls.Add(this.chkAutoUpload);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "formSettings";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "GlucaTrack Settings";
            this.TopMost = true;
            this.menuNotifyIcon.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip menuNotifyIcon;
        private System.Windows.Forms.ToolStripMenuItem menuItem_Version;
        private System.Windows.Forms.ToolStripMenuItem menuItem_Settings;
        private System.Windows.Forms.ToolStripMenuItem menuItem_Website;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.CheckBox chkAutoUpload;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuItem_Exit;
        private System.Windows.Forms.Timer tStartService;
    }
}

