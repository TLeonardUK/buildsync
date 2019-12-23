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
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
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
            // NameTextBox
            // 
            this.NameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NameTextBox.Location = new System.Drawing.Point(16, 109);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(809, 26);
            this.NameTextBox.TabIndex = 18;
            this.NameTextBox.TextChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 20);
            this.label2.TabIndex = 17;
            this.label2.Text = "Name";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(13, 304);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(825, 1);
            this.panel1.TabIndex = 16;
            // 
            // PublishButton
            // 
            this.PublishButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.PublishButton.Enabled = false;
            this.PublishButton.Location = new System.Drawing.Point(695, 320);
            this.PublishButton.Name = "PublishButton";
            this.PublishButton.Size = new System.Drawing.Size(143, 45);
            this.PublishButton.TabIndex = 15;
            this.PublishButton.Text = "Publish Build";
            this.PublishButton.UseVisualStyleBackColor = true;
            this.PublishButton.Click += new System.EventHandler(this.PublishClicked);
            // 
            // VirtualPathTextBox
            // 
            this.VirtualPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.VirtualPathTextBox.Location = new System.Drawing.Point(16, 172);
            this.VirtualPathTextBox.Name = "VirtualPathTextBox";
            this.VirtualPathTextBox.Size = new System.Drawing.Size(809, 26);
            this.VirtualPathTextBox.TabIndex = 20;
            this.VirtualPathTextBox.TextChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 149);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 20);
            this.label1.TabIndex = 19;
            this.label1.Text = "Virtual Path";
            // 
            // LocalFolderTextBox
            // 
            this.LocalFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LocalFolderTextBox.Location = new System.Drawing.Point(16, 233);
            this.LocalFolderTextBox.Name = "LocalFolderTextBox";
            this.LocalFolderTextBox.ReadOnly = true;
            this.LocalFolderTextBox.Size = new System.Drawing.Size(672, 26);
            this.LocalFolderTextBox.TabIndex = 22;
            this.LocalFolderTextBox.TextChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 210);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 20);
            this.label3.TabIndex = 21;
            this.label3.Text = "Local Folder";
            // 
            // LocalFolderBrowseButton
            // 
            this.LocalFolderBrowseButton.Location = new System.Drawing.Point(695, 233);
            this.LocalFolderBrowseButton.Name = "LocalFolderBrowseButton";
            this.LocalFolderBrowseButton.Size = new System.Drawing.Size(130, 26);
            this.LocalFolderBrowseButton.TabIndex = 23;
            this.LocalFolderBrowseButton.Text = "Browse";
            this.LocalFolderBrowseButton.UseVisualStyleBackColor = true;
            this.LocalFolderBrowseButton.Click += new System.EventHandler(this.BrowseForLocalFolderClicked);
            // 
            // PublishProgressBar
            // 
            this.PublishProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PublishProgressBar.Location = new System.Drawing.Point(16, 345);
            this.PublishProgressBar.Name = "PublishProgressBar";
            this.PublishProgressBar.Size = new System.Drawing.Size(661, 20);
            this.PublishProgressBar.TabIndex = 24;
            this.PublishProgressBar.Visible = false;
            // 
            // PublishProgressLabel
            // 
            this.PublishProgressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PublishProgressLabel.AutoSize = true;
            this.PublishProgressLabel.Location = new System.Drawing.Point(14, 318);
            this.PublishProgressLabel.Name = "PublishProgressLabel";
            this.PublishProgressLabel.Size = new System.Drawing.Size(231, 20);
            this.PublishProgressLabel.TabIndex = 25;
            this.PublishProgressLabel.Text = "Publishing manifest on server ...";
            this.PublishProgressLabel.Visible = false;
            // 
            // label4
            // 
            this.label4.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label4.Location = new System.Drawing.Point(13, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(812, 69);
            this.label4.TabIndex = 26;
            this.label4.Text = "This will create a build manifest out of the contents of a given folder and publi" +
    "sh it to the server at the given virtual path. This process may take a while.\r\n";
            // 
            // ProgressTimer
            // 
            this.ProgressTimer.Enabled = true;
            this.ProgressTimer.Tick += new System.EventHandler(this.ProgressTimerTick);
            // 
            // PublishBuildForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(849, 379);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.PublishProgressLabel);
            this.Controls.Add(this.PublishProgressBar);
            this.Controls.Add(this.LocalFolderBrowseButton);
            this.Controls.Add(this.LocalFolderTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.VirtualPathTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.PublishButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
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

        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.Label label2;
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