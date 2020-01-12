namespace BuildSync.Client.Forms
{
    partial class AddScmWorkspaceForm
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
            this.ServerNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AddButton = new System.Windows.Forms.Button();
            this.UsernameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.WorkspaceTypeComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // LocalFolderBrowseButton
            // 
            this.LocalFolderBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LocalFolderBrowseButton.Location = new System.Drawing.Point(443, 146);
            this.LocalFolderBrowseButton.Margin = new System.Windows.Forms.Padding(2);
            this.LocalFolderBrowseButton.Name = "LocalFolderBrowseButton";
            this.LocalFolderBrowseButton.Size = new System.Drawing.Size(88, 20);
            this.LocalFolderBrowseButton.TabIndex = 5;
            this.LocalFolderBrowseButton.Text = "Browse";
            this.LocalFolderBrowseButton.UseVisualStyleBackColor = true;
            this.LocalFolderBrowseButton.Click += new System.EventHandler(this.BrowseClicked);
            // 
            // LocalFolderTextBox
            // 
            this.LocalFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LocalFolderTextBox.Location = new System.Drawing.Point(9, 146);
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
            this.label3.Location = new System.Drawing.Point(8, 131);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "Local Folder";
            // 
            // ServerNameTextBox
            // 
            this.ServerNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ServerNameTextBox.Location = new System.Drawing.Point(9, 67);
            this.ServerNameTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.ServerNameTextBox.Name = "ServerNameTextBox";
            this.ServerNameTextBox.Size = new System.Drawing.Size(522, 20);
            this.ServerNameTextBox.TabIndex = 1;
            this.ServerNameTextBox.TextChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 52);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Server Address";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(9, 181);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(523, 1);
            this.panel1.TabIndex = 28;
            // 
            // AddButton
            // 
            this.AddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddButton.Enabled = false;
            this.AddButton.Location = new System.Drawing.Point(396, 193);
            this.AddButton.Margin = new System.Windows.Forms.Padding(2);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(135, 29);
            this.AddButton.TabIndex = 6;
            this.AddButton.Text = "Add Workspace";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddClicked);
            // 
            // UsernameTextBox
            // 
            this.UsernameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UsernameTextBox.Location = new System.Drawing.Point(9, 106);
            this.UsernameTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.UsernameTextBox.Name = "UsernameTextBox";
            this.UsernameTextBox.Size = new System.Drawing.Size(250, 20);
            this.UsernameTextBox.TabIndex = 2;
            this.UsernameTextBox.TextChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 91);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = "Username";
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PasswordTextBox.Location = new System.Drawing.Point(263, 106);
            this.PasswordTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.Size = new System.Drawing.Size(268, 20);
            this.PasswordTextBox.TabIndex = 3;
            this.PasswordTextBox.UseSystemPasswordChar = true;
            this.PasswordTextBox.TextChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(260, 91);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 36;
            this.label4.Text = "Password";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 10);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 38;
            this.label5.Text = "Type";
            // 
            // WorkspaceTypeComboBox
            // 
            this.WorkspaceTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WorkspaceTypeComboBox.FormattingEnabled = true;
            this.WorkspaceTypeComboBox.Items.AddRange(new object[] {
            "Perforce,",
            "Git"});
            this.WorkspaceTypeComboBox.Location = new System.Drawing.Point(9, 27);
            this.WorkspaceTypeComboBox.Name = "WorkspaceTypeComboBox";
            this.WorkspaceTypeComboBox.Size = new System.Drawing.Size(522, 21);
            this.WorkspaceTypeComboBox.TabIndex = 0;
            this.WorkspaceTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.DataStateChanged);
            // 
            // AddScmWorkspaceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 234);
            this.Controls.Add(this.WorkspaceTypeComboBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.PasswordTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.UsernameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.LocalFolderBrowseButton);
            this.Controls.Add(this.LocalFolderTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ServerNameTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.AddButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddScmWorkspaceForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add SCM Workspace";
            this.Load += new System.EventHandler(this.OnLoaded);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LocalFolderBrowseButton;
        private System.Windows.Forms.TextBox LocalFolderTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ServerNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.TextBox UsernameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox WorkspaceTypeComboBox;
    }
}