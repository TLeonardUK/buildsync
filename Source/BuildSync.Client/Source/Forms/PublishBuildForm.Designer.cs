namespace BuildSync.Client.Forms
{
    partial class PublishBuildForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.PublishButton = new System.Windows.Forms.Button();
            this.VirtualPathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LocalFolderTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.LocalFolderBrowseButton = new System.Windows.Forms.Button();
            this.PublishProgressBar = new System.Windows.Forms.ProgressBar();
            this.PublishProgressLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ProgressTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(9, 145);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(550, 1);
            this.panel1.TabIndex = 16;
            // 
            // PublishButton
            // 
            this.PublishButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.PublishButton.Enabled = false;
            this.PublishButton.Location = new System.Drawing.Point(463, 157);
            this.PublishButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PublishButton.Name = "PublishButton";
            this.PublishButton.Size = new System.Drawing.Size(95, 29);
            this.PublishButton.TabIndex = 15;
            this.PublishButton.Text = "Publish Build";
            this.PublishButton.UseVisualStyleBackColor = true;
            this.PublishButton.Click += new System.EventHandler(this.PublishClicked);
            // 
            // VirtualPathTextBox
            // 
            this.VirtualPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VirtualPathTextBox.Location = new System.Drawing.Point(11, 70);
            this.VirtualPathTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.VirtualPathTextBox.Name = "VirtualPathTextBox";
            this.VirtualPathTextBox.Size = new System.Drawing.Size(541, 20);
            this.VirtualPathTextBox.TabIndex = 20;
            this.VirtualPathTextBox.TextChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 55);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Path";
            // 
            // LocalFolderTextBox
            // 
            this.LocalFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LocalFolderTextBox.Location = new System.Drawing.Point(11, 109);
            this.LocalFolderTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.LocalFolderTextBox.Name = "LocalFolderTextBox";
            this.LocalFolderTextBox.ReadOnly = true;
            this.LocalFolderTextBox.Size = new System.Drawing.Size(449, 20);
            this.LocalFolderTextBox.TabIndex = 22;
            this.LocalFolderTextBox.TextChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 94);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Local Folder";
            // 
            // LocalFolderBrowseButton
            // 
            this.LocalFolderBrowseButton.Location = new System.Drawing.Point(463, 109);
            this.LocalFolderBrowseButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.LocalFolderBrowseButton.Name = "LocalFolderBrowseButton";
            this.LocalFolderBrowseButton.Size = new System.Drawing.Size(87, 20);
            this.LocalFolderBrowseButton.TabIndex = 23;
            this.LocalFolderBrowseButton.Text = "Browse";
            this.LocalFolderBrowseButton.UseVisualStyleBackColor = true;
            this.LocalFolderBrowseButton.Click += new System.EventHandler(this.BrowseForLocalFolderClicked);
            // 
            // PublishProgressBar
            // 
            this.PublishProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PublishProgressBar.Location = new System.Drawing.Point(11, 172);
            this.PublishProgressBar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PublishProgressBar.Name = "PublishProgressBar";
            this.PublishProgressBar.Size = new System.Drawing.Size(441, 13);
            this.PublishProgressBar.TabIndex = 24;
            this.PublishProgressBar.Visible = false;
            // 
            // PublishProgressLabel
            // 
            this.PublishProgressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PublishProgressLabel.AutoSize = true;
            this.PublishProgressLabel.Location = new System.Drawing.Point(9, 154);
            this.PublishProgressLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.PublishProgressLabel.Name = "PublishProgressLabel";
            this.PublishProgressLabel.Size = new System.Drawing.Size(156, 13);
            this.PublishProgressLabel.TabIndex = 25;
            this.PublishProgressLabel.Text = "Publishing manifest on server ...";
            this.PublishProgressLabel.Visible = false;
            // 
            // label4
            // 
            this.label4.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label4.Location = new System.Drawing.Point(9, 10);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(541, 45);
            this.label4.TabIndex = 26;
            this.label4.Text = "This will create a build manifest out of the contents of a given folder and publi" +
    "sh it to the server at the given path. This process may take a while.\r\n";
            // 
            // ProgressTimer
            // 
            this.ProgressTimer.Enabled = true;
            this.ProgressTimer.Tick += new System.EventHandler(this.ProgressTimerTick);
            // 
            // PublishBuildForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(566, 197);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.PublishProgressLabel);
            this.Controls.Add(this.PublishProgressBar);
            this.Controls.Add(this.LocalFolderBrowseButton);
            this.Controls.Add(this.LocalFolderTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.VirtualPathTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PublishButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "PublishBuildForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Publish Build";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAboutToClose);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button PublishButton;
        private System.Windows.Forms.TextBox VirtualPathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox LocalFolderTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button LocalFolderBrowseButton;
        private System.Windows.Forms.ProgressBar PublishProgressBar;
        private System.Windows.Forms.Label PublishProgressLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Timer ProgressTimer;
    }
}