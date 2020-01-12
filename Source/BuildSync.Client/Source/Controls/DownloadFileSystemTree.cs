using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Utils;
using BuildSync.Core.Networking.Messages;

namespace BuildSync.Client.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DownloadFileSystemTree : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public class DownloadFileSystemTreeNodeMetadata
        {
            public bool IsBuild;
            public bool IsBuildContainer;
            public Guid ManifestId;
            public DateTime CreateTime;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public bool CanSelectBuildContainers
        {
            get;
            set;
        } = true;

        /// <summary>
        /// 
        /// </summary>
        public string SelectedPath
        {
            get 
            {
                TreeNode SelectedNode = MainTreeView.SelectedNode;
                if (SelectedNode != null)
                {
                    DownloadFileSystemTreeNodeMetadata Metadata = SelectedNode.Tag as DownloadFileSystemTreeNodeMetadata;
                    if (Metadata != null && (Metadata.IsBuild || (Metadata.IsBuildContainer && CanSelectBuildContainers)))
                    {
                        return SelectedNode.FullPath;
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
        /// 
        /// </summary>
        public Guid SelectedManifestId
        {
            get
            {
                TreeNode SelectedNode = MainTreeView.SelectedNode;
                if (SelectedNode != null)
                {
                    DownloadFileSystemTreeNodeMetadata Metadata = SelectedNode.Tag as DownloadFileSystemTreeNodeMetadata;
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
        private List<string> PathsToSelect = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public VirtualFileSystem BuildFileSystem = new VirtualFileSystem();

        /// <summary>
        /// 
        /// </summary>
        [Browsable(true)]
        public event EventHandler OnSelectedNodeChanged;

        /// <summary>
        /// 
        /// </summary>
        [Browsable(true)]
        public event EventHandler OnDateUpdated;

        /// <summary>
        /// 
        /// </summary>
        private void SelectNextPath()
        {
            while (PathsToSelect.Count > 0)
            {
                string NextNode = PathsToSelect[0];
                if (NextNode != "")
                {
                    TreeNode Node = GetNodeByPath(NextNode);
                    if (Node == null)
                    {
                        return;
                    }

                    Node.Expand();
                    MainTreeView.SelectedNode = Node;
                }

                PathsToSelect.RemoveAt(0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, EventArgs e)
        {
            if (Program.NetClient != null)
            {
                Program.NetClient.OnBuildsRecieved += OnBuildInfoRecieved;
            }

            BuildFileSystem.ChildrenRefreshInterval = 5 * 1000;
            BuildFileSystem.AutoRefreshChildren = false;

            BuildFileSystem.OnRequestChildren += (VirtualFileSystem FileSystem, string Path) => {
                if (Program.NetClient != null)
                {
                    Program.NetClient.RequestBuilds(Path);
                }
            };

            BuildFileSystem.OnNodeAdded += (VirtualFileSystem FileSystem, VirtualFileSystemNode Node) => {

                Invoke((MethodInvoker)(() => {

                    TreeNodeCollection NodeCollection = MainTreeView.Nodes;
                    if (Node.Parent != null && Node.Parent.Name != "")
                    {
                        NodeCollection = GetNodeByPath(Node.Parent.Path).Nodes;
                    }

                    DownloadFileSystemTreeNodeMetadata Metadata = new DownloadFileSystemTreeNodeMetadata();
                    Metadata.IsBuild = Node.Metadata != null ? ((Guid)Node.Metadata) != Guid.Empty : false;
                    Metadata.IsBuildContainer = false;
                    Metadata.ManifestId = Node.Metadata != null ? (Guid)Node.Metadata : Guid.Empty;
                    Metadata.CreateTime = Node.Metadata != null ? Node.CreateTime : DateTime.UtcNow;

                    TreeNode TrNode = new TreeNode(Node.Name);
                    TrNode.Tag = Metadata;
                    TrNode.Name = Node.Name;
                    if (Metadata.IsBuild)
                    {
                        TrNode.SelectedImageIndex = 0;
                        TrNode.ImageIndex = 0;
                    }
                    else
                    {
                        TrNode.SelectedImageIndex = 2;
                        TrNode.ImageIndex = 2;
                    }

                    // Insert based on create time.
                    bool Inserted = false;
                    for (int i = 0; i < NodeCollection.Count; i++)
                    {
                        TreeNode SubNode = NodeCollection[i];
                        DownloadFileSystemTreeNodeMetadata SubNodeMetadata = SubNode.Tag as DownloadFileSystemTreeNodeMetadata;
                        if (SubNodeMetadata.CreateTime < Metadata.CreateTime)
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

                    // If its a build, the folder parent becomes a "container".
                    if (Metadata.IsBuild && TrNode.Parent != null)
                    {
                        DownloadFileSystemTreeNodeMetadata ParentMetadata = TrNode.Parent.Tag as DownloadFileSystemTreeNodeMetadata;
                        if (ParentMetadata != null)
                        {
                            ParentMetadata.IsBuildContainer = true;
                        }

                        TrNode.Parent.SelectedImageIndex = 3;
                        TrNode.Parent.ImageIndex = 3;
                    }

                    // If parent node is expanded, then request all children of this node.
                    if (TrNode.Parent == null || TrNode.Parent.IsExpanded)
                    {
                        if (!Metadata.IsBuild)
                        {
                            BuildFileSystem.GetChildrenNames(Node.Path);
                        }
                    }

                    SelectNextPath();

                    OnDateUpdated?.Invoke(this, null);

                }));

            };

            BuildFileSystem.OnNodeRemoved += (VirtualFileSystem FileSystem, VirtualFileSystemNode Node) => {

                Invoke((MethodInvoker)(() => {

                    TreeNode TrNode = GetNodeByPath(Node.Path);
                    TrNode.Remove();

                }));

            };

            BuildFileSystem.Init();
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="RootPath"></param>
        /// <param name="Builds"></param>
        private void OnBuildInfoRecieved(string RootPath, NetMessage_GetBuildsResponse.BuildInfo[] Builds)
        {
            List<VirtualFileSystemInsertChild> NewChildren = new List<VirtualFileSystemInsertChild>();
            foreach (NetMessage_GetBuildsResponse.BuildInfo Build in Builds)
            {
                NewChildren.Add(new VirtualFileSystemInsertChild { 
                    VirtualPath = Build.VirtualPath,
                    CreateTime = Build.Guid == Guid.Empty ? DateTime.UtcNow : Build.CreateTime, 
                    Metadata = Build.Guid 
                });
            }
            BuildFileSystem.ReconcileChildren(RootPath, NewChildren);
            /*
            // Erase old entries for this path.
            BuildFileSystem.RemoveChildNodes(RootPath);

            // Insert new nodes.
            foreach (NetMessage_GetBuildsResponse.BuildInfo Build in Builds)
            {
                BuildFileSystem.InsertNode(Build.VirtualPath, Build.Guid);
            }
            */
        }

        /// <summary>
        /// 
        /// </summary>
        public DownloadFileSystemTree()
        {
            InitializeComponent();

            HandleDestroyed += OnDestroyed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNodeExpanded(object sender, TreeViewEventArgs e)
        {
            // Retrieve all sub nodes.
            foreach (TreeNode Node in e.Node.Nodes)
            {
                DownloadFileSystemTreeNodeMetadata Metadata = Node.Tag as DownloadFileSystemTreeNodeMetadata;
                if (Metadata == null || !Metadata.IsBuild)
                {
                    BuildFileSystem.GetChildrenNames(Node.FullPath);
                }
            }

            // TODO: Force sub-refresh of childrens childrens after a given amount of time?
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="VirtualPath"></param>
        private TreeNode GetNodeByPath(string VirtualPath)
        {
            string[] PathSegments = VirtualPath.Split('\\', '/');
            TreeNode Node = null;
            TreeNodeCollection NodeCollection = MainTreeView.Nodes;

            for (int i = 0; i < PathSegments.Length; i++)
            {
                string Name = PathSegments[i];
                if (NodeCollection.ContainsKey(Name))
                {
                    Node = NodeCollection[Name];
                    NodeCollection = Node.Nodes;
                }
                else
                {
                    Node = null;
                    break;
                }
            }

            return Node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AfterNodeSelected(object sender, TreeViewEventArgs e)
        {
            OnSelectedNodeChanged?.Invoke(this, null);
        }
    }
}
