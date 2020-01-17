namespace BuildSync.Client.Forms
{
    partial class StatisticsForm
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
            this.SampleTimer = new System.Windows.Forms.Timer(this.components);
            this.StatsTreeView = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.StatPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SampleTimer
            // 
            this.SampleTimer.Enabled = true;
            this.SampleTimer.Interval = 1000;
            this.SampleTimer.Tick += new System.EventHandler(this.GetSamples);
            // 
            // StatsTreeView
            // 
            this.StatsTreeView.CheckBoxes = true;
            this.StatsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StatsTreeView.FullRowSelect = true;
            this.StatsTreeView.Indent = 25;
            this.StatsTreeView.ItemHeight = 25;
            this.StatsTreeView.Location = new System.Drawing.Point(0, 0);
            this.StatsTreeView.Name = "StatsTreeView";
            this.StatsTreeView.Size = new System.Drawing.Size(231, 361);
            this.StatsTreeView.TabIndex = 4;
            this.StatsTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.StatisticCheckChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.StatsTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.StatPanel);
            this.splitContainer1.Size = new System.Drawing.Size(684, 361);
            this.splitContainer1.SplitterDistance = 231;
            this.splitContainer1.TabIndex = 5;
            // 
            // StatPanel
            // 
            this.StatPanel.AutoScroll = true;
            this.StatPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StatPanel.Location = new System.Drawing.Point(0, 0);
            this.StatPanel.Name = "StatPanel";
            this.StatPanel.Padding = new System.Windows.Forms.Padding(5);
            this.StatPanel.Size = new System.Drawing.Size(449, 361);
            this.StatPanel.TabIndex = 0;
            // 
            // StatisticsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 361);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(700, 400);
            this.Name = "StatisticsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Statistics";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.Load += new System.EventHandler(this.OnLoaded);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer SampleTimer;
        private System.Windows.Forms.TreeView StatsTreeView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel StatPanel;
    }
}