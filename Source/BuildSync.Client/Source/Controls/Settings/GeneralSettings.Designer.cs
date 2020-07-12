namespace BuildSync.Client.Controls.Settings
{
    partial class GeneralSettings
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
            this.RunOnStartupCheckbox = new System.Windows.Forms.CheckBox();
            this.MinimizeToTrayCheckbox = new System.Windows.Forms.CheckBox();
            this.runInstallWhenLaunchingCheckbox = new System.Windows.Forms.CheckBox();
            this.skipVerificationCheckBox = new System.Windows.Forms.CheckBox();
            this.skipInitialization = new System.Windows.Forms.CheckBox();
            this.showInternalDownloadsCheckBox = new System.Windows.Forms.CheckBox();
            this.logLevelComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.viewLogsButton = new System.Windows.Forms.Button();
            this.autoFixValidationErrorsCheckBox = new System.Windows.Forms.CheckBox();
            this.allowRemoteActionsCheckBox = new System.Windows.Forms.CheckBox();
            this.autoInstallUpdatesCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // RunOnStartupCheckbox
            // 
            this.RunOnStartupCheckbox.AutoSize = true;
            this.RunOnStartupCheckbox.Location = new System.Drawing.Point(16, 15);
            this.RunOnStartupCheckbox.Margin = new System.Windows.Forms.Padding(2);
            this.RunOnStartupCheckbox.Name = "RunOnStartupCheckbox";
            this.RunOnStartupCheckbox.Size = new System.Drawing.Size(156, 17);
            this.RunOnStartupCheckbox.TabIndex = 33;
            this.RunOnStartupCheckbox.Text = "Run when computer starts?";
            this.RunOnStartupCheckbox.UseVisualStyleBackColor = true;
            this.RunOnStartupCheckbox.CheckedChanged += new System.EventHandler(this.StateChanged);
            // 
            // MinimizeToTrayCheckbox
            // 
            this.MinimizeToTrayCheckbox.AutoSize = true;
            this.MinimizeToTrayCheckbox.Location = new System.Drawing.Point(16, 44);
            this.MinimizeToTrayCheckbox.Margin = new System.Windows.Forms.Padding(2);
            this.MinimizeToTrayCheckbox.Name = "MinimizeToTrayCheckbox";
            this.MinimizeToTrayCheckbox.Size = new System.Drawing.Size(147, 17);
            this.MinimizeToTrayCheckbox.TabIndex = 34;
            this.MinimizeToTrayCheckbox.Text = "Minimize to tray on close?";
            this.MinimizeToTrayCheckbox.UseVisualStyleBackColor = true;
            this.MinimizeToTrayCheckbox.CheckedChanged += new System.EventHandler(this.StateChanged);
            // 
            // runInstallWhenLaunchingCheckbox
            // 
            this.runInstallWhenLaunchingCheckbox.AutoSize = true;
            this.runInstallWhenLaunchingCheckbox.Location = new System.Drawing.Point(16, 73);
            this.runInstallWhenLaunchingCheckbox.Margin = new System.Windows.Forms.Padding(2);
            this.runInstallWhenLaunchingCheckbox.Name = "runInstallWhenLaunchingCheckbox";
            this.runInstallWhenLaunchingCheckbox.Size = new System.Drawing.Size(494, 17);
            this.runInstallWhenLaunchingCheckbox.TabIndex = 35;
            this.runInstallWhenLaunchingCheckbox.Text = "Always run install before launching build? (Only recommended to disable for non-i" +
    "ncremental installs)\r\n";
            this.runInstallWhenLaunchingCheckbox.UseVisualStyleBackColor = true;
            this.runInstallWhenLaunchingCheckbox.CheckedChanged += new System.EventHandler(this.StateChanged);
            // 
            // skipVerificationCheckBox
            // 
            this.skipVerificationCheckBox.AutoSize = true;
            this.skipVerificationCheckBox.Location = new System.Drawing.Point(16, 102);
            this.skipVerificationCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.skipVerificationCheckBox.Name = "skipVerificationCheckBox";
            this.skipVerificationCheckBox.Size = new System.Drawing.Size(358, 17);
            this.skipVerificationCheckBox.TabIndex = 36;
            this.skipVerificationCheckBox.Text = "Skip validation of builds? (Faster but may not catch any corrupted files)";
            this.skipVerificationCheckBox.UseVisualStyleBackColor = true;
            this.skipVerificationCheckBox.CheckedChanged += new System.EventHandler(this.StateChanged);
            // 
            // skipInitialization
            // 
            this.skipInitialization.AutoSize = true;
            this.skipInitialization.Location = new System.Drawing.Point(16, 131);
            this.skipInitialization.Margin = new System.Windows.Forms.Padding(2);
            this.skipInitialization.Name = "skipInitialization";
            this.skipInitialization.Size = new System.Drawing.Size(444, 17);
            this.skipInitialization.TabIndex = 37;
            this.skipInitialization.Text = "Skip pre-allocation of disk space? (May be faster, but will result in less consis" +
    "tent speeds)";
            this.skipInitialization.UseVisualStyleBackColor = true;
            this.skipInitialization.CheckedChanged += new System.EventHandler(this.StateChanged);
            // 
            // showInternalDownloadsCheckBox
            // 
            this.showInternalDownloadsCheckBox.AutoSize = true;
            this.showInternalDownloadsCheckBox.Location = new System.Drawing.Point(16, 160);
            this.showInternalDownloadsCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.showInternalDownloadsCheckBox.Name = "showInternalDownloadsCheckBox";
            this.showInternalDownloadsCheckBox.Size = new System.Drawing.Size(150, 17);
            this.showInternalDownloadsCheckBox.TabIndex = 38;
            this.showInternalDownloadsCheckBox.Text = "Show internal downloads?";
            this.showInternalDownloadsCheckBox.UseVisualStyleBackColor = true;
            this.showInternalDownloadsCheckBox.CheckedChanged += new System.EventHandler(this.StateChanged);
            // 
            // logLevelComboBox
            // 
            this.logLevelComboBox.FormattingEnabled = true;
            this.logLevelComboBox.Location = new System.Drawing.Point(16, 295);
            this.logLevelComboBox.Name = "logLevelComboBox";
            this.logLevelComboBox.Size = new System.Drawing.Size(367, 21);
            this.logLevelComboBox.TabIndex = 39;
            this.logLevelComboBox.SelectedIndexChanged += new System.EventHandler(this.StateChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 276);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 40;
            this.label1.Text = "Logging Level";
            // 
            // viewLogsButton
            // 
            this.viewLogsButton.Location = new System.Drawing.Point(390, 295);
            this.viewLogsButton.Name = "viewLogsButton";
            this.viewLogsButton.Size = new System.Drawing.Size(120, 21);
            this.viewLogsButton.TabIndex = 41;
            this.viewLogsButton.Text = "View Logs";
            this.viewLogsButton.UseVisualStyleBackColor = true;
            this.viewLogsButton.Click += new System.EventHandler(this.ViewLogsClicked);
            // 
            // autoFixValidationErrorsCheckBox
            // 
            this.autoFixValidationErrorsCheckBox.AutoSize = true;
            this.autoFixValidationErrorsCheckBox.Location = new System.Drawing.Point(16, 189);
            this.autoFixValidationErrorsCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.autoFixValidationErrorsCheckBox.Name = "autoFixValidationErrorsCheckBox";
            this.autoFixValidationErrorsCheckBox.Size = new System.Drawing.Size(184, 17);
            this.autoFixValidationErrorsCheckBox.TabIndex = 42;
            this.autoFixValidationErrorsCheckBox.Text = "Automatically fix validation errors?";
            this.autoFixValidationErrorsCheckBox.UseVisualStyleBackColor = true;
            this.autoFixValidationErrorsCheckBox.CheckedChanged += new System.EventHandler(this.StateChanged);
            // 
            // allowRemoteActionsCheckBox
            // 
            this.allowRemoteActionsCheckBox.AutoSize = true;
            this.allowRemoteActionsCheckBox.Location = new System.Drawing.Point(16, 218);
            this.allowRemoteActionsCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.allowRemoteActionsCheckBox.Name = "allowRemoteActionsCheckBox";
            this.allowRemoteActionsCheckBox.Size = new System.Drawing.Size(234, 17);
            this.allowRemoteActionsCheckBox.TabIndex = 43;
            this.allowRemoteActionsCheckBox.Text = "Allow server to run remote actions on client?";
            this.allowRemoteActionsCheckBox.UseVisualStyleBackColor = true;
            this.allowRemoteActionsCheckBox.CheckedChanged += new System.EventHandler(this.StateChanged);
            // 
            // autoInstallUpdatesCheckBox
            // 
            this.autoInstallUpdatesCheckBox.AutoSize = true;
            this.autoInstallUpdatesCheckBox.Location = new System.Drawing.Point(16, 247);
            this.autoInstallUpdatesCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.autoInstallUpdatesCheckBox.Name = "autoInstallUpdatesCheckBox";
            this.autoInstallUpdatesCheckBox.Size = new System.Drawing.Size(124, 17);
            this.autoInstallUpdatesCheckBox.TabIndex = 44;
            this.autoInstallUpdatesCheckBox.Text = "Auto install updates?";
            this.autoInstallUpdatesCheckBox.UseVisualStyleBackColor = true;
            this.autoInstallUpdatesCheckBox.CheckedChanged += new System.EventHandler(this.StateChanged);
            // 
            // GeneralSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.autoInstallUpdatesCheckBox);
            this.Controls.Add(this.allowRemoteActionsCheckBox);
            this.Controls.Add(this.autoFixValidationErrorsCheckBox);
            this.Controls.Add(this.viewLogsButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.logLevelComboBox);
            this.Controls.Add(this.showInternalDownloadsCheckBox);
            this.Controls.Add(this.skipInitialization);
            this.Controls.Add(this.skipVerificationCheckBox);
            this.Controls.Add(this.runInstallWhenLaunchingCheckbox);
            this.Controls.Add(this.MinimizeToTrayCheckbox);
            this.Controls.Add(this.RunOnStartupCheckbox);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "GeneralSettings";
            this.Size = new System.Drawing.Size(519, 333);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox RunOnStartupCheckbox;
        private System.Windows.Forms.CheckBox MinimizeToTrayCheckbox;
        private System.Windows.Forms.CheckBox runInstallWhenLaunchingCheckbox;
        private System.Windows.Forms.CheckBox skipVerificationCheckBox;
        private System.Windows.Forms.CheckBox skipInitialization;
        private System.Windows.Forms.CheckBox showInternalDownloadsCheckBox;
		private System.Windows.Forms.ComboBox logLevelComboBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button viewLogsButton;
        private System.Windows.Forms.CheckBox autoFixValidationErrorsCheckBox;
		private System.Windows.Forms.CheckBox allowRemoteActionsCheckBox;
        private System.Windows.Forms.CheckBox autoInstallUpdatesCheckBox;
    }
}
