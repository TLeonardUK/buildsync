namespace BuildSync.Core.Controls.Graph
{
    partial class GraphControl
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
			this.refreshTimer = new System.Windows.Forms.Timer(this.components);
			this.mainPanel = new System.Windows.Forms.Panel();
			this.xSeriesMinLabel = new System.Windows.Forms.Label();
			this.xSeriesMaxLabel = new System.Windows.Forms.Label();
			this.ySeriesMaxLabel = new System.Windows.Forms.Label();
			this.graphNameLabel = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// refreshTimer
			// 
			this.refreshTimer.Enabled = true;
			this.refreshTimer.Tick += new System.EventHandler(this.OnTimerTicked);
			// 
			// mainPanel
			// 
			this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mainPanel.Location = new System.Drawing.Point(0, 21);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.Size = new System.Drawing.Size(887, 214);
			this.mainPanel.TabIndex = 9;
			this.mainPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPanelPaint);
			this.mainPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMoved);
			// 
			// xSeriesMinLabel
			// 
			this.xSeriesMinLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.xSeriesMinLabel.ForeColor = System.Drawing.SystemColors.GrayText;
			this.xSeriesMinLabel.Location = new System.Drawing.Point(442, 3);
			this.xSeriesMinLabel.Name = "xSeriesMinLabel";
			this.xSeriesMinLabel.Size = new System.Drawing.Size(433, 13);
			this.xSeriesMinLabel.TabIndex = 8;
			this.xSeriesMinLabel.Text = "0";
			this.xSeriesMinLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// xSeriesMaxLabel
			// 
			this.xSeriesMaxLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.xSeriesMaxLabel.ForeColor = System.Drawing.SystemColors.GrayText;
			this.xSeriesMaxLabel.Location = new System.Drawing.Point(3, 3);
			this.xSeriesMaxLabel.Name = "xSeriesMaxLabel";
			this.xSeriesMaxLabel.Size = new System.Drawing.Size(433, 13);
			this.xSeriesMaxLabel.TabIndex = 7;
			this.xSeriesMaxLabel.Text = "60 seconds";
			// 
			// ySeriesMaxLabel
			// 
			this.ySeriesMaxLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ySeriesMaxLabel.ForeColor = System.Drawing.SystemColors.GrayText;
			this.ySeriesMaxLabel.Location = new System.Drawing.Point(443, 0);
			this.ySeriesMaxLabel.Name = "ySeriesMaxLabel";
			this.ySeriesMaxLabel.Size = new System.Drawing.Size(435, 13);
			this.ySeriesMaxLabel.TabIndex = 6;
			this.ySeriesMaxLabel.Text = "100%";
			this.ySeriesMaxLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// graphNameLabel
			// 
			this.graphNameLabel.ForeColor = System.Drawing.SystemColors.GrayText;
			this.graphNameLabel.Location = new System.Drawing.Point(3, 0);
			this.graphNameLabel.Name = "graphNameLabel";
			this.graphNameLabel.Size = new System.Drawing.Size(434, 13);
			this.graphNameLabel.TabIndex = 5;
			this.graphNameLabel.Text = "Cpu Usage";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.graphNameLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.ySeriesMaxLabel, 1, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 5);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(881, 16);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.Controls.Add(this.xSeriesMaxLabel, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.xSeriesMinLabel, 1, 0);
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 235);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 1;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(878, 16);
			this.tableLayoutPanel2.TabIndex = 0;
			// 
			// GraphControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel2);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.mainPanel);
			this.Name = "GraphControl";
			this.Size = new System.Drawing.Size(887, 255);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer refreshTimer;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Label xSeriesMinLabel;
        private System.Windows.Forms.Label xSeriesMaxLabel;
        private System.Windows.Forms.Label ySeriesMaxLabel;
        private System.Windows.Forms.Label graphNameLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}
