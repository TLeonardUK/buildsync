namespace BuildSync.Client.Controls
{
    partial class DownloadList
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadList));
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.EmptyPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.AddDownloadButton = new System.Windows.Forms.Button();
            this.EmptyPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Enabled = true;
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimerTick);
            // 
            // EmptyPanel
            // 
            this.EmptyPanel.Controls.Add(this.AddDownloadButton);
            this.EmptyPanel.Controls.Add(this.label1);
            this.EmptyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EmptyPanel.Location = new System.Drawing.Point(0, 0);
            this.EmptyPanel.Name = "EmptyPanel";
            this.EmptyPanel.Size = new System.Drawing.Size(517, 135);
            this.EmptyPanel.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(514, 27);
            this.label1.TabIndex = 0;
            this.label1.Text = "Looking pretty empty ... Why not add a new download?";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AddDownloadButton
            // 
            this.AddDownloadButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AddDownloadButton.Location = new System.Drawing.Point(190, 77);
            this.AddDownloadButton.Name = "AddDownloadButton";
            this.AddDownloadButton.Size = new System.Drawing.Size(137, 23);
            this.AddDownloadButton.TabIndex = 1;
            this.AddDownloadButton.Text = "Add New Download";
            this.AddDownloadButton.UseVisualStyleBackColor = true;
            this.AddDownloadButton.Click += new System.EventHandler(this.AddDownloadClicked);
            // 
            // DownloadList
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(517, 135);
            this.Controls.Add(this.EmptyPanel);
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(13);
            this.Name = "DownloadList";
            this.ShowIcon = false;
            this.Text = "Downloads";
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MouseClicked);
            this.EmptyPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.Panel EmptyPanel;
        private System.Windows.Forms.Button AddDownloadButton;
        private System.Windows.Forms.Label label1;
    }
}
