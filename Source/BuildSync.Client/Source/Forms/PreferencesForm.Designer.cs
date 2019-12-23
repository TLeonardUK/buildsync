namespace BuildSync.Client.Forms
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
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Storage", 0, 0);
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Bandwidth", 1, 1);
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
            this.panel1.Location = new System.Drawing.Point(10, 523);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1002, 1);
            this.panel1.TabIndex = 5;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(853, 536);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(160, 45);
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
            this.groupTreeView.Indent = 5;
            this.groupTreeView.ItemHeight = 45;
            this.groupTreeView.Location = new System.Drawing.Point(0, 0);
            this.groupTreeView.Name = "groupTreeView";
            treeNode1.ImageIndex = 2;
            treeNode1.Name = "serverSettingsNode";
            treeNode1.SelectedImageIndex = 2;
            treeNode1.Text = "Server";
            treeNode2.ImageIndex = 0;
            treeNode2.Name = "storageSettingsNode";
            treeNode2.SelectedImageIndex = 0;
            treeNode2.Text = "Storage";
            treeNode3.ImageIndex = 1;
            treeNode3.Name = "bandwidthSettingsNode";
            treeNode3.SelectedImageIndex = 1;
            treeNode3.Text = "Bandwidth";
            this.groupTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            this.groupTreeView.SelectedImageIndex = 0;
            this.groupTreeView.ShowLines = false;
            this.groupTreeView.ShowPlusMinus = false;
            this.groupTreeView.ShowRootLines = false;
            this.groupTreeView.Size = new System.Drawing.Size(259, 506);
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
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(-5, 1);
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
            this.splitContainer1.Size = new System.Drawing.Size(1038, 506);
            this.splitContainer1.SplitterDistance = 259;
            this.splitContainer1.TabIndex = 6;
            // 
            // settingsPanelContainer
            // 
            this.settingsPanelContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsPanelContainer.AutoScroll = true;
            this.settingsPanelContainer.Location = new System.Drawing.Point(2, 68);
            this.settingsPanelContainer.Name = "settingsPanelContainer";
            this.settingsPanelContainer.Size = new System.Drawing.Size(765, 435);
            this.settingsPanelContainer.TabIndex = 2;
            // 
            // settingsGroupNameLabel
            // 
            this.settingsGroupNameLabel.AutoSize = true;
            this.settingsGroupNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settingsGroupNameLabel.Location = new System.Drawing.Point(2, 21);
            this.settingsGroupNameLabel.Name = "settingsGroupNameLabel";
            this.settingsGroupNameLabel.Size = new System.Drawing.Size(147, 26);
            this.settingsGroupNameLabel.TabIndex = 0;
            this.settingsGroupNameLabel.Text = "Group Name";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Location = new System.Drawing.Point(22, 35);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(730, 0);
            this.panel2.TabIndex = 1;
            // 
            // PreferencesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1025, 593);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.closeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(1047, 452);
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