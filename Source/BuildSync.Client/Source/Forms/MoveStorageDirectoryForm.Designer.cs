namespace BuildSync.Client.Forms
{
    partial class MoveStorageDirectoryForm
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
            this.TotalProgressBar = new System.Windows.Forms.ProgressBar();
            this.TotalProgressLabel = new System.Windows.Forms.Label();
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
            this.TaskProgressBar.Location = new System.Drawing.Point(11, 24);
            this.TaskProgressBar.Margin = new System.Windows.Forms.Padding(2);
            this.TaskProgressBar.Name = "TaskProgressBar";
            this.TaskProgressBar.Size = new System.Drawing.Size(633, 15);
            this.TaskProgressBar.TabIndex = 2;
            // 
            // TotalProgressBar
            // 
            this.TotalProgressBar.Location = new System.Drawing.Point(11, 64);
            this.TotalProgressBar.Margin = new System.Windows.Forms.Padding(2);
            this.TotalProgressBar.Name = "TotalProgressBar";
            this.TotalProgressBar.Size = new System.Drawing.Size(633, 15);
            this.TotalProgressBar.TabIndex = 4;
            // 
            // TotalProgressLabel
            // 
            this.TotalProgressLabel.AutoSize = true;
            this.TotalProgressLabel.Location = new System.Drawing.Point(8, 49);
            this.TotalProgressLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.TotalProgressLabel.Name = "TotalProgressLabel";
            this.TotalProgressLabel.Size = new System.Drawing.Size(75, 13);
            this.TotalProgressLabel.TabIndex = 3;
            this.TotalProgressLabel.Text = "Total Progress";
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Enabled = true;
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimerTick);
            // 
            // MoveStorageDirectoryForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(655, 94);
            this.Controls.Add(this.TotalProgressBar);
            this.Controls.Add(this.TotalProgressLabel);
            this.Controls.Add(this.TaskProgressBar);
            this.Controls.Add(this.TaskProgressLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MoveStorageDirectoryForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Moving Storage Directory ...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClosingRequested);
            this.Load += new System.EventHandler(this.FormLoaded);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TaskProgressLabel;
        private System.Windows.Forms.ProgressBar TaskProgressBar;
        private System.Windows.Forms.ProgressBar TotalProgressBar;
        private System.Windows.Forms.Label TotalProgressLabel;
        private System.Windows.Forms.Timer UpdateTimer;
    }
}