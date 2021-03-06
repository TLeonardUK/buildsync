﻿namespace BuildSync.Client.Controls
{
    partial class ManifestDeltaTree
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManifestDeltaTree));
            this.TreeImageList = new System.Windows.Forms.ImageList(this.components);
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.MainTreeView = new Aga.Controls.Tree.TreeViewAdv();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // TreeImageList
            // 
            this.TreeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("TreeImageList.ImageStream")));
            this.TreeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.TreeImageList.Images.SetKeyName(0, "appbar.box.png");
            this.TreeImageList.Images.SetKeyName(1, "appbar.folder.png");
            this.TreeImageList.Images.SetKeyName(2, "appbar.folder.open.png");
            this.TreeImageList.Images.SetKeyName(3, "appbar.database.png");
            this.TreeImageList.Images.SetKeyName(4, "appbar.refresh.png");
            // 
            // pathTextBox
            // 
            this.pathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.pathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pathTextBox.Location = new System.Drawing.Point(30, 2);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(415, 20);
            this.pathTextBox.TabIndex = 8;
            // 
            // MainTreeView
            // 
            this.MainTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainTreeView.BackColor = System.Drawing.SystemColors.Window;
            this.MainTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MainTreeView.DefaultToolTipProvider = null;
            this.MainTreeView.DragDropMarkColor = System.Drawing.Color.Black;
            this.MainTreeView.FullRowSelect = true;
            this.MainTreeView.LineColor = System.Drawing.SystemColors.ControlDark;
            this.MainTreeView.Location = new System.Drawing.Point(0, 23);
            this.MainTreeView.Model = null;
            this.MainTreeView.Name = "MainTreeView";
            this.MainTreeView.RowHeight = 22;
            this.MainTreeView.SelectedNode = null;
            this.MainTreeView.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.Multi;
            this.MainTreeView.Size = new System.Drawing.Size(445, 175);
            this.MainTreeView.TabIndex = 7;
            this.MainTreeView.Text = "treeViewAdv1";
            this.MainTreeView.UseColumns = true;
            this.MainTreeView.SelectionChanged += new System.EventHandler(this.OnSelectionChanged);
            this.MainTreeView.Collapsed += new System.EventHandler<Aga.Controls.Tree.TreeViewAdvEventArgs>(this.NodeCollapsed);
            this.MainTreeView.Expanded += new System.EventHandler<Aga.Controls.Tree.TreeViewAdvEventArgs>(this.NodeExpanded);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::BuildSync.Client.Properties.Resources.appbar_folder_open;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(0, -2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(28, 28);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // ManifestDeltaTree
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.pathTextBox);
            this.Controls.Add(this.MainTreeView);
            this.Controls.Add(this.pictureBox1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ManifestDeltaTree";
            this.Size = new System.Drawing.Size(445, 198);
            this.Load += new System.EventHandler(this.OnLoaded);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ImageList TreeImageList;
        private System.Windows.Forms.TextBox pathTextBox;
        private Aga.Controls.Tree.TreeViewAdv MainTreeView;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
