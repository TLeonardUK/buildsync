/*
  buildsync
  Copyright (C) 2020 Tim Leonard <me@timleonard.uk>

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.
  
  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

namespace BuildSync.Client.Forms
{
    partial class DiffManifestForm
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
            this.AvailabilityUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.SourceManifestDeltaTree = new BuildSync.Client.Controls.ManifestDeltaTree();
            this.DestinationManifestDeltaTree = new BuildSync.Client.Controls.ManifestDeltaTree();
            this.label1 = new System.Windows.Forms.Label();
            this.pathLabel = new System.Windows.Forms.Label();
            this.blockLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checksumLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.manifestGuidLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.manifestBlocksLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.manifestSizeLabel = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.matchingFilesLabel = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.stateLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.matchingBlocksLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // AvailabilityUpdateTimer
            // 
            this.AvailabilityUpdateTimer.Enabled = true;
            this.AvailabilityUpdateTimer.Interval = 1000;
            this.AvailabilityUpdateTimer.Tick += new System.EventHandler(this.UpdateTicked);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.matchingBlocksLabel);
            this.splitContainer1.Panel2.Controls.Add(this.label8);
            this.splitContainer1.Panel2.Controls.Add(this.stateLabel);
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Panel2.Controls.Add(this.matchingFilesLabel);
            this.splitContainer1.Panel2.Controls.Add(this.label11);
            this.splitContainer1.Panel2.Controls.Add(this.manifestGuidLabel);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.manifestBlocksLabel);
            this.splitContainer1.Panel2.Controls.Add(this.label7);
            this.splitContainer1.Panel2.Controls.Add(this.manifestSizeLabel);
            this.splitContainer1.Panel2.Controls.Add(this.label9);
            this.splitContainer1.Panel2.Controls.Add(this.checksumLabel);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.blockLabel);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.pathLabel);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(1146, 574);
            this.splitContainer1.SplitterDistance = 469;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.SourceManifestDeltaTree);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.DestinationManifestDeltaTree);
            this.splitContainer2.Size = new System.Drawing.Size(1146, 469);
            this.splitContainer2.SplitterDistance = 571;
            this.splitContainer2.TabIndex = 1;
            // 
            // SourceManifestDeltaTree
            // 
            this.SourceManifestDeltaTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SourceManifestDeltaTree.Location = new System.Drawing.Point(0, 0);
            this.SourceManifestDeltaTree.Margin = new System.Windows.Forms.Padding(2);
            this.SourceManifestDeltaTree.Name = "SourceManifestDeltaTree";
            this.SourceManifestDeltaTree.Size = new System.Drawing.Size(571, 469);
            this.SourceManifestDeltaTree.TabIndex = 0;
            this.SourceManifestDeltaTree.OnSelectedNodeChanged += new BuildSync.Client.Controls.DeltaTreeNodeChanged(this.SelectedNodeChanged);
            this.SourceManifestDeltaTree.OnNodeExpanded += new BuildSync.Client.Controls.DeltaTreeNodeExpanded(this.NodeExpanded);
            this.SourceManifestDeltaTree.OnNodeCollapsed += new BuildSync.Client.Controls.DeltaTreeNodeCollapsed(this.NodeCollapsed);
            // 
            // DestinationManifestDeltaTree
            // 
            this.DestinationManifestDeltaTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DestinationManifestDeltaTree.Location = new System.Drawing.Point(0, 0);
            this.DestinationManifestDeltaTree.Margin = new System.Windows.Forms.Padding(2);
            this.DestinationManifestDeltaTree.Name = "DestinationManifestDeltaTree";
            this.DestinationManifestDeltaTree.Size = new System.Drawing.Size(571, 469);
            this.DestinationManifestDeltaTree.TabIndex = 1;
            this.DestinationManifestDeltaTree.OnSelectedNodeChanged += new BuildSync.Client.Controls.DeltaTreeNodeChanged(this.SelectedNodeChanged);
            this.DestinationManifestDeltaTree.OnNodeExpanded += new BuildSync.Client.Controls.DeltaTreeNodeExpanded(this.NodeExpanded);
            this.DestinationManifestDeltaTree.OnNodeCollapsed += new BuildSync.Client.Controls.DeltaTreeNodeCollapsed(this.NodeCollapsed);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Path:";
            // 
            // pathLabel
            // 
            this.pathLabel.AutoSize = true;
            this.pathLabel.Location = new System.Drawing.Point(76, 9);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(16, 13);
            this.pathLabel.TabIndex = 1;
            this.pathLabel.Text = "...";
            // 
            // blockLabel
            // 
            this.blockLabel.AutoSize = true;
            this.blockLabel.Location = new System.Drawing.Point(76, 26);
            this.blockLabel.Name = "blockLabel";
            this.blockLabel.Size = new System.Drawing.Size(16, 13);
            this.blockLabel.TabIndex = 3;
            this.blockLabel.Text = "...";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Blocks:";
            // 
            // checksumLabel
            // 
            this.checksumLabel.AutoSize = true;
            this.checksumLabel.Location = new System.Drawing.Point(76, 43);
            this.checksumLabel.Name = "checksumLabel";
            this.checksumLabel.Size = new System.Drawing.Size(16, 13);
            this.checksumLabel.TabIndex = 5;
            this.checksumLabel.Text = "...";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Hash:";
            // 
            // manifestGuidLabel
            // 
            this.manifestGuidLabel.AutoSize = true;
            this.manifestGuidLabel.Location = new System.Drawing.Point(687, 43);
            this.manifestGuidLabel.Name = "manifestGuidLabel";
            this.manifestGuidLabel.Size = new System.Drawing.Size(16, 13);
            this.manifestGuidLabel.TabIndex = 11;
            this.manifestGuidLabel.Text = "...";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(578, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Manifest Guid:";
            // 
            // manifestBlocksLabel
            // 
            this.manifestBlocksLabel.AutoSize = true;
            this.manifestBlocksLabel.Location = new System.Drawing.Point(687, 26);
            this.manifestBlocksLabel.Name = "manifestBlocksLabel";
            this.manifestBlocksLabel.Size = new System.Drawing.Size(16, 13);
            this.manifestBlocksLabel.TabIndex = 9;
            this.manifestBlocksLabel.Text = "...";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(578, 26);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Manifest Blocks:";
            // 
            // manifestSizeLabel
            // 
            this.manifestSizeLabel.AutoSize = true;
            this.manifestSizeLabel.Location = new System.Drawing.Point(687, 9);
            this.manifestSizeLabel.Name = "manifestSizeLabel";
            this.manifestSizeLabel.Size = new System.Drawing.Size(16, 13);
            this.manifestSizeLabel.TabIndex = 7;
            this.manifestSizeLabel.Text = "...";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(578, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(73, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Manifest Size:";
            // 
            // matchingFilesLabel
            // 
            this.matchingFilesLabel.AutoSize = true;
            this.matchingFilesLabel.Location = new System.Drawing.Point(687, 60);
            this.matchingFilesLabel.Name = "matchingFilesLabel";
            this.matchingFilesLabel.Size = new System.Drawing.Size(16, 13);
            this.matchingFilesLabel.TabIndex = 13;
            this.matchingFilesLabel.Text = "...";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(578, 60);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(78, 13);
            this.label11.TabIndex = 12;
            this.label11.Text = "Matching Files:";
            // 
            // stateLabel
            // 
            this.stateLabel.AutoSize = true;
            this.stateLabel.Location = new System.Drawing.Point(76, 60);
            this.stateLabel.Name = "stateLabel";
            this.stateLabel.Size = new System.Drawing.Size(16, 13);
            this.stateLabel.TabIndex = 15;
            this.stateLabel.Text = "...";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "State:";
            // 
            // matchingBlocksLabel
            // 
            this.matchingBlocksLabel.AutoSize = true;
            this.matchingBlocksLabel.Location = new System.Drawing.Point(687, 77);
            this.matchingBlocksLabel.Name = "matchingBlocksLabel";
            this.matchingBlocksLabel.Size = new System.Drawing.Size(16, 13);
            this.matchingBlocksLabel.TabIndex = 17;
            this.matchingBlocksLabel.Text = "...";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(578, 77);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Matching Blocks:";
            // 
            // DiffManifestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1146, 574);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DiffManifestForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manifest Diff";
            this.Load += new System.EventHandler(this.OnLoaded);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer AvailabilityUpdateTimer;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private Controls.ManifestDeltaTree SourceManifestDeltaTree;
        private Controls.ManifestDeltaTree DestinationManifestDeltaTree;
        private System.Windows.Forms.Label checksumLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label blockLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label matchingFilesLabel;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label manifestGuidLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label manifestBlocksLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label manifestSizeLabel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label stateLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label matchingBlocksLabel;
        private System.Windows.Forms.Label label8;
    }
}