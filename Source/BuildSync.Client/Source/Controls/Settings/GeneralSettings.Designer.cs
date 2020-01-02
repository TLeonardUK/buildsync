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
            this.SuspendLayout();
            // 
            // RunOnStartupCheckbox
            // 
            this.RunOnStartupCheckbox.AutoSize = true;
            this.RunOnStartupCheckbox.Location = new System.Drawing.Point(16, 17);
            this.RunOnStartupCheckbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.MinimizeToTrayCheckbox.Location = new System.Drawing.Point(16, 52);
            this.MinimizeToTrayCheckbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MinimizeToTrayCheckbox.Name = "MinimizeToTrayCheckbox";
            this.MinimizeToTrayCheckbox.Size = new System.Drawing.Size(147, 17);
            this.MinimizeToTrayCheckbox.TabIndex = 34;
            this.MinimizeToTrayCheckbox.Text = "Minimize to tray on close?";
            this.MinimizeToTrayCheckbox.UseVisualStyleBackColor = true;
            this.MinimizeToTrayCheckbox.CheckedChanged += new System.EventHandler(this.StateChanged);
            // 
            // GeneralSettings
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.MinimizeToTrayCheckbox);
            this.Controls.Add(this.RunOnStartupCheckbox);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "GeneralSettings";
            this.Size = new System.Drawing.Size(519, 81);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox RunOnStartupCheckbox;
        private System.Windows.Forms.CheckBox MinimizeToTrayCheckbox;
    }
}
