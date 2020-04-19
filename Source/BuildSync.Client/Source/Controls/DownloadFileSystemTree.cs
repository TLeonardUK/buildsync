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
    /// </summary>
    public partial class DownloadFileSystemTree : UserControl
    {
        public class DownloadFileSystemTreeNode : Node
        {
            public Image Icon;
            public string Name;
            public string FullPath;
            public DateTime CreateTime;
            public string CreateTimeFormatted;
            public string SizeFormatted;
            public bool IsBuild;
            public bool IsBuildContainer;
            public Guid ManifestId;
            public string Availability;
            public Image AvailabilityIcon;
            public string TagsFormatted;
        }

        /// <summary>
        /// </summary>
        public VirtualFileSystem BuildFileSystem = new VirtualFileSystem();

        /// <summary>
        /// </summary>
        private List<string> PathsToSelect = new List<string>();

        /// <summary>
        /// </summary>
        [Browsable(true)]
        public event EventHandler OnDateUpdated;

        /// <summary>
        /// </summary>
        [Browsable(true)]
        public event EventHandler OnSelectedNodeChanged;

        /// <summary>
        /// </summary>
        public bool CanSelectBuildContainers { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        private TreeModel Model = null;

        /// <summary>
        /// 
        /// </summary>
        private bool InternallyChangingPathText = false;

        /// <summary>
        /// 
        /// </summary>
        private bool NodeSelectedAutomatically = false;

        /// <summary>
        /// 
        /// </summary>
        private List<string> RequestedBuildPaths = new List<string>();

        /// <summary>
        /// </summary>
        public Guid SelectedManifestId
        {
            get
            {
                TreeNodeAdv SelectedNode = MainTreeView.SelectedNode;
                if (SelectedNode != null)
                {
                    DownloadFileSystemTreeNode Metadata = SelectedNode.Tag as DownloadFileSystemTreeNode;
                    if (Metadata != null)
                    {
                        return Metadata.ManifestId;
                    }
                }

                return Guid.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Tag> SelectedManifestTags
        {
            get
            {
                TreeNodeAdv SelectedNode = MainTreeView.SelectedNode;
                if (SelectedNode != null)
                {
                    DownloadFileSystemTreeNode Metadata = SelectedNode.Tag as DownloadFileSystemTreeNode;
                    if (Metadata != null)
                    {
                        VirtualFileSystemNode Node = BuildFileSystem.GetNodeByPath(Metadata.FullPath);
                        if (Node != null && Node.Metadata != null)
                        {
                            NetMessage_GetBuildsResponse.BuildInfo BuildInfo = (NetMessage_GetBuildsResponse.BuildInfo)Node.Metadata;
                            return new List<Tag>(BuildInfo.Tags);
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// </summary>
        public bool IsSelectedBuildContainer
        {
            get
            {
                TreeNodeAdv SelectedNode = MainTreeView.SelectedNode;
                if (SelectedNode != null)
                {
                    DownloadFileSystemTreeNode Metadata = SelectedNode.Tag as DownloadFileSystemTreeNode;
                    if (Metadata != null)
                    {
                        return Metadata.IsBuildContainer;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// </summary>
        public string SelectedPath
        {
            get
            {
                TreeNodeAdv SelectedNode = MainTreeView.SelectedNode;
                if (SelectedNode != null)
                {
                    DownloadFileSystemTreeNode Metadata = SelectedNode.Tag as DownloadFileSystemTreeNode;
                    if (Metadata != null && (Metadata.IsBuild || Metadata.IsBuildContainer && CanSelectBuildContainers))
                    {
                        return Metadata.FullPath;
                    }
                }

                return "";
            }
            set
            {
                PathsToSelect = BuildFileSystem.GetSubPaths(value);
                PathsToSelect.Reverse();
                SelectNextPath();
            }
        }
        
        /// <summary>
        /// </summary>
        public string SelectedPathRaw
        {
            get
            {
                TreeNodeAdv SelectedNode = MainTreeView.SelectedNode;
                if (SelectedNode != null)
                {
                    DownloadFileSystemTreeNode Metadata = SelectedNode.Tag as DownloadFileSystemTreeNode;
                    if (Metadata != null)
                    {
                        return Metadata.FullPath;
                    }
                }

                return "";
            }
            set
            {
                PathsToSelect = BuildFileSystem.GetSubPaths(value);
                PathsToSelect.Reverse();
                SelectNextPath();
            }
        }

        /// <summary>
        /// </summary>
        public bool ShowInternal { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public void UpdatePathText(string Text)
        {
            InternallyChangingPathText = true;
            pathTextBox.Text = Text;
            InternallyChangingPathText = false;
        }

        /// <summary>
        /// </summary>
        public DownloadFileSystemTree()
        {
            InitializeComponent();

            HandleDestroyed += OnDestroyed;

            Model = new TreeModel();
            MainTreeView.Model = Model;

            TreeColumn NameColumn = new TreeColumn();
            NameColumn.Header = "Name";
            NameColumn.Width = 200;
            MainTreeView.Columns.Add(NameColumn);
            
                ScaledNodeIcon IconControl = new ScaledNodeIcon();
                IconControl.ParentColumn = NameColumn;
                IconControl.DataPropertyName = "Icon";
                IconControl.FixedSize = new Size((int)(MainTreeView.RowHeight * 1.5f), (int)(MainTreeView.RowHeight * 1.5f));
                IconControl.Offset = new Size(0, 5);
                MainTreeView.NodeControls.Add(IconControl);

                NodeTextBox TextControl = new NodeTextBox();
                TextControl.ParentColumn = NameColumn;
                TextControl.DataPropertyName = "Name";
                MainTreeView.NodeControls.Add(TextControl);
                
            TreeColumn SizeColumn = new TreeColumn();
            SizeColumn.Header = "Size";
            SizeColumn.Width = 70;
            MainTreeView.Columns.Add(SizeColumn);

                NodeTextBox SizeControl = new NodeTextBox();
                SizeControl.ParentColumn = SizeColumn;
                SizeControl.DataPropertyName = "SizeFormatted";
                MainTreeView.NodeControls.Add(SizeControl);

            TreeColumn CreatedColumn = new TreeColumn();
            CreatedColumn.Header = "Created";
            CreatedColumn.Width = 120;
            MainTreeView.Columns.Add(CreatedColumn);

                NodeTextBox CreatedControl = new NodeTextBox();
                CreatedControl.ParentColumn = CreatedColumn;
                CreatedControl.DataPropertyName = "CreateTimeFormatted";
                MainTreeView.NodeControls.Add(CreatedControl);

            TreeColumn AvailabilityColumn = new TreeColumn();
            AvailabilityColumn.Header = "Availability";
            AvailabilityColumn.Width = 200;
            MainTreeView.Columns.Add(AvailabilityColumn);
            
                ScaledNodeIcon AvailabilityIconControl = new ScaledNodeIcon();
                AvailabilityIconControl.ParentColumn = AvailabilityColumn;
                AvailabilityIconControl.DataPropertyName = "AvailabilityIcon";
                AvailabilityIconControl.FixedSize = new Size((int)(MainTreeView.RowHeight * 1.5f), (int)(MainTreeView.RowHeight * 1.5f));
                AvailabilityIconControl.Offset = new Size(0, 5);
                MainTreeView.NodeControls.Add(AvailabilityIconControl);

                NodeTextBox AvailabilityControl = new NodeTextBox();
                AvailabilityControl.ParentColumn = AvailabilityColumn;
                AvailabilityControl.DataPropertyName = "Availability";
                MainTreeView.NodeControls.Add(AvailabilityControl);
                
            TreeColumn TagsColumn = new TreeColumn();
            TagsColumn.Header = "Tags";
            TagsColumn.Width = 200;
            MainTreeView.Columns.Add(TagsColumn);

                NodeTextBox TagControl = new NodeTextBox();
                TagControl.ParentColumn = TagsColumn;
                TagControl.DataPropertyName = "TagsFormatted";
                MainTreeView.NodeControls.Add(TagControl);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AfterNodeSelected(object sender, EventArgs e)
        {
            OnSelectedNodeChanged?.Invoke(this, null);

            if (!NodeSelectedAutomatically)
            {
                InternallyChangingPathText = true;
                pathTextBox.Text = SelectedPathRaw;
                InternallyChangingPathText = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Collection"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        private Node CollectionGetByName(Collection<Node> Collection, string Key)
        {
            foreach (DownloadFileSystemTreeNode node in Collection)
            {
                if (node.Name == Key)
                {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="VirtualPath"></param>
        private Node GetNodeByPath(string VirtualPath)
        {
            string[] PathSegments = VirtualPath.Split('\\', '/');
            Node SearchNode = null;
            Collection<Node> NodeCollection = Model.Root.Nodes;

            for (int i = 0; i < PathSegments.Length; i++)
            {
                string Name = PathSegments[i];

                Node NamedNode = CollectionGetByName(NodeCollection, Name);
                if (NamedNode != null)
                {
                    SearchNode = NamedNode;
                    NodeCollection = SearchNode.Nodes;
                }
                else
                {
                    SearchNode = null;
                    break;
                }
            }

            return SearchNode;
        }

        /// <summary>
        /// </summary>
        /// <param name="VirtualPath"></param>
        private TreeNodeAdv GetViewNodeByPath(string VirtualPath)
        {
            foreach (TreeNodeAdv Node in MainTreeView.AllNodes)
            {
                DownloadFileSystemTreeNode Meta = Node.Tag as DownloadFileSystemTreeNode;
                if (Meta != null && Meta.FullPath == VirtualPath)
                {
                    return Node;
                }
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="RootPath"></param>
        /// <param name="Builds"></param>
        private void OnBuildInfoRecieved(string RootPath, NetMessage_GetBuildsResponse.BuildInfo[] Builds)
        {
            //if (!RequestedBuildPaths.Contains(RootPath))
            //{
            //    return;
            //}

            RequestedBuildPaths.Remove(RootPath);

            List<VirtualFileSystemInsertChild> NewChildren = new List<VirtualFileSystemInsertChild>();
            foreach (NetMessage_GetBuildsResponse.BuildInfo Build in Builds)
            {
                NewChildren.Add(
                    new VirtualFileSystemInsertChild
                    {
                        VirtualPath = Build.VirtualPath,
                        CreateTime = Build.Guid == Guid.Empty ? DateTime.UtcNow : Build.CreateTime,
                        Metadata = Build
                    }
                );
            }

            BuildFileSystem.ReconcileChildren(RootPath, NewChildren);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDestroyed(object sender, EventArgs e)
        {
            if (Program.NetClient != null)
            {
                Program.NetClient.OnBuildsRecieved -= OnBuildInfoRecieved;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, EventArgs e)
        {
            if (Program.NetClient != null)
            {
                Program.NetClient.OnBuildsRecieved += OnBuildInfoRecieved;
            }

            SetupFileSystem();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TrNode"></param>
        /// <param name="Node"></param>
        private void UpdateNode(DownloadFileSystemTreeNode TrNode, VirtualFileSystemNode Node)
        {
            TrNode.FullPath = Node.Path;
            TrNode.Name = Node.Name;
            TrNode.Icon = Resources.appbar_box;
            TrNode.AvailabilityIcon = null;

            if (Node.Metadata != null)
            {
                NetMessage_GetBuildsResponse.BuildInfo BuildInfo = (NetMessage_GetBuildsResponse.BuildInfo)Node.Metadata;

                TrNode.IsBuild = BuildInfo.Guid != Guid.Empty;
                TrNode.ManifestId = BuildInfo.Guid;
                TrNode.CreateTime = Node.CreateTime;
                TrNode.SizeFormatted = StringUtils.FormatAsSize((long)BuildInfo.TotalSize);
                TrNode.CreateTimeFormatted = BuildInfo.CreateTime.ToString("dd/MM/yyyy HH:mm");

                // 9        = very high
                // 7,8      = high
                // 4,5,6,   = medium
                // 2,3      = low
                // 1        = very low
                // 0        = not

                TrNode.Availability = BuildInfo.AvailablePeers + " peers have entire build";
                if (BuildInfo.AvailablePeers >= 9)
                {
                    TrNode.AvailabilityIcon = Resources.appbar_connection_quality_veryhigh;
                }
                else if (BuildInfo.AvailablePeers >= 7)
                {
                    TrNode.AvailabilityIcon = Resources.appbar_connection_quality_high;
                }
                else if (BuildInfo.AvailablePeers >= 3)
                {
                    TrNode.AvailabilityIcon = Resources.appbar_connection_quality_medium;
                }
                else if (BuildInfo.AvailablePeers >= 2)
                {
                    TrNode.AvailabilityIcon = Resources.appbar_connection_quality_low;
                }
                else if (BuildInfo.AvailablePeers >= 1)
                {
                    TrNode.AvailabilityIcon = Resources.appbar_connection_quality_verylow;
                }
                else
                {
                    TrNode.AvailabilityIcon = Resources.appbar_close;
                    TrNode.Availability = "Last available " + BuildInfo.LastSeenOnPeer.ToString("dd/MM/yyyy HH:mm");
                }

                if (BuildInfo.Tags != null)
                {
                    TrNode.TagsFormatted = "";
                    foreach (Tag Tag in BuildInfo.Tags)
                    {
                        if (TrNode.TagsFormatted.Length > 0)
                        {
                            TrNode.TagsFormatted += ", ";
                        }
                        TrNode.TagsFormatted += Tag.Name;
                    }
                }
                else
                {
                    TrNode.TagsFormatted = "";
                }
            }
            else
            {
                TrNode.IsBuild = false;
                TrNode.ManifestId = Guid.Empty;
                TrNode.CreateTime = DateTime.UtcNow;
            }

            if (!TrNode.IsBuild)
            {
                TrNode.SizeFormatted = "";
                TrNode.Availability = "";
                TrNode.CreateTimeFormatted = "";
                TrNode.AvailabilityIcon = null;
                TrNode.TagsFormatted = "";
            }

            if (TrNode.IsBuild)
            {
                TrNode.Icon = Resources.appbar_box;
            }
            else if (TrNode.IsBuildContainer)
            {
                TrNode.Icon = Resources.appbar_database;
            }
            else
            {
                TrNode.Icon = Resources.appbar_folder_open;
            }

            // If its a build, the folder parent becomes a "container".
            if (TrNode.IsBuild && TrNode.Parent != null)
            {
                DownloadFileSystemTreeNode ParentNode = TrNode.Parent as DownloadFileSystemTreeNode;
                if (ParentNode != null)
                {
                    ParentNode.IsBuildContainer = true;
                    ParentNode.Icon = Resources.appbar_database;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetupFileSystem()
        {
            RequestedBuildPaths.Clear();

            Model.Nodes.Clear();

            BuildFileSystem = new VirtualFileSystem();
            BuildFileSystem.ChildrenRefreshInterval = 5 * 1000;
            BuildFileSystem.AutoRefreshChildren = false;

            BuildFileSystem.OnRequestChildren += (FileSystem, Path) =>
            {
                if (Program.NetClient != null)
                {
                    RequestedBuildPaths.Add(Path);
                    Program.NetClient.RequestBuilds(Path);
                }
            };

            BuildFileSystem.OnNodeUpdated += (FileSystem, Node) =>
            {
                Node ModelNode = GetNodeByPath(Node.Path);
                if (ModelNode != null)
                {
                    UpdateNode(ModelNode as DownloadFileSystemTreeNode, Node);
                }

                MainTreeView.Refresh();
            };

            BuildFileSystem.OnNodeAdded += (FileSystem, Node) =>
            {
                // Ignore internal parts of the heirarchy.
                if (Node.Path.Contains("$") && !ShowInternal)
                {
                    return;
                }

                Collection<Node> NodeCollection = Model.Root.Nodes;
                if (Node.Parent != null && Node.Parent.Name != "")
                {
                    NodeCollection = GetNodeByPath(Node.Parent.Path).Nodes;
                }

                DownloadFileSystemTreeNode TrNode = new DownloadFileSystemTreeNode();
                TrNode.IsBuildContainer = false;

                DateTime SortTime = DateTime.UtcNow;
                if (Node.Metadata != null)
                {
                    NetMessage_GetBuildsResponse.BuildInfo BuildInfo = (NetMessage_GetBuildsResponse.BuildInfo)Node.Metadata;
                    SortTime = Node.CreateTime;
                }

                // Insert based on create time.
                bool Inserted = false;
                for (int i = 0; i < NodeCollection.Count; i++)
                {
                    DownloadFileSystemTreeNode SubNode = NodeCollection[i] as DownloadFileSystemTreeNode;
                    if (SubNode != null && (SubNode.CreateTime.Ticks - SortTime.Ticks) < -10000000) // At least a second off.
                    {
                        NodeCollection.Insert(i, TrNode);
                        Inserted = true;
                        break;
                    }
                }

                if (!Inserted)
                {
                    NodeCollection.Add(TrNode);
                }

                UpdateNode(TrNode, Node);

                // If parent node is expanded, then request all children of this node.
                TreeNodeAdv ParentViewNode = null;
                if (TrNode.Parent != null)
                {
                    DownloadFileSystemTreeNode ParentNode = TrNode.Parent as DownloadFileSystemTreeNode;
                    if (ParentNode != null)
                    {
                        ParentViewNode = GetViewNodeByPath(ParentNode.FullPath);
                    }
                    else
                    {
                        ParentViewNode = MainTreeView.Root;
                    }
                }

                if (ParentViewNode == null || ParentViewNode.IsExpanded)
                {
                    if (!TrNode.IsBuild)
                    {
                        BuildFileSystem.GetChildrenNames(Node.Path);
                    }
                }

                MainTreeView.FullUpdate();

                SelectNextPath();

                OnDateUpdated?.Invoke(this, null);
            };

            BuildFileSystem.OnNodeRemoved += (FileSystem, Node) =>
            {
                // Ignore internal parts of the heirarchy.
                if (Node.Path.Contains("$") && !ShowInternal)
                {
                    return;
                }

                Node ModelNode = GetNodeByPath(Node.Path);
                if (ModelNode != null && ModelNode.Parent != null)
                {
                    ModelNode.Parent.Nodes.Remove(ModelNode);
                }
            };

            BuildFileSystem.Init();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNodeExpanded(object sender, Aga.Controls.Tree.TreeViewAdvEventArgs e)
        {
            // Retrieve all sub nodes.
            foreach (TreeNodeAdv SNode in e.Node.Children)
            {
                DownloadFileSystemTreeNode Metadata = SNode.Tag as DownloadFileSystemTreeNode;
                if (Metadata == null || !Metadata.IsBuild)
                {
                    BuildFileSystem.GetChildrenNames(Metadata.FullPath);
                }
            }

            // TODO: Force sub-refresh of childrens childrens after a given amount of time?
        }

        /// <summary>
        /// </summary>
        private void SelectNextPath()
        {
            NodeSelectedAutomatically = true;

            while (PathsToSelect.Count > 0)
            {
                string NextNode = PathsToSelect[0];
                if (NextNode != "")
                {
                    TreeNodeAdv Node = GetViewNodeByPath(NextNode);
                    if (Node == null)
                    {
                        return;
                    }

                    Node.Expand();
                    MainTreeView.SelectedNode = Node;
                }

                PathsToSelect.RemoveAt(0);
            }

            NodeSelectedAutomatically = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PathTextChanged(object sender, EventArgs e)
        {
            if (!InternallyChangingPathText)
            {
                SelectedPathRaw = pathTextBox.Text;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshClicked(object sender, EventArgs e)
        {
            string OldPath = SelectedPathRaw;
            SetupFileSystem();
//            SelectedPathRaw = OldPath;
            pathTextBox.Text = OldPath;
        }
    }
}