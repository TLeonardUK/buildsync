﻿namespace BuildSync.Client.Forms
{
    partial class PreferencesForm
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Server", 2, 2);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("General", 3, 3);
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Storage", 0, 0);
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Bandwidth", 1, 1);
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Source Control", 4, 4);
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Replication", 6, 6);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferencesForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.closeButton = new System.Windows.Forms.Button();
            this.groupTreeView = new System.Windows.Forms.TreeView();
            this.treeImageList = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.settingsPanelContainer = new System.Windows.Forms.Panel();
            this.settingsGroupNameLabel = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(7, 401);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(692, 1);
            this.panel1.TabIndex = 5;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(593, 409);
            this.closeButton.Margin = new System.Windows.Forms.Padding(2);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(107, 29);
            this.closeButton.TabIndex = 4;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseClicked);
            // 
            // groupTreeView
            // 
            this.groupTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupTreeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupTreeView.FullRowSelect = true;
            this.groupTreeView.HideSelection = false;
            this.groupTreeView.ImageIndex = 0;
            this.groupTreeView.ImageList = this.treeImageList;
            this.groupTreeView.Indent = 51;
            this.groupTreeView.ItemHeight = 38;
            this.groupTreeView.Location = new System.Drawing.Point(0, 0);
            this.groupTreeView.Margin = new System.Windows.Forms.Padding(2);
            this.groupTreeView.Name = "groupTreeView";
            treeNode1.ImageIndex = 2;
            treeNode1.Name = "serverSettingsNode";
            treeNode1.SelectedImageIndex = 2;
            treeNode1.Text = "Server";
            treeNode2.ImageIndex = 3;
            treeNode2.Name = "generalSettingsNode";
            treeNode2.SelectedImageIndex = 3;
            treeNode2.Text = "General";
            treeNode3.ImageIndex = 0;
            treeNode3.Name = "storageSettingsNode";
            treeNode3.SelectedImageIndex = 0;
            treeNode3.Text = "Storage";
            treeNode4.ImageIndex = 1;
            treeNode4.Name = "bandwidthSettingsNode";
            treeNode4.SelectedImageIndex = 1;
            treeNode4.Text = "Bandwidth";
            treeNode5.ImageIndex = 4;
            treeNode5.Name = "scmSettingsNode";
            treeNode5.SelectedImageIndex = 4;
            treeNode5.Text = "Source Control";
            treeNode6.ImageIndex = 6;
            treeNode6.Name = "replicationSettingsNode";
            treeNode6.SelectedImageIndex = 6;
            treeNode6.Text = "Replication";
            this.groupTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6});
            this.groupTreeView.SelectedImageIndex = 0;
            this.groupTreeView.ShowRootLines = false;
            this.groupTreeView.Size = new System.Drawing.Size(148, 390);
            this.groupTreeView.TabIndex = 0;
            this.groupTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.SelectedSettingsNodeChanged);
            // 
            // treeImageList
            // 
            this.treeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeImageList.ImageStream")));
            this.treeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.treeImageList.Images.SetKeyName(0, "appbar.disk.png");
            this.treeImageList.Images.SetKeyName(1, "appbar.arrow.left.right.png");
            this.treeImageList.Images.SetKeyName(2, "appbar.connect.png");
            this.treeImageList.Images.SetKeyName(3, "appbar.settings.png");
            this.treeImageList.Images.SetKeyName(4, "appbar.database.png");
            this.treeImageList.Images.SetKeyName(5, "perforce.png");
            this.treeImageList.Images.SetKeyName(6, "appbar.diagram.png");
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(-3, 1);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.settingsPanelContainer);
            this.splitContainer1.Panel2.Controls.Add(this.settingsGroupNameLabel);
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Size = new System.Drawing.Size(716, 390);
            this.splitContainer1.SplitterDistance = 148;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 6;
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // settingsPanelContainer
            // 
            this.settingsPanelContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsPanelContainer.AutoScroll = true;
            this.settingsPanelContainer.Location = new System.Drawing.Point(1, 44);
            this.settingsPanelContainer.Margin = new System.Windows.Forms.Padding(2);
            this.settingsPanelContainer.Name = "settingsPanelContainer";
            this.settingsPanelContainer.Size = new System.Drawing.Size(562, 344);
            this.settingsPanelContainer.TabIndex = 2;
            // 
            // settingsGroupNameLabel
            // 
            this.settingsGroupNameLabel.AutoSize = true;
            this.settingsGroupNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settingsGroupNameLabel.Location = new System.Drawing.Point(1, 14);
            this.settingsGroupNameLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.settingsGroupNameLabel.Name = "settingsGroupNameLabel";
            this.settingsGroupNameLabel.Size = new System.Drawing.Size(104, 18);
            this.settingsGroupNameLabel.TabIndex = 0;
            this.settingsGroupNameLabel.Text = "Group Name";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Location = new System.Drawing.Point(15, 23);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(539, 1);
            this.panel2.TabIndex = 1;
            // 
            // PreferencesForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(711, 446);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.closeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(703, 307);
            this.Name = "PreferencesForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preferences";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormHasClosed);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.TreeView groupTreeView;
        private System.Windows.Forms.ImageList treeImageList;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel settingsPanelContainer;
        private System.Windows.Forms.Label settingsGroupNameLabel;
        private System.Windows.Forms.Panel panel2;
    }
}