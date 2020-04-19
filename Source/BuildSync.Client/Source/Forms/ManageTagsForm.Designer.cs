namespace BuildSync.Client.Forms
{
    partial class ManageTagsForm
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
            this.MainTreeView = new Aga.Controls.Tree.TreeViewAdv();
            this.downloadListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.downloadListContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTreeView
            // 
            this.MainTreeView.BackColor = System.Drawing.SystemColors.Window;
            this.MainTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.MainTreeView.ContextMenuStrip = this.downloadListContextMenu;
            this.MainTreeView.DefaultToolTipProvider = null;
            this.MainTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainTreeView.DragDropMarkColor = System.Drawing.Color.Black;
            this.MainTreeView.FullRowSelect = true;
            this.MainTreeView.LineColor = System.Drawing.SystemColors.ControlDark;
            this.MainTreeView.Location = new System.Drawing.Point(0, 0);
            this.MainTreeView.Model = null;
            this.MainTreeView.Name = "MainTreeView";
            this.MainTreeView.RowHeight = 22;
            this.MainTreeView.SelectedNode = null;
            this.MainTreeView.Size = new System.Drawing.Size(800, 450);
            this.MainTreeView.TabIndex = 0;
            this.MainTreeView.Text = "treeViewAdv1";
            this.MainTreeView.UseColumns = true;
            this.MainTreeView.SelectionChanged += new System.EventHandler(this.OnSelectedChanged);
            // 
            // downloadListContextMenu
            // 
            this.downloadListContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.downloadListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addTagToolStripMenuItem,
            this.toolStripSeparator9,
            this.deleteToolStripMenuItem});
            this.downloadListContextMenu.Name = "downloadListContextMenu";
            this.downloadListContextMenu.Size = new System.Drawing.Size(138, 70);
            // 
            // addTagToolStripMenuItem
            // 
            this.addTagToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_add;
            this.addTagToolStripMenuItem.Name = "addTagToolStripMenuItem";
            this.addTagToolStripMenuItem.Size = new System.Drawing.Size(137, 30);
            this.addTagToolStripMenuItem.Text = "Add Tag ...";
            this.addTagToolStripMenuItem.Click += new System.EventHandler(this.AddClicked);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(134, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_delete;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(137, 30);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteClicked);
            // 
            // RefreshTimer
            // 
            this.RefreshTimer.Enabled = true;
            this.RefreshTimer.Interval = 1000;
            this.RefreshTimer.Tick += new System.EventHandler(this.RefreshTagList);
            // 
            // ManageTagsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.MainTreeView);
            this.HideOnClose = true;
            this.Name = "ManageTagsForm";
            this.Text = "Tag Manager";
            this.Shown += new System.EventHandler(this.OnShown);
            this.downloadListContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Aga.Controls.Tree.TreeViewAdv MainTreeView;
        private System.Windows.Forms.ContextMenuStrip downloadListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.Timer RefreshTimer;
	}
}