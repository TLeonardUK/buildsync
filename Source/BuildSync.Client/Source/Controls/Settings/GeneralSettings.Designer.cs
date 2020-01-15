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
            this.MinimizeToTrayCheckbox.Location = new System.Drawing.Point(16, 45);
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
            this.runInstallWhenLaunchingCheckbox.Location = new System.Drawing.Point(16, 75);
            this.runInstallWhenLaunchingCheckbox.Margin = new System.Windows.Forms.Padding(2);
            this.runInstallWhenLaunchingCheckbox.Name = "runInstallWhenLaunchingCheckbox";
            this.runInstallWhenLaunchingCheckbox.Size = new System.Drawing.Size(446, 17);
            this.runInstallWhenLaunchingCheckbox.TabIndex = 35;
            this.runInstallWhenLaunchingCheckbox.Text = "Always run install before launching build (Only recommended for non-incremental i" +
    "nstalls)?";
            this.runInstallWhenLaunchingCheckbox.UseVisualStyleBackColor = true;
            this.runInstallWhenLaunchingCheckbox.CheckedChanged += new System.EventHandler(this.StateChanged);
            // 
            // skipVerificationCheckBox
            // 
            this.skipVerificationCheckBox.AutoSize = true;
            this.skipVerificationCheckBox.Location = new System.Drawing.Point(16, 105);
            this.skipVerificationCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.skipVerificationCheckBox.Name = "skipVerificationCheckBox";
            this.skipVerificationCheckBox.Size = new System.Drawing.Size(287, 17);
            this.skipVerificationCheckBox.TabIndex = 36;
            this.skipVerificationCheckBox.Text = "Skip validation of builds (Faster but not recommended)?";
            this.skipVerificationCheckBox.UseVisualStyleBackColor = true;
            // 
            // GeneralSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.skipVerificationCheckBox);
            this.Controls.Add(this.runInstallWhenLaunchingCheckbox);
            this.Controls.Add(this.MinimizeToTrayCheckbox);
            this.Controls.Add(this.RunOnStartupCheckbox);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "GeneralSettings";
            this.Size = new System.Drawing.Size(519, 138);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox RunOnStartupCheckbox;
        private System.Windows.Forms.CheckBox MinimizeToTrayCheckbox;
        private System.Windows.Forms.CheckBox runInstallWhenLaunchingCheckbox;
        private System.Windows.Forms.CheckBox skipVerificationCheckBox;
    }
}
