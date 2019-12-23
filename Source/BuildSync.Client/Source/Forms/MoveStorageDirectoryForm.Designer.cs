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
            this.TaskProgressLabel = new System.Windows.Forms.Label();
            this.TaskProgressBar = new System.Windows.Forms.ProgressBar();
            this.TotalProgressBar = new System.Windows.Forms.ProgressBar();
            this.TotalProgressLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TaskProgressLabel
            // 
            this.TaskProgressLabel.AutoSize = true;
            this.TaskProgressLabel.Location = new System.Drawing.Point(12, 15);
            this.TaskProgressLabel.Name = "TaskProgressLabel";
            this.TaskProgressLabel.Size = new System.Drawing.Size(83, 20);
            this.TaskProgressLabel.TabIndex = 1;
            this.TaskProgressLabel.Text = "Build Sync";
            // 
            // TaskProgressBar
            // 
            this.TaskProgressBar.Location = new System.Drawing.Point(13, 39);
            this.TaskProgressBar.Name = "TaskProgressBar";
            this.TaskProgressBar.Size = new System.Drawing.Size(665, 23);
            this.TaskProgressBar.TabIndex = 2;
            // 
            // TotalProgressBar
            // 
            this.TotalProgressBar.Location = new System.Drawing.Point(13, 100);
            this.TotalProgressBar.Name = "TotalProgressBar";
            this.TotalProgressBar.Size = new System.Drawing.Size(665, 23);
            this.TotalProgressBar.TabIndex = 4;
            // 
            // TotalProgressLabel
            // 
            this.TotalProgressLabel.AutoSize = true;
            this.TotalProgressLabel.Location = new System.Drawing.Point(12, 76);
            this.TotalProgressLabel.Name = "TotalProgressLabel";
            this.TotalProgressLabel.Size = new System.Drawing.Size(111, 20);
            this.TotalProgressLabel.TabIndex = 3;
            this.TotalProgressLabel.Text = "Total Progress";
            // 
            // MoveStorageDirectoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 145);
            this.Controls.Add(this.TotalProgressBar);
            this.Controls.Add(this.TotalProgressLabel);
            this.Controls.Add(this.TaskProgressBar);
            this.Controls.Add(this.TaskProgressLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
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
    }
}