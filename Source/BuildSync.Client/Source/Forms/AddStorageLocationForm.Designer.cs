namespace BuildSync.Client.Forms
{
    partial class AddStorageLocationForm
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
            this.LocalFolderBrowseButton = new System.Windows.Forms.Button();
            this.LocalFolderTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AddButton = new System.Windows.Forms.Button();
            this.MaxSizeTextBox = new BuildSync.Core.Controls.SizeTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LocalFolderBrowseButton
            // 
            this.LocalFolderBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LocalFolderBrowseButton.Location = new System.Drawing.Point(443, 27);
            this.LocalFolderBrowseButton.Margin = new System.Windows.Forms.Padding(2);
            this.LocalFolderBrowseButton.Name = "LocalFolderBrowseButton";
            this.LocalFolderBrowseButton.Size = new System.Drawing.Size(88, 28);
            this.LocalFolderBrowseButton.TabIndex = 5;
            this.LocalFolderBrowseButton.Text = "Browse";
            this.LocalFolderBrowseButton.UseVisualStyleBackColor = true;
            this.LocalFolderBrowseButton.Click += new System.EventHandler(this.BrowseClicked);
            // 
            // LocalFolderTextBox
            // 
            this.LocalFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LocalFolderTextBox.Location = new System.Drawing.Point(9, 32);
            this.LocalFolderTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.LocalFolderTextBox.Name = "LocalFolderTextBox";
            this.LocalFolderTextBox.ReadOnly = true;
            this.LocalFolderTextBox.Size = new System.Drawing.Size(430, 20);
            this.LocalFolderTextBox.TabIndex = 4;
            this.LocalFolderTextBox.TextChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 14);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Local Folder";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(9, 120);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(523, 1);
            this.panel1.TabIndex = 28;
            // 
            // AddButton
            // 
            this.AddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddButton.Enabled = false;
            this.AddButton.Location = new System.Drawing.Point(396, 132);
            this.AddButton.Margin = new System.Windows.Forms.Padding(2);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(135, 29);
            this.AddButton.TabIndex = 6;
            this.AddButton.Text = "OK";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddClicked);
            // 
            // MaxSizeTextBox
            // 
            this.MaxSizeTextBox.DisplayAsTransferRate = false;
            this.MaxSizeTextBox.Location = new System.Drawing.Point(9, 79);
            this.MaxSizeTextBox.Name = "MaxSizeTextBox";
            this.MaxSizeTextBox.Size = new System.Drawing.Size(522, 26);
            this.MaxSizeTextBox.TabIndex = 32;
            this.MaxSizeTextBox.Value = ((long)(0));
            this.MaxSizeTextBox.OnValueChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 63);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 13);
            this.label1.TabIndex = 33;
            this.label1.Text = "Maximum Size (0 for unlimited)";
            // 
            // AddStorageLocationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 173);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MaxSizeTextBox);
            this.Controls.Add(this.LocalFolderBrowseButton);
            this.Controls.Add(this.LocalFolderTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.AddButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddStorageLocationForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Modify Storage Location";
            this.Load += new System.EventHandler(this.OnLoaded);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LocalFolderBrowseButton;
        private System.Windows.Forms.TextBox LocalFolderTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button AddButton;
        private Core.Controls.SizeTextBox MaxSizeTextBox;
        private System.Windows.Forms.Label label1;
    }
}