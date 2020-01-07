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
            this.label1 = new System.Windows.Forms.Label();
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
            this.label5 = new System.Windows.Forms.Label();
            this.LegendImages = new System.Windows.Forms.ImageList(this.components);
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.downloadFileSystemTree = new BuildSync.Client.Controls.DownloadFileSystemTree();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 8);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Location to retrieve build from:";
            // 
            // addDownloadButton
            // 
            this.addDownloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addDownloadButton.Enabled = false;
            this.addDownloadButton.Location = new System.Drawing.Point(489, 280);
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
            this.panel1.Location = new System.Drawing.Point(292, 268);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(292, 1);
            this.panel1.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(286, 56);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Priority of download";
            // 
            // autoUpdateCheckBox
            // 
            this.autoUpdateCheckBox.AutoSize = true;
            this.autoUpdateCheckBox.Checked = true;
            this.autoUpdateCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoUpdateCheckBox.Location = new System.Drawing.Point(289, 103);
            this.autoUpdateCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.autoUpdateCheckBox.Name = "autoUpdateCheckBox";
            this.autoUpdateCheckBox.Size = new System.Drawing.Size(166, 17);
            this.autoUpdateCheckBox.TabIndex = 11;
            this.autoUpdateCheckBox.Text = "Automatically keep up to date";
            this.autoUpdateCheckBox.UseVisualStyleBackColor = true;
            this.autoUpdateCheckBox.CheckedChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // priorityComboBox
            // 
            this.priorityComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.priorityComboBox.FormattingEnabled = true;
            this.priorityComboBox.Location = new System.Drawing.Point(289, 71);
            this.priorityComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.priorityComboBox.Name = "priorityComboBox";
            this.priorityComboBox.Size = new System.Drawing.Size(295, 21);
            this.priorityComboBox.TabIndex = 12;
            this.priorityComboBox.SelectedIndexChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(286, 14);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Name";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nameTextBox.Location = new System.Drawing.Point(289, 29);
            this.nameTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(295, 20);
            this.nameTextBox.TabIndex = 6;
            this.nameTextBox.TextChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // autoInstallCheckBox
            // 
            this.autoInstallCheckBox.AutoSize = true;
            this.autoInstallCheckBox.Location = new System.Drawing.Point(289, 124);
            this.autoInstallCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.autoInstallCheckBox.Name = "autoInstallCheckBox";
            this.autoInstallCheckBox.Size = new System.Drawing.Size(117, 17);
            this.autoInstallCheckBox.TabIndex = 14;
            this.autoInstallCheckBox.Text = "Automatically install";
            this.autoInstallCheckBox.UseVisualStyleBackColor = true;
            this.autoInstallCheckBox.CheckedChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // deviceTextBox
            // 
            this.deviceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.deviceTextBox.Location = new System.Drawing.Point(289, 161);
            this.deviceTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.deviceTextBox.Name = "deviceTextBox";
            this.deviceTextBox.Size = new System.Drawing.Size(295, 20);
            this.deviceTextBox.TabIndex = 16;
            this.deviceTextBox.TextChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(286, 146);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(256, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Device name or ip to use when installing and running";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label5.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label5.ImageIndex = 0;
            this.label5.Location = new System.Drawing.Point(56, 278);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(227, 40);
            this.label5.TabIndex = 17;
            this.label5.Text = "This is a build container, selecting it will result in downloading the latest bui" +
    "ld contained within it.";
            // 
            // LegendImages
            // 
            this.LegendImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("LegendImages.ImageStream")));
            this.LegendImages.TransparentColor = System.Drawing.Color.Transparent;
            this.LegendImages.Images.SetKeyName(0, "appbar.box.png");
            this.LegendImages.Images.SetKeyName(1, "appbar.database.png");
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label6.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label6.ImageIndex = 1;
            this.label6.ImageList = this.LegendImages;
            this.label6.Location = new System.Drawing.Point(12, 278);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 40);
            this.label6.TabIndex = 19;
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label7.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label7.ImageIndex = 0;
            this.label7.ImageList = this.LegendImages;
            this.label7.Location = new System.Drawing.Point(12, 326);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 34);
            this.label7.TabIndex = 21;
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label8.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label8.ImageIndex = 0;
            this.label8.Location = new System.Drawing.Point(56, 326);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(227, 34);
            this.label8.TabIndex = 20;
            this.label8.Text = "This is an individual build, selecting it will always download that specific buil" +
    "d.\r\n";
            // 
            // downloadFileSystemTree
            // 
            this.downloadFileSystemTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.downloadFileSystemTree.CanSelectBuildContainers = true;
            this.downloadFileSystemTree.Location = new System.Drawing.Point(12, 29);
            this.downloadFileSystemTree.Margin = new System.Windows.Forms.Padding(1);
            this.downloadFileSystemTree.Name = "downloadFileSystemTree";
            this.downloadFileSystemTree.SelectedPath = "";
            this.downloadFileSystemTree.Size = new System.Drawing.Size(271, 240);
            this.downloadFileSystemTree.TabIndex = 13;
            this.downloadFileSystemTree.OnSelectedNodeChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // AddDownloadForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(595, 369);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.deviceTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.autoInstallCheckBox);
            this.Controls.Add(this.downloadFileSystemTree);
            this.Controls.Add(this.priorityComboBox);
            this.Controls.Add(this.autoUpdateCheckBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.addDownloadButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(565, 282);
            this.Name = "AddDownloadForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Download";
            this.Load += new System.EventHandler(this.OnLoaded);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
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
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ImageList LegendImages;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}