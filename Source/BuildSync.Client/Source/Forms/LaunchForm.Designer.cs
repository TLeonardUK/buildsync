namespace BuildSync.Client.Forms
{
    partial class LaunchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LaunchForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.closeButton = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ModesTreeView = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.ModePropertyGrid = new System.Windows.Forms.PropertyGrid();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Location = new System.Drawing.Point(6, 307);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(587, 1);
            this.panel1.TabIndex = 7;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(487, 318);
            this.closeButton.Margin = new System.Windows.Forms.Padding(2);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(107, 29);
            this.closeButton.TabIndex = 6;
            this.closeButton.Text = "Launch";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.LaunchClicked);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(6, 8);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ModesTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ModePropertyGrid);
            this.splitContainer1.Size = new System.Drawing.Size(587, 294);
            this.splitContainer1.SplitterDistance = 167;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 8;
            // 
            // ModesTreeView
            // 
            this.ModesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModesTreeView.FullRowSelect = true;
            this.ModesTreeView.HideSelection = false;
            this.ModesTreeView.ImageIndex = 0;
            this.ModesTreeView.ImageList = this.imageList;
            this.ModesTreeView.Location = new System.Drawing.Point(0, 0);
            this.ModesTreeView.Margin = new System.Windows.Forms.Padding(2);
            this.ModesTreeView.Name = "ModesTreeView";
            this.ModesTreeView.SelectedImageIndex = 0;
            this.ModesTreeView.ShowLines = false;
            this.ModesTreeView.ShowPlusMinus = false;
            this.ModesTreeView.ShowRootLines = false;
            this.ModesTreeView.Size = new System.Drawing.Size(167, 294);
            this.ModesTreeView.TabIndex = 0;
            this.ModesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.LaunchModeNodeSelected);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "appbar.os.windows.8.png");
            this.imageList.Images.SetKeyName(1, "appbar.social.playstation.png");
            this.imageList.Images.SetKeyName(2, "appbar.xbox.png");
            // 
            // ModePropertyGrid
            // 
            this.ModePropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModePropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.ModePropertyGrid.Margin = new System.Windows.Forms.Padding(2);
            this.ModePropertyGrid.Name = "ModePropertyGrid";
            this.ModePropertyGrid.Size = new System.Drawing.Size(417, 294);
            this.ModePropertyGrid.TabIndex = 0;
            this.ModePropertyGrid.ToolbarVisible = false;
            this.ModePropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PropertyValueChanged);
            // 
            // LaunchForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(601, 357);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.closeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LaunchForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Launch Build";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormHasClosed);
            this.Shown += new System.EventHandler(this.OnShown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView ModesTreeView;
        private System.Windows.Forms.PropertyGrid ModePropertyGrid;
        private System.Windows.Forms.ImageList imageList;
    }
}