namespace BuildSync.Client.Forms
{
    partial class ProgressForm
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
            this.TaskProgressLabel = new System.Windows.Forms.Label();
            this.TaskProgressBar = new System.Windows.Forms.ProgressBar();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // TaskProgressLabel
            // 
            this.TaskProgressLabel.AutoSize = true;
            this.TaskProgressLabel.Location = new System.Drawing.Point(8, 10);
            this.TaskProgressLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.TaskProgressLabel.Name = "TaskProgressLabel";
            this.TaskProgressLabel.Size = new System.Drawing.Size(57, 13);
            this.TaskProgressLabel.TabIndex = 1;
            this.TaskProgressLabel.Text = "Build Sync";
            // 
            // TaskProgressBar
            // 
            this.TaskProgressBar.Location = new System.Drawing.Point(9, 25);
            this.TaskProgressBar.Margin = new System.Windows.Forms.Padding(2);
            this.TaskProgressBar.Name = "TaskProgressBar";
            this.TaskProgressBar.Size = new System.Drawing.Size(443, 15);
            this.TaskProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.TaskProgressBar.TabIndex = 2;
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Enabled = true;
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimerTick);
            // 
            // ProgressForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(460, 55);
            this.Controls.Add(this.TaskProgressBar);
            this.Controls.Add(this.TaskProgressLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ProgressForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Installing ....";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClosingRequested);
            this.Load += new System.EventHandler(this.FormLoaded);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TaskProgressLabel;
        private System.Windows.Forms.ProgressBar TaskProgressBar;
        private System.Windows.Forms.Timer UpdateTimer;
    }
}