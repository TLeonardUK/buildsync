namespace BuildSync.Client.Controls.Setup
{
    partial class ServerSetupPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ServerHostnameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.savePathBrowseButton = new System.Windows.Forms.Button();
            this.ServerHostnameIcon = new System.Windows.Forms.PictureBox();
            this.connectionLabelStatus = new System.Windows.Forms.Label();
            this.ConnectTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ServerHostnameIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // ServerHostnameTextBox
            // 
            this.ServerHostnameTextBox.Location = new System.Drawing.Point(61, 90);
            this.ServerHostnameTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.ServerHostnameTextBox.Name = "ServerHostnameTextBox";
            this.ServerHostnameTextBox.Size = new System.Drawing.Size(273, 20);
            this.ServerHostnameTextBox.TabIndex = 11;
            this.ServerHostnameTextBox.TextChanged += new System.EventHandler(this.StateChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(621, 57);
            this.label1.TabIndex = 10;
            this.label1.Text = "Please enter the hostname or IP address of the build sync coordination server you" +
    " wish to use.\r\n\r\nIf you are not sure which one to use, ask your build sync admin" +
    "istrator.\r\n\r\n";
            // 
            // savePathBrowseButton
            // 
            this.savePathBrowseButton.Location = new System.Drawing.Point(338, 86);
            this.savePathBrowseButton.Margin = new System.Windows.Forms.Padding(2);
            this.savePathBrowseButton.Name = "savePathBrowseButton";
            this.savePathBrowseButton.Size = new System.Drawing.Size(296, 27);
            this.savePathBrowseButton.TabIndex = 12;
            this.savePathBrowseButton.Text = "Unsure of address? Search local network to find it.";
            this.savePathBrowseButton.UseVisualStyleBackColor = true;
            this.savePathBrowseButton.Click += new System.EventHandler(this.FindServerClicked);
            // 
            // ServerHostnameIcon
            // 
            this.ServerHostnameIcon.Image = global::BuildSync.Client.Properties.Resources.ic_check_circle_2x;
            this.ServerHostnameIcon.Location = new System.Drawing.Point(19, 84);
            this.ServerHostnameIcon.Margin = new System.Windows.Forms.Padding(2);
            this.ServerHostnameIcon.Name = "ServerHostnameIcon";
            this.ServerHostnameIcon.Size = new System.Drawing.Size(32, 31);
            this.ServerHostnameIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ServerHostnameIcon.TabIndex = 13;
            this.ServerHostnameIcon.TabStop = false;
            // 
            // connectionLabelStatus
            // 
            this.connectionLabelStatus.Location = new System.Drawing.Point(58, 121);
            this.connectionLabelStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.connectionLabelStatus.Name = "connectionLabelStatus";
            this.connectionLabelStatus.Size = new System.Drawing.Size(576, 31);
            this.connectionLabelStatus.TabIndex = 14;
            this.connectionLabelStatus.Text = "Connecting to server ...";
            this.connectionLabelStatus.Visible = false;
            // 
            // ConnectTimer
            // 
            this.ConnectTimer.Enabled = true;
            this.ConnectTimer.Tick += new System.EventHandler(this.ConnectTimerTick);
            // 
            // ServerSetupPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.connectionLabelStatus);
            this.Controls.Add(this.ServerHostnameIcon);
            this.Controls.Add(this.savePathBrowseButton);
            this.Controls.Add(this.ServerHostnameTextBox);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ServerSetupPage";
            this.Size = new System.Drawing.Size(656, 205);
            ((System.ComponentModel.ISupportInitialize)(this.ServerHostnameIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox ServerHostnameIcon;
        private System.Windows.Forms.TextBox ServerHostnameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button savePathBrowseButton;
        private System.Windows.Forms.Label connectionLabelStatus;
        private System.Windows.Forms.Timer ConnectTimer;
    }
}
