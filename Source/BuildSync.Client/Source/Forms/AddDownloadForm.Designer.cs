namespace BuildSync.Client.Forms
{
    partial class AddDownloadForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddDownloadForm));
            this.addDownloadButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.autoUpdateCheckBox = new System.Windows.Forms.CheckBox();
            this.priorityComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.autoInstallCheckBox = new System.Windows.Forms.CheckBox();
            this.deviceTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.LegendImages = new System.Windows.Forms.ImageList(this.components);
            this.selectionRuleComboBox = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.scmSettingsPanel = new System.Windows.Forms.Panel();
            this.workspaceComboBox = new System.Windows.Forms.ComboBox();
            this.buildSelectionRulePanel = new System.Windows.Forms.Panel();
            this.selectionFilterComboBox = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.scmFilePanel = new System.Windows.Forms.Panel();
            this.scmFileTextBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.installLocationTextBox = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.downloadFileSystemTree = new BuildSync.Client.Controls.DownloadFileSystemTree();
            this.scmSettingsPanel.SuspendLayout();
            this.buildSelectionRulePanel.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.scmFilePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // addDownloadButton
            // 
            this.addDownloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addDownloadButton.Enabled = false;
            this.addDownloadButton.Location = new System.Drawing.Point(974, 472);
            this.addDownloadButton.Margin = new System.Windows.Forms.Padding(2);
            this.addDownloadButton.Name = "addDownloadButton";
            this.addDownloadButton.Size = new System.Drawing.Size(95, 29);
            this.addDownloadButton.TabIndex = 2;
            this.addDownloadButton.Text = "Add Download";
            this.addDownloadButton.UseVisualStyleBackColor = true;
            this.addDownloadButton.Click += new System.EventHandler(this.AddDownloadClicked);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(10, 461);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1059, 1);
            this.panel1.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 43);
            this.label3.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(340, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Priority of download";
            // 
            // autoUpdateCheckBox
            // 
            this.autoUpdateCheckBox.AutoSize = true;
            this.autoUpdateCheckBox.Checked = true;
            this.autoUpdateCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoUpdateCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.autoUpdateCheckBox.Location = new System.Drawing.Point(2, 274);
            this.autoUpdateCheckBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.autoUpdateCheckBox.Name = "autoUpdateCheckBox";
            this.autoUpdateCheckBox.Size = new System.Drawing.Size(338, 17);
            this.autoUpdateCheckBox.TabIndex = 11;
            this.autoUpdateCheckBox.Text = "Automatically keep up to date";
            this.autoUpdateCheckBox.UseVisualStyleBackColor = true;
            this.autoUpdateCheckBox.CheckedChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // priorityComboBox
            // 
            this.priorityComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.priorityComboBox.FormattingEnabled = true;
            this.priorityComboBox.Location = new System.Drawing.Point(2, 58);
            this.priorityComboBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.priorityComboBox.Name = "priorityComboBox";
            this.priorityComboBox.Size = new System.Drawing.Size(338, 21);
            this.priorityComboBox.TabIndex = 12;
            this.priorityComboBox.SelectedIndexChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(340, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Name";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.nameTextBox.Location = new System.Drawing.Point(2, 15);
            this.nameTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.nameTextBox.MinimumSize = new System.Drawing.Size(338, 0);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(338, 20);
            this.nameTextBox.TabIndex = 6;
            this.nameTextBox.TextChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // autoInstallCheckBox
            // 
            this.autoInstallCheckBox.AutoSize = true;
            this.autoInstallCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.autoInstallCheckBox.Location = new System.Drawing.Point(2, 301);
            this.autoInstallCheckBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.autoInstallCheckBox.Name = "autoInstallCheckBox";
            this.autoInstallCheckBox.Size = new System.Drawing.Size(338, 17);
            this.autoInstallCheckBox.TabIndex = 14;
            this.autoInstallCheckBox.Text = "Automatically install";
            this.autoInstallCheckBox.UseVisualStyleBackColor = true;
            this.autoInstallCheckBox.CheckedChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // deviceTextBox
            // 
            this.deviceTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.deviceTextBox.Location = new System.Drawing.Point(2, 341);
            this.deviceTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.deviceTextBox.Name = "deviceTextBox";
            this.deviceTextBox.Size = new System.Drawing.Size(338, 20);
            this.deviceTextBox.TabIndex = 16;
            this.deviceTextBox.TextChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(0, 326);
            this.label4.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(340, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Device name or ip to use when installing and running";
            // 
            // LegendImages
            // 
            this.LegendImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("LegendImages.ImageStream")));
            this.LegendImages.TransparentColor = System.Drawing.Color.Transparent;
            this.LegendImages.Images.SetKeyName(0, "appbar.box.png");
            this.LegendImages.Images.SetKeyName(1, "appbar.database.png");
            // 
            // selectionRuleComboBox
            // 
            this.selectionRuleComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.selectionRuleComboBox.FormattingEnabled = true;
            this.selectionRuleComboBox.Items.AddRange(new object[] {
            "Newest",
            "Oldest",
            "Closest one before SCM head revision",
            "Closest one after SCM head revision"});
            this.selectionRuleComboBox.Location = new System.Drawing.Point(0, 15);
            this.selectionRuleComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.selectionRuleComboBox.Name = "selectionRuleComboBox";
            this.selectionRuleComboBox.Size = new System.Drawing.Size(338, 21);
            this.selectionRuleComboBox.TabIndex = 23;
            this.selectionRuleComboBox.SelectedIndexChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(0, 0);
            this.label9.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(95, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "Build selection rule";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(0, 0);
            this.label10.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(131, 13);
            this.label10.TabIndex = 24;
            this.label10.Text = "Source control workspace";
            // 
            // scmSettingsPanel
            // 
            this.scmSettingsPanel.Controls.Add(this.workspaceComboBox);
            this.scmSettingsPanel.Controls.Add(this.label10);
            this.scmSettingsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.scmSettingsPanel.Location = new System.Drawing.Point(2, 179);
            this.scmSettingsPanel.Margin = new System.Windows.Forms.Padding(2, 3, 3, 8);
            this.scmSettingsPanel.Name = "scmSettingsPanel";
            this.scmSettingsPanel.Size = new System.Drawing.Size(337, 37);
            this.scmSettingsPanel.TabIndex = 27;
            this.scmSettingsPanel.Visible = false;
            // 
            // workspaceComboBox
            // 
            this.workspaceComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.workspaceComboBox.FormattingEnabled = true;
            this.workspaceComboBox.Items.AddRange(new object[] {
            "Newest",
            "Oldest",
            "Closest one before SCM head revision",
            "Closest one after SCM head revision"});
            this.workspaceComboBox.Location = new System.Drawing.Point(0, 15);
            this.workspaceComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.workspaceComboBox.Name = "workspaceComboBox";
            this.workspaceComboBox.Size = new System.Drawing.Size(337, 21);
            this.workspaceComboBox.TabIndex = 25;
            this.workspaceComboBox.SelectedIndexChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // buildSelectionRulePanel
            // 
            this.buildSelectionRulePanel.Controls.Add(this.selectionFilterComboBox);
            this.buildSelectionRulePanel.Controls.Add(this.label11);
            this.buildSelectionRulePanel.Controls.Add(this.selectionRuleComboBox);
            this.buildSelectionRulePanel.Controls.Add(this.label9);
            this.buildSelectionRulePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.buildSelectionRulePanel.Location = new System.Drawing.Point(2, 90);
            this.buildSelectionRulePanel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 4);
            this.buildSelectionRulePanel.Name = "buildSelectionRulePanel";
            this.buildSelectionRulePanel.Size = new System.Drawing.Size(338, 82);
            this.buildSelectionRulePanel.TabIndex = 28;
            this.buildSelectionRulePanel.Visible = false;
            // 
            // selectionFilterComboBox
            // 
            this.selectionFilterComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.selectionFilterComboBox.FormattingEnabled = true;
            this.selectionFilterComboBox.Items.AddRange(new object[] {
            "Newest",
            "Oldest",
            "Closest one before SCM head revision",
            "Closest one after SCM head revision"});
            this.selectionFilterComboBox.Location = new System.Drawing.Point(0, 58);
            this.selectionFilterComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.selectionFilterComboBox.Name = "selectionFilterComboBox";
            this.selectionFilterComboBox.Size = new System.Drawing.Size(338, 21);
            this.selectionFilterComboBox.TabIndex = 25;
            this.selectionFilterComboBox.SelectedIndexChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(-1, 43);
            this.label11.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(97, 13);
            this.label11.TabIndex = 24;
            this.label11.Text = "Build selection filter";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.nameTextBox);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.priorityComboBox);
            this.flowLayoutPanel1.Controls.Add(this.buildSelectionRulePanel);
            this.flowLayoutPanel1.Controls.Add(this.scmSettingsPanel);
            this.flowLayoutPanel1.Controls.Add(this.scmFilePanel);
            this.flowLayoutPanel1.Controls.Add(this.autoUpdateCheckBox);
            this.flowLayoutPanel1.Controls.Add(this.autoInstallCheckBox);
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Controls.Add(this.deviceTextBox);
            this.flowLayoutPanel1.Controls.Add(this.label13);
            this.flowLayoutPanel1.Controls.Add(this.installLocationTextBox);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(350, 444);
            this.flowLayoutPanel1.TabIndex = 29;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // scmFilePanel
            // 
            this.scmFilePanel.Controls.Add(this.scmFileTextBox);
            this.scmFilePanel.Controls.Add(this.label12);
            this.scmFilePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.scmFilePanel.Location = new System.Drawing.Point(2, 227);
            this.scmFilePanel.Margin = new System.Windows.Forms.Padding(2, 3, 3, 8);
            this.scmFilePanel.Name = "scmFilePanel";
            this.scmFilePanel.Size = new System.Drawing.Size(337, 37);
            this.scmFilePanel.TabIndex = 30;
            this.scmFilePanel.Visible = false;
            // 
            // scmFileTextBox
            // 
            this.scmFileTextBox.Location = new System.Drawing.Point(0, 17);
            this.scmFileTextBox.Name = "scmFileTextBox";
            this.scmFileTextBox.Size = new System.Drawing.Size(337, 20);
            this.scmFileTextBox.TabIndex = 25;
            this.scmFileTextBox.TextChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(0, 0);
            this.label12.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(214, 13);
            this.label12.TabIndex = 24;
            this.label12.Text = "Source control, workspace relative, file path";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Dock = System.Windows.Forms.DockStyle.Top;
            this.label13.Location = new System.Drawing.Point(0, 369);
            this.label13.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(340, 13);
            this.label13.TabIndex = 31;
            this.label13.Text = "Location or workspace to install to";
            // 
            // installLocationTextBox
            // 
            this.installLocationTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.installLocationTextBox.Location = new System.Drawing.Point(2, 384);
            this.installLocationTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 8);
            this.installLocationTextBox.Name = "installLocationTextBox";
            this.installLocationTextBox.Size = new System.Drawing.Size(338, 20);
            this.installLocationTextBox.TabIndex = 32;
            this.installLocationTextBox.Text = "buildsync";
            this.installLocationTextBox.TextChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(8, 12);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.flowLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label7);
            this.splitContainer1.Panel2.Controls.Add(this.label8);
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.downloadFileSystemTree);
            this.splitContainer1.Size = new System.Drawing.Size(1060, 444);
            this.splitContainer1.SplitterDistance = 350;
            this.splitContainer1.TabIndex = 30;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label7.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label7.ImageIndex = 0;
            this.label7.ImageList = this.LegendImages;
            this.label7.Location = new System.Drawing.Point(269, 404);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 34);
            this.label7.TabIndex = 23;
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label8.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label8.ImageIndex = 0;
            this.label8.Location = new System.Drawing.Point(313, 404);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(227, 34);
            this.label8.TabIndex = 22;
            this.label8.Text = "This is an individual build, selecting it will always download that specific buil" +
    "d.\r\n";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label6.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label6.ImageIndex = 1;
            this.label6.ImageList = this.LegendImages;
            this.label6.Location = new System.Drawing.Point(-1, 404);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 40);
            this.label6.TabIndex = 21;
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label5.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label5.ImageIndex = 0;
            this.label5.Location = new System.Drawing.Point(43, 404);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(227, 40);
            this.label5.TabIndex = 20;
            this.label5.Text = "This is a build container, selecting it will result in downloading a build in it " +
    "that matches the build selection rule.\r\n";
            // 
            // downloadFileSystemTree
            // 
            this.downloadFileSystemTree.CanSelectBuildContainers = true;
            this.downloadFileSystemTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downloadFileSystemTree.Location = new System.Drawing.Point(0, 0);
            this.downloadFileSystemTree.Margin = new System.Windows.Forms.Padding(1);
            this.downloadFileSystemTree.Name = "downloadFileSystemTree";
            this.downloadFileSystemTree.Padding = new System.Windows.Forms.Padding(0, 0, 0, 50);
            this.downloadFileSystemTree.SelectedPath = "";
            this.downloadFileSystemTree.ShowInternal = false;
            this.downloadFileSystemTree.Size = new System.Drawing.Size(706, 444);
            this.downloadFileSystemTree.TabIndex = 13;
            this.downloadFileSystemTree.OnDateUpdated += new System.EventHandler(this.DataStateChanged);
            this.downloadFileSystemTree.OnSelectedNodeChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // AddDownloadForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1080, 510);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.addDownloadButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(660, 460);
            this.Name = "AddDownloadForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Download";
            this.Load += new System.EventHandler(this.OnLoaded);
            this.Shown += new System.EventHandler(this.OnShown);
            this.scmSettingsPanel.ResumeLayout(false);
            this.scmSettingsPanel.PerformLayout();
            this.buildSelectionRulePanel.ResumeLayout(false);
            this.buildSelectionRulePanel.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.scmFilePanel.ResumeLayout(false);
            this.scmFilePanel.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button addDownloadButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox autoUpdateCheckBox;
        private System.Windows.Forms.ComboBox priorityComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox nameTextBox;
        private Controls.DownloadFileSystemTree downloadFileSystemTree;
        private System.Windows.Forms.CheckBox autoInstallCheckBox;
        private System.Windows.Forms.TextBox deviceTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ImageList LegendImages;
        private System.Windows.Forms.ComboBox selectionRuleComboBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel scmSettingsPanel;
        private System.Windows.Forms.Panel buildSelectionRulePanel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ComboBox workspaceComboBox;
        private System.Windows.Forms.ComboBox selectionFilterComboBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel scmFilePanel;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox scmFileTextBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox installLocationTextBox;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
    }
}