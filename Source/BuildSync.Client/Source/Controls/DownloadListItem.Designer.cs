namespace BuildSync.Client.Controls
{
    partial class DownloadListItem
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
            this.MainPanel = new System.Windows.Forms.Panel();
            this.NameLabel = new System.Windows.Forms.Label();
            this.PlayButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.DownloadSpeedLabel = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.UploadSpeedLabel = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.BuildLabel = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.EtaLabel = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.RemoveButton = new System.Windows.Forms.Button();
            this.SettingsButton = new System.Windows.Forms.Button();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.MainPanel.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainPanel
            // 
            this.MainPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.MainPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.MainPanel.Controls.Add(this.NameLabel);
            this.MainPanel.Controls.Add(this.PlayButton);
            this.MainPanel.Controls.Add(this.flowLayoutPanel1);
            this.MainPanel.Controls.Add(this.RemoveButton);
            this.MainPanel.Controls.Add(this.SettingsButton);
            this.MainPanel.Controls.Add(this.ProgressBar);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(6, 6);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(903, 71);
            this.MainPanel.TabIndex = 0;
            this.MainPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.MainPanel_Paint);
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(9, 11);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(69, 20);
            this.NameLabel.TabIndex = 1;
            this.NameLabel.Text = "PC Build";
            // 
            // PlayButton
            // 
            this.PlayButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PlayButton.Image = global::BuildSync.Client.Properties.Resources.appbar_controller_xbox;
            this.PlayButton.Location = new System.Drawing.Point(734, 11);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(50, 50);
            this.PlayButton.TabIndex = 7;
            this.PlayButton.UseVisualStyleBackColor = true;
            this.PlayButton.Click += new System.EventHandler(this.PlayClicked);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel1.Controls.Add(this.DownloadSpeedLabel);
            this.flowLayoutPanel1.Controls.Add(this.panel3);
            this.flowLayoutPanel1.Controls.Add(this.UploadSpeedLabel);
            this.flowLayoutPanel1.Controls.Add(this.panel4);
            this.flowLayoutPanel1.Controls.Add(this.BuildLabel);
            this.flowLayoutPanel1.Controls.Add(this.panel2);
            this.flowLayoutPanel1.Controls.Add(this.EtaLabel);
            this.flowLayoutPanel1.Controls.Add(this.panel5);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(132, 1);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(583, 36);
            this.flowLayoutPanel1.TabIndex = 6;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // DownloadSpeedLabel
            // 
            this.DownloadSpeedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DownloadSpeedLabel.AutoSize = true;
            this.DownloadSpeedLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.DownloadSpeedLabel.Location = new System.Drawing.Point(511, 10);
            this.DownloadSpeedLabel.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.DownloadSpeedLabel.Name = "DownloadSpeedLabel";
            this.DownloadSpeedLabel.Size = new System.Drawing.Size(69, 20);
            this.DownloadSpeedLabel.TabIndex = 6;
            this.DownloadSpeedLabel.Text = "100 kb/s";
            // 
            // panel3
            // 
            this.panel3.BackgroundImage = global::BuildSync.Client.Properties.Resources.appbar_download;
            this.panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel3.Location = new System.Drawing.Point(470, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(35, 35);
            this.panel3.TabIndex = 7;
            // 
            // UploadSpeedLabel
            // 
            this.UploadSpeedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UploadSpeedLabel.AutoSize = true;
            this.UploadSpeedLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.UploadSpeedLabel.Location = new System.Drawing.Point(395, 10);
            this.UploadSpeedLabel.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.UploadSpeedLabel.Name = "UploadSpeedLabel";
            this.UploadSpeedLabel.Size = new System.Drawing.Size(69, 20);
            this.UploadSpeedLabel.TabIndex = 8;
            this.UploadSpeedLabel.Text = "100 kb/s";
            // 
            // panel4
            // 
            this.panel4.BackgroundImage = global::BuildSync.Client.Properties.Resources.appbar_upload;
            this.panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel4.Location = new System.Drawing.Point(354, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(35, 35);
            this.panel4.TabIndex = 9;
            // 
            // BuildLabel
            // 
            this.BuildLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BuildLabel.AutoSize = true;
            this.BuildLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.BuildLabel.Location = new System.Drawing.Point(217, 10);
            this.BuildLabel.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.BuildLabel.Name = "BuildLabel";
            this.BuildLabel.Size = new System.Drawing.Size(131, 20);
            this.BuildLabel.TabIndex = 2;
            this.BuildLabel.Text = "Tourist/PC/38212";
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::BuildSync.Client.Properties.Resources.appbar_box;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Location = new System.Drawing.Point(176, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(35, 35);
            this.panel2.TabIndex = 5;
            // 
            // EtaLabel
            // 
            this.EtaLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EtaLabel.AutoSize = true;
            this.EtaLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.EtaLabel.Location = new System.Drawing.Point(99, 10);
            this.EtaLabel.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.EtaLabel.Name = "EtaLabel";
            this.EtaLabel.Size = new System.Drawing.Size(71, 20);
            this.EtaLabel.TabIndex = 10;
            this.EtaLabel.Text = "03:12:32";
            // 
            // panel5
            // 
            this.panel5.BackgroundImage = global::BuildSync.Client.Properties.Resources.appbar_timer;
            this.panel5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel5.Location = new System.Drawing.Point(58, 3);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(35, 35);
            this.panel5.TabIndex = 11;
            // 
            // RemoveButton
            // 
            this.RemoveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveButton.Image = global::BuildSync.Client.Properties.Resources.appbar_delete;
            this.RemoveButton.Location = new System.Drawing.Point(790, 11);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(50, 50);
            this.RemoveButton.TabIndex = 4;
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.DeleteClicked);
            // 
            // SettingsButton
            // 
            this.SettingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SettingsButton.Image = global::BuildSync.Client.Properties.Resources.appbar_settings;
            this.SettingsButton.Location = new System.Drawing.Point(846, 11);
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.Size = new System.Drawing.Size(50, 50);
            this.SettingsButton.TabIndex = 3;
            this.SettingsButton.UseVisualStyleBackColor = true;
            this.SettingsButton.Click += new System.EventHandler(this.SettingsClicked);
            // 
            // ProgressBar
            // 
            this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBar.Location = new System.Drawing.Point(13, 39);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(702, 22);
            this.ProgressBar.TabIndex = 0;
            this.ProgressBar.Value = 50;
            // 
            // DownloadListItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.MainPanel);
            this.Name = "DownloadListItem";
            this.Padding = new System.Windows.Forms.Padding(6, 6, 6, 0);
            this.Size = new System.Drawing.Size(915, 77);
            this.MainPanel.ResumeLayout(false);
            this.MainPanel.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Label BuildLabel;
        private System.Windows.Forms.Button SettingsButton;
        private System.Windows.Forms.Button RemoveButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label DownloadSpeedLabel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label UploadSpeedLabel;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label EtaLabel;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button PlayButton;
    }
}
