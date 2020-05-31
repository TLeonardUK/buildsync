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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Utils;
using BuildSync.Core.Tags;
using BuildSync.Core.Manifests;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using BuildSync.Client.Properties;

namespace BuildSync.Client.Controls
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Path"></param>
    public delegate void DeltaTreeNodeChanged(string Path);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Path"></param>
    public delegate void DeltaTreeNodeExpanded(string Path);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Path"></param>
    public delegate void DeltaTreeNodeCollapsed(string Path);

    /// <summary>
    /// </summary>
    public partial class ManifestDeltaTree : UserControl
    {
        public class DeltaTreeNode : Node
        {
            public Image Icon;
            public string Name;

            public string FullPath;
            public string SizeFormatted;
            public string ChecksumFormatted;

            public BuildManifestFileDiff Diff;
        }

        /// <summary>
        /// 
        /// </summary>
        private TreeModel Model = null;

        /// <summary>
        /// </summary>
        [Browsable(true)]
        public event DeltaTreeNodeChanged OnSelectedNodeChanged;

        /// <summary>
        /// </summary>
        [Browsable(true)]
        public event DeltaTreeNodeExpanded OnNodeExpanded;

        /// <summary>
        /// </summary>
        [Browsable(true)]
        public event DeltaTreeNodeCollapsed OnNodeCollapsed;

        /// <summary>
        /// 
        /// </summary>
        public BuildManifestFileDiff SelectedDiff
        {
            get
            {
                if (MainTreeView.SelectedNode == null)
                {
                    return null;
                }

                DeltaTreeNode Node = MainTreeView.SelectedNode.Tag as DeltaTreeNode;
                if (Node == null)
                {
                    return null;
                }

                return Node.Diff;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private BuildManifest Manifest = null;

        private List<BuildManifestFileDiff> Diff = null;

        private Dictionary<string, BuildManifestFileDiffType> FolderDiffTypes = new Dictionary<string, BuildManifestFileDiffType>();

        /// <summary>
        /// </summary>
        public ManifestDeltaTree()
        {
            InitializeComponent();

            Model = new TreeModel();
            MainTreeView.Model = Model;

            TreeColumn NameColumn = new TreeColumn();
            NameColumn.Header = "Name";
            NameColumn.Width = 460;
            MainTreeView.Columns.Add(NameColumn);
            
                ScaledNodeIcon IconControl = new ScaledNodeIcon();
                IconControl.ParentColumn = NameColumn;
                IconControl.DataPropertyName = "Icon";
                IconControl.FixedSize = new Size((int)(MainTreeView.RowHeight * 1.5f), (int)(MainTreeView.RowHeight * 1.5f));
                IconControl.Offset = new Size(0, 5);
                MainTreeView.NodeControls.Add(IconControl);

                NodeTextBox TextControl = new NodeTextBox();
                TextControl.DrawText += DrawNodeText;
                TextControl.ParentColumn = NameColumn;
                TextControl.DataPropertyName = "Name";
                MainTreeView.NodeControls.Add(TextControl);
                
            TreeColumn SizeColumn = new TreeColumn();
            SizeColumn.Header = "Size";
            SizeColumn.Width = 85;
            MainTreeView.Columns.Add(SizeColumn);

                NodeTextBox SizeControl = new NodeTextBox();
                SizeControl.ParentColumn = SizeColumn;
                SizeControl.DataPropertyName = "SizeFormatted";
                MainTreeView.NodeControls.Add(SizeControl);

            /*TreeColumn CreatedColumn = new TreeColumn();
            CreatedColumn.Header = "Checksum";
            CreatedColumn.Width = 260;
            MainTreeView.Columns.Add(CreatedColumn);

                NodeTextBox CreatedControl = new NodeTextBox();
                CreatedControl.ParentColumn = CreatedColumn;
                CreatedControl.DataPropertyName = "ChecksumFormatted";
                MainTreeView.NodeControls.Add(CreatedControl);*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawNodeText(object sender, DrawEventArgs e)
        {
            DeltaTreeNode DeltaNode = e.Node.Tag as DeltaTreeNode;
            if (DeltaNode != null)
            {
                BuildManifestFileDiffType Type = BuildManifestFileDiffType.Unchanged;

                if (DeltaNode.Diff != null)
                {
                    Type = DeltaNode.Diff.Type;
                }
                else if (FolderDiffTypes.ContainsKey(DeltaNode.FullPath))
                {
                    Type = FolderDiffTypes[DeltaNode.FullPath];
                }

                switch (Type)
                {
                    case BuildManifestFileDiffType.Added:
                        {
                            e.TextColor = Color.Green;
                            break;
                        }
                    case BuildManifestFileDiffType.Modified:
                        {
                            e.TextColor = Color.Orange;
                            break;
                        }
                    case BuildManifestFileDiffType.Removed:
                        {
                            e.TextColor = Color.Red;
                            break;
                        }
                    case BuildManifestFileDiffType.Unchanged:
                        {
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ApplyDiff(BuildManifest InManifest, List<BuildManifestFileDiff> InDiff)
        {
            Manifest = InManifest;
            Diff = InDiff;

            pathTextBox.Text = InManifest.VirtualPath;

            Model.Nodes.Clear();

            // Create folders first.
            foreach (BuildManifestFileDiff Diff in InDiff)
            {
                string ParentPath = VirtualFileSystem.GetParentPath(Diff.FileInfo.Path);
                if (ParentPath != "")
                {
                    GetOrCreateNode(ParentPath, null);
                }
            }

            // Create files after.
            foreach (BuildManifestFileDiff Diff in InDiff)
            {
                GetOrCreateNode(Diff.FileInfo.Path, Diff);
            }

            // Calculate the folder states.
            foreach (BuildManifestFileDiff Diff in InDiff)
            {
                string ParentPath = Diff.FileInfo.Path;
                while (true)
                {
                    ParentPath = VirtualFileSystem.GetParentPath(ParentPath);
                    if (ParentPath == "")
                    {
                        break;
                    }

                    if (FolderDiffTypes.ContainsKey(ParentPath))
                    {
                        switch (FolderDiffTypes[ParentPath])
                        {
                            case BuildManifestFileDiffType.Added:
                                {
                                    // Added can be upgraded to modified if we are not already added..
                                    if (Diff.Type != BuildManifestFileDiffType.Added)
                                    {
                                        FolderDiffTypes[ParentPath] = BuildManifestFileDiffType.Modified;
                                    }
                                    break;
                                }
                            case BuildManifestFileDiffType.Modified:
                                {
                                    // Modified never changes.
                                    break;
                                }
                            case BuildManifestFileDiffType.Removed:
                                {
                                    // Removed can be upgraded to modified if we are not already removed.
                                    if (Diff.Type != BuildManifestFileDiffType.Removed)
                                    {
                                        FolderDiffTypes[ParentPath] = BuildManifestFileDiffType.Modified;
                                    }
                                    break;
                                }
                            case BuildManifestFileDiffType.Unchanged:
                                {
                                    // Unchanged can be upgraded to modified.
                                    if (Diff.Type != BuildManifestFileDiffType.Unchanged)
                                    {
                                        FolderDiffTypes[ParentPath] = BuildManifestFileDiffType.Modified;
                                    }
                                    break;
                                }
                        }
                    }
                    else
                    {
                        FolderDiffTypes[ParentPath] = Diff.Type;
                    }
                }
            }

            MainTreeView.FullUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Metadata"></param>
        /// <returns></returns>
        private Node GetOrCreateNode(string Path, BuildManifestFileDiff Metadata, bool DoNotCreate = false)
        {
            if (Path == "")
            {
                return Model.Root;
            }

            string ChildNode = VirtualFileSystem.GetNodeName(Path);

            Node Parent = GetOrCreateNode(VirtualFileSystem.GetParentPath(Path), null);
            foreach (Node node in Parent.Nodes)
            {
                if (node.Text == ChildNode)
                {
                    return node;
                }
            }

            if (DoNotCreate)
            {
                return null;
            }

            DeltaTreeNode NewNode = new DeltaTreeNode();
            NewNode.Name = ChildNode;
            NewNode.Diff = Metadata;
            NewNode.Text = ChildNode;
            NewNode.FullPath = Path;
            Parent.Nodes.Add(NewNode);
            UpdateNode(NewNode);

            return NewNode;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TrNode"></param>
        /// <param name="Node"></param>
        private void UpdateNode(DeltaTreeNode TrNode)
        {
            TrNode.Icon = Resources.appbar_folder_open;

            if (TrNode.Diff != null)
            {
                switch (TrNode.Diff.Type)
                {
                    case BuildManifestFileDiffType.Added:
                        {
                            TrNode.Icon = Resources.appbar_page_add;
                            break;
                        }
                    case BuildManifestFileDiffType.Modified:
                        {
                            TrNode.Icon = Resources.appbar_page_edit;
                            break;
                        }
                    case BuildManifestFileDiffType.Removed:
                        {
                            TrNode.Icon = Resources.appbar_page_delete;
                            break;
                        }
                    case BuildManifestFileDiffType.Unchanged:
                        {
                            TrNode.Icon = Resources.appbar_page;
                            break;
                        }
                }

                if (TrNode.Diff.Type != BuildManifestFileDiffType.Removed)
                {
                    TrNode.SizeFormatted = StringUtils.FormatAsSize((long)TrNode.Diff.FileInfo.Size);
                    TrNode.ChecksumFormatted = string.Format("{0:x}", TrNode.Diff.FileInfo.Checksum);
                }
                else
                {
                    TrNode.SizeFormatted = "";
                    TrNode.ChecksumFormatted = "";
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        public void SelectPath(string Path)
        {
            foreach (TreeNodeAdv Node in MainTreeView.AllNodes)
            {
                DeltaTreeNode Meta = Node.Tag as DeltaTreeNode;
                if (Meta != null && Meta.FullPath == Path)
                {
                    MainTreeView.SelectedNode = Node;
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        public void ExpandNode(string Path)
        {
            foreach (TreeNodeAdv Node in MainTreeView.AllNodes)
            {
                DeltaTreeNode Meta = Node.Tag as DeltaTreeNode;
                if (Meta != null && Meta.FullPath == Path)
                {
                    Node.Expand();
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        public void CollapseNode(string Path)
        {
            foreach (TreeNodeAdv Node in MainTreeView.AllNodes)
            {
                DeltaTreeNode Meta = Node.Tag as DeltaTreeNode;
                if (Meta != null && Meta.FullPath == Path)
                {
                    Node.Collapse();
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (MainTreeView.SelectedNode == null)
            {
                return;
            }

            DeltaTreeNode Node = MainTreeView.SelectedNode.Tag as DeltaTreeNode;
            if (Node == null)
            {
                return;
            }

            OnSelectedNodeChanged?.Invoke(Node.FullPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeExpanded(object sender, TreeViewAdvEventArgs e)
        {
            if (MainTreeView.SelectedNode == null)
            {
                return;
            }

            DeltaTreeNode Node = e.Node.Tag as DeltaTreeNode;
            if (Node == null)
            {
                return;
            }

            OnNodeExpanded?.Invoke(Node.FullPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeCollapsed(object sender, TreeViewAdvEventArgs e)
        {
            if (MainTreeView.SelectedNode == null)
            {
                return;
            }

            DeltaTreeNode Node = e.Node.Tag as DeltaTreeNode;
            if (Node == null)
            {
                return;
            }

            OnNodeCollapsed?.Invoke(Node.FullPath);
        }
    }
}