namespace BuildSync.Client.Forms
{
    partial class AddPermissionForm
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
            this.permissionTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.virtualPathTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.addDownloadButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // permissionTypeComboBox
            // 
            this.permissionTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.permissionTypeComboBox.FormattingEnabled = true;
            this.permissionTypeComboBox.Location = new System.Drawing.Point(11, 29);
            this.permissionTypeComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.permissionTypeComboBox.Name = "permissionTypeComboBox";
            this.permissionTypeComboBox.Size = new System.Drawing.Size(379, 21);
            this.permissionTypeComboBox.TabIndex = 21;
            this.permissionTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.PermissionChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 14);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Permission Type";
            // 
            // virtualPathTextBox
            // 
            this.virtualPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.virtualPathTextBox.Location = new System.Drawing.Point(11, 76);
            this.virtualPathTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.virtualPathTextBox.Name = "virtualPathTextBox";
            this.virtualPathTextBox.Size = new System.Drawing.Size(379, 20);
            this.virtualPathTextBox.TabIndex = 18;
            this.virtualPathTextBox.TextChanged += new System.EventHandler(this.VirtualPathChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 61);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Virtual Path";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(10, 117);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(380, 1);
            this.panel1.TabIndex = 16;
            // 
            // addDownloadButton
            // 
            this.addDownloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addDownloadButton.Enabled = false;
            this.addDownloadButton.Location = new System.Drawing.Point(295, 125);
            this.addDownloadButton.Margin = new System.Windows.Forms.Padding(2);
            this.addDownloadButton.Name = "addDownloadButton";
            this.addDownloadButton.Size = new System.Drawing.Size(95, 29);
            this.addDownloadButton.TabIndex = 15;
            this.addDownloadButton.Text = "Add Permission";
            this.addDownloadButton.UseVisualStyleBackColor = true;
            this.addDownloadButton.Click += new System.EventHandler(this.addDownloadButton_Click);
            // 
            // AddPermissionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 163);
            this.Controls.Add(this.permissionTypeComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.virtualPathTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.addDownloadButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AddPermissionForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Permission";
            this.Shown += new System.EventHandler(this.OnShown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox permissionTypeComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox virtualPathTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button addDownloadButton;
    }
}