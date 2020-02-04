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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadListItem));
            this.MainPanel = new System.Windows.Forms.Panel();
            this.blockStatusPanel = new BuildSync.Client.Controls.BlockStatusPanel();
            this.collapseButton = new System.Windows.Forms.Button();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.NameLabel = new System.Windows.Forms.Label();
            this.PlayButton = new System.Windows.Forms.Button();
            this.Buttonimages = new System.Windows.Forms.ImageList(this.components);
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
            this.blockRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.MainPanel.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainPanel
            // 
            this.MainPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.MainPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.MainPanel.Controls.Add(this.blockStatusPanel);
            this.MainPanel.Controls.Add(this.collapseButton);
            this.MainPanel.Controls.Add(this.ProgressBar);
            this.MainPanel.Controls.Add(this.NameLabel);
            this.MainPanel.Controls.Add(this.PlayButton);
            this.MainPanel.Controls.Add(this.flowLayoutPanel1);
            this.MainPanel.Controls.Add(this.RemoveButton);
            this.MainPanel.Controls.Add(this.SettingsButton);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(4, 4);
            this.MainPanel.Margin = new System.Windows.Forms.Padding(2);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(602, 46);
            this.MainPanel.TabIndex = 0;
            this.MainPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.MainPanel_Paint);
            // 
            // blockStatusPanel
            // 
            this.blockStatusPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.blockStatusPanel.BackColor = System.Drawing.SystemColors.Control;
            this.blockStatusPanel.Location = new System.Drawing.Point(0, 45);
            this.blockStatusPanel.Name = "blockStatusPanel";
            this.blockStatusPanel.Size = new System.Drawing.Size(602, 101);
            this.blockStatusPanel.TabIndex = 9;
            // 
            // collapseButton
            // 
            this.collapseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.collapseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.collapseButton.Location = new System.Drawing.Point(584, 7);
            this.collapseButton.Name = "collapseButton";
            this.collapseButton.Size = new System.Drawing.Size(10, 32);
            this.collapseButton.TabIndex = 8;
            this.collapseButton.UseVisualStyleBackColor = true;
            this.collapseButton.Click += new System.EventHandler(this.CollapseButtonClicked);
            // 
            // ProgressBar
            // 
            this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBar.Location = new System.Drawing.Point(8, 25);
            this.ProgressBar.Margin = new System.Windows.Forms.Padding(2);
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Size = new System.Drawing.Size(455, 14);
            this.ProgressBar.TabIndex = 0;
            this.ProgressBar.Value = 50;
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(8, 7);
            this.NameLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(47, 13);
            this.NameLabel.TabIndex = 1;
            this.NameLabel.Text = "PC Build";
            // 
            // PlayButton
            // 
            this.PlayButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PlayButton.ImageIndex = 0;
            this.PlayButton.ImageList = this.Buttonimages;
            this.PlayButton.Location = new System.Drawing.Point(472, 7);
            this.PlayButton.Margin = new System.Windows.Forms.Padding(2);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(33, 32);
            this.PlayButton.TabIndex = 7;
            this.PlayButton.UseVisualStyleBackColor = true;
            this.PlayButton.Click += new System.EventHandler(this.PlayClicked);
            // 
            // Buttonimages
            // 
            this.Buttonimages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("Buttonimages.ImageStream")));
            this.Buttonimages.TransparentColor = System.Drawing.Color.Transparent;
            this.Buttonimages.Images.SetKeyName(0, "appbar.controller.xbox.png");
            this.Buttonimages.Images.SetKeyName(1, "appbar.settings.png");
            this.Buttonimages.Images.SetKeyName(2, "appbar.delete.png");
            this.Buttonimages.Images.SetKeyName(3, "appbar.control.play.png");
            this.Buttonimages.Images.SetKeyName(4, "appbar.control.pause.png");
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
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
            this.flowLayoutPanel1.Location = new System.Drawing.Point(81, 1);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(382, 28);
            this.flowLayoutPanel1.TabIndex = 6;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // DownloadSpeedLabel
            // 
            this.DownloadSpeedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DownloadSpeedLabel.AutoSize = true;
            this.DownloadSpeedLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.DownloadSpeedLabel.Location = new System.Drawing.Point(330, 6);
            this.DownloadSpeedLabel.Margin = new System.Windows.Forms.Padding(2, 6, 2, 0);
            this.DownloadSpeedLabel.Name = "DownloadSpeedLabel";
            this.DownloadSpeedLabel.Size = new System.Drawing.Size(50, 13);
            this.DownloadSpeedLabel.TabIndex = 6;
            this.DownloadSpeedLabel.Text = "100 kb/s";
            // 
            // panel3
            // 
            this.panel3.BackgroundImage = global::BuildSync.Client.Properties.Resources.appbar_download;
            this.panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel3.Location = new System.Drawing.Point(303, 2);
            this.panel3.Margin = new System.Windows.Forms.Padding(2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(23, 23);
            this.panel3.TabIndex = 7;
            // 
            // UploadSpeedLabel
            // 
            this.UploadSpeedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UploadSpeedLabel.AutoSize = true;
            this.UploadSpeedLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.UploadSpeedLabel.Location = new System.Drawing.Point(249, 6);
            this.UploadSpeedLabel.Margin = new System.Windows.Forms.Padding(2, 6, 2, 0);
            this.UploadSpeedLabel.Name = "UploadSpeedLabel";
            this.UploadSpeedLabel.Size = new System.Drawing.Size(50, 13);
            this.UploadSpeedLabel.TabIndex = 8;
            this.UploadSpeedLabel.Text = "100 kb/s";
            // 
            // panel4
            // 
            this.panel4.BackgroundImage = global::BuildSync.Client.Properties.Resources.appbar_upload;
            this.panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel4.Location = new System.Drawing.Point(222, 2);
            this.panel4.Margin = new System.Windows.Forms.Padding(2);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(23, 23);
            this.panel4.TabIndex = 9;
            // 
            // BuildLabel
            // 
            this.BuildLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BuildLabel.AutoSize = true;
            this.BuildLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.BuildLabel.Location = new System.Drawing.Point(125, 6);
            this.BuildLabel.Margin = new System.Windows.Forms.Padding(2, 6, 2, 0);
            this.BuildLabel.Name = "BuildLabel";
            this.BuildLabel.Size = new System.Drawing.Size(93, 13);
            this.BuildLabel.TabIndex = 2;
            this.BuildLabel.Text = "Tourist/PC/38212";
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = global::BuildSync.Client.Properties.Resources.appbar_box;
            this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel2.Location = new System.Drawing.Point(98, 2);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(23, 23);
            this.panel2.TabIndex = 5;
            // 
            // EtaLabel
            // 
            this.EtaLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EtaLabel.AutoSize = true;
            this.EtaLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.EtaLabel.Location = new System.Drawing.Point(45, 6);
            this.EtaLabel.Margin = new System.Windows.Forms.Padding(2, 6, 2, 0);
            this.EtaLabel.Name = "EtaLabel";
            this.EtaLabel.Size = new System.Drawing.Size(49, 13);
            this.EtaLabel.TabIndex = 10;
            this.EtaLabel.Text = "03:12:32";
            // 
            // panel5
            // 
            this.panel5.BackgroundImage = global::BuildSync.Client.Properties.Resources.appbar_timer;
            this.panel5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel5.Location = new System.Drawing.Point(17, 2);
            this.panel5.Margin = new System.Windows.Forms.Padding(2);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(24, 24);
            this.panel5.TabIndex = 11;
            // 
            // RemoveButton
            // 
            this.RemoveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveButton.ImageIndex = 2;
            this.RemoveButton.ImageList = this.Buttonimages;
            this.RemoveButton.Location = new System.Drawing.Point(510, 7);
            this.RemoveButton.Margin = new System.Windows.Forms.Padding(2);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(33, 32);
            this.RemoveButton.TabIndex = 4;
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.DeleteClicked);
            // 
            // SettingsButton
            // 
            this.SettingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SettingsButton.ImageIndex = 1;
            this.SettingsButton.ImageList = this.Buttonimages;
            this.SettingsButton.Location = new System.Drawing.Point(547, 7);
            this.SettingsButton.Margin = new System.Windows.Forms.Padding(2);
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.Size = new System.Drawing.Size(33, 32);
            this.SettingsButton.TabIndex = 3;
            this.SettingsButton.UseVisualStyleBackColor = true;
            this.SettingsButton.Click += new System.EventHandler(this.SettingsClicked);
            // 
            // blockRefreshTimer
            // 
            this.blockRefreshTimer.Enabled = true;
            this.blockRefreshTimer.Tick += new System.EventHandler(this.BlockRefreshTimer);
            // 
            // DownloadListItem
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.MainPanel);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DownloadListItem";
            this.Padding = new System.Windows.Forms.Padding(4, 4, 4, 0);
            this.Size = new System.Drawing.Size(610, 50);
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
        private System.Windows.Forms.ImageList Buttonimages;
        private System.Windows.Forms.Button collapseButton;
        private System.Windows.Forms.Timer blockRefreshTimer;
        private BlockStatusPanel blockStatusPanel;
    }
}
