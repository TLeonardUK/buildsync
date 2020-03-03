namespace BuildSync.Client.Forms
{
    partial class ManageUsersForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageUsersForm));
            this.MainTreeView = new Aga.Controls.Tree.TreeViewAdv();
            this.userListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.RefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.addMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.userListContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTreeView
            // 
            this.MainTreeView.BackColor = System.Drawing.SystemColors.Window;
            this.MainTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MainTreeView.ContextMenuStrip = this.userListContextMenu;
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
            this.MainTreeView.Size = new System.Drawing.Size(658, 315);
            this.MainTreeView.TabIndex = 2;
            this.MainTreeView.Text = "treeViewAdv1";
            this.MainTreeView.UseColumns = true;
            this.MainTreeView.SelectionChanged += new System.EventHandler(this.SelectionChanged);
            // 
            // userListContextMenu
            // 
            this.userListContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.userListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addMenuItem,
            this.deleteMenuItem});
            this.userListContextMenu.Name = "downloadListContextMenu";
            this.userListContextMenu.Size = new System.Drawing.Size(189, 86);
            // 
            // RefreshTimer
            // 
            this.RefreshTimer.Enabled = true;
            this.RefreshTimer.Interval = 1000;
            this.RefreshTimer.Tick += new System.EventHandler(this.RefreshUserList);
            // 
            // addMenuItem
            // 
            this.addMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_add;
            this.addMenuItem.Name = "addMenuItem";
            this.addMenuItem.Size = new System.Drawing.Size(188, 30);
            this.addMenuItem.Text = "Add User ...";
            this.addMenuItem.Click += new System.EventHandler(this.AddClicked);
            // 
            // deleteMenuItem
            // 
            this.deleteMenuItem.Image = global::BuildSync.Client.Properties.Resources.appbar_minus;
            this.deleteMenuItem.Name = "deleteMenuItem";
            this.deleteMenuItem.Size = new System.Drawing.Size(188, 30);
            this.deleteMenuItem.Text = "Remove ...";
            this.deleteMenuItem.Click += new System.EventHandler(this.DeleteClicked);
            // 
            // ManageUsersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 315);
            this.Controls.Add(this.MainTreeView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManageUsersForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "User Manager";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosed);
            this.Shown += new System.EventHandler(this.OnShown);
            this.userListContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Aga.Controls.Tree.TreeViewAdv MainTreeView;
        private System.Windows.Forms.Timer RefreshTimer;
        private System.Windows.Forms.ContextMenuStrip userListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteMenuItem;
    }
}