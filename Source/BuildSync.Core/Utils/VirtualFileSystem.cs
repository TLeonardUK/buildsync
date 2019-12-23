using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace BuildSync.Core.Utils
{

    /// <summary>
    /// 
    /// </summary>
    public class VirtualFileSystemNode
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name = "";

        /// <summary>
        /// 
        /// </summary>
        public string Path = "";

        /// <summary>
        /// 
        /// </summary>
        public VirtualFileSystemNode Parent;

        /// <summary>
        /// 
        /// </summary>
        public List<VirtualFileSystemNode> Children = new List<VirtualFileSystemNode>();

        /// <summary>
        /// 
        /// </summary>
        public int LastChildRefreshTime = 0;

        /// <summary>
        /// 
        /// </summary>
        public object Metadata = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public VirtualFileSystemNode GetChildByName(string InName)
        {
            foreach (VirtualFileSystemNode Child in Children)
            {
                if (Child.Name == InName)
                {
                    return Child;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Naem"></param>
        public VirtualFileSystemNode(string InName, string InPath, VirtualFileSystemNode InParent = null, object InMetadata = null)
        {
            Name = InName;
            Path = InPath;
            Parent = InParent;
            Metadata = InMetadata;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    public delegate void VirtualFileSystemRequestChildrenHandler(VirtualFileSystem FileSystem, string Path);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    public delegate void VirtualFileSystemNodeAddedHandler(VirtualFileSystem FileSystem, VirtualFileSystemNode Node);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    public delegate void VirtualFileSystemNodeRemovedHandler(VirtualFileSystem FileSystem, VirtualFileSystemNode Node);

    /// <summary>
    /// 
    /// </summary>
    public struct VirtualFileSystemInsertChild
    {
        public string VirtualPath;
        public object Metadata;
    }

    /// <summary>
    /// 
    /// </summary>
    public class VirtualFileSystem
    {
        /// <summary>
        /// 
        /// </summary>
        public int ChildrenRefreshInterval = 60 * 1000;

        /// <summary>
        /// 
        /// </summary>
        public bool AutoRefreshChildren = true;

        /// <summary>
        /// 
        /// </summary>
        public VirtualFileSystemNode Root;

        /// <summary>
        /// 
        /// </summary>
        public VirtualFileSystemRequestChildrenHandler OnRequestChildren;

        /// <summary>
        /// 
        /// </summary>
        public VirtualFileSystemNodeAddedHandler OnNodeAdded;

        /// <summary>
        /// 
        /// </summary>
        public VirtualFileSystemNodeRemovedHandler OnNodeRemoved;

        public VirtualFileSystem()
        {
            Root = new VirtualFileSystemNode("", "", null, null);
            RefreshNodeChildren(Root);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Node"></param>
        private void RefreshNodeChildren(VirtualFileSystemNode Node)
        {
            int ElapsedTime = Environment.TickCount - Node.LastChildRefreshTime;
            if (Node.LastChildRefreshTime == 0 || (ElapsedTime > ChildrenRefreshInterval && AutoRefreshChildren))
            {
                OnRequestChildren?.Invoke(this, Node.Path);
                Node.LastChildRefreshTime = Environment.TickCount;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ForceRefresh(VirtualFileSystemNode Node = null)
        {
            if (Node == null)
            {
                Node = Root;
            }
            Node.LastChildRefreshTime = 0;
            foreach (VirtualFileSystemNode Child in Node.Children)
            {
                ForceRefresh(Child);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            // Get root nodes.
            OnRequestChildren?.Invoke(this, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Children"></param>
        public void ReconcileChildren(string Path, List<VirtualFileSystemInsertChild> Children)
        {
            VirtualFileSystemNode BaseNode = GetNodeByPath(Path);

            List<VirtualFileSystemNode> ChildrenToRemove = new List<VirtualFileSystemNode>();
            List<VirtualFileSystemInsertChild> ChildrenToAdd = new List<VirtualFileSystemInsertChild>();

            // Find children to remove.
            foreach (VirtualFileSystemNode Child in BaseNode.Children)
            {
                bool Exists = false;

                foreach (VirtualFileSystemInsertChild NewChild in Children)
                {
                    if (Child.Path == NewChild.VirtualPath)
                    {
                        Exists = true;
                        break;
                    }
                }

                if (!Exists)
                {
                    ChildrenToRemove.Add(Child);
                }
            }

            // Find children to add.
            foreach (VirtualFileSystemInsertChild NewChild in Children)
            {
                bool Exists = false;

                foreach (VirtualFileSystemNode Child in BaseNode.Children)
                {
                    if (Child.Path == NewChild.VirtualPath)
                    {
                        // Update metadata while we are here in case it changed.
                        Child.Metadata = NewChild.Metadata;
                        Exists = true;
                        break;
                    }
                }

                if (!Exists)
                {
                    ChildrenToAdd.Add(NewChild);
                }
            }

            // Remove children.
            foreach (VirtualFileSystemNode Child in ChildrenToRemove)
            {
                OnNodeRemoved?.Invoke(this, Child);
                BaseNode.Children.Remove(Child);
            }

            // Add children.
            foreach (VirtualFileSystemInsertChild NewChild in ChildrenToAdd)
            {
                string NodeName = GetNodeName(NewChild.VirtualPath);

                VirtualFileSystemNode Child = new VirtualFileSystemNode(NodeName, NewChild.VirtualPath, BaseNode, NewChild.Metadata);
                BaseNode.Children.Add(Child);

                OnNodeAdded?.Invoke(this, Child);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        public VirtualFileSystemNode InsertNode(string Path, object Metadata = null)
        {
            VirtualFileSystemNode ParentNode = Root;

            string ParentPath = GetParentPath(Path);
            if (ParentPath.Length > 0)
            {
                ParentNode = InsertNode(ParentPath, null);
            }

            string NodeName = GetNodeName(Path);
            VirtualFileSystemNode Child = ParentNode.GetChildByName(NodeName);
            if (Child == null)
            {
                Child = new VirtualFileSystemNode(NodeName, Path, ParentNode, Metadata);
                ParentNode.Children.Add(Child);

                OnNodeAdded?.Invoke(this, Child);
            }
            else
            {
                Child.Metadata = Metadata;
            }

            return Child;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        public void RemoveChildNodes(string Path)
        {
            VirtualFileSystemNode Node = GetNodeByPath(Path);
            if (Node != null)
            {
                foreach (VirtualFileSystemNode Child in Node.Children)
                {
                    OnNodeRemoved?.Invoke(this, Child);
                }
                Node.Children.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        public List<string> GetChildrenNames(string Path)
        {
            List<string> Result = new List<string>();            

            VirtualFileSystemNode Node = GetNodeByPath(Path);
            if (Node != null)
            {
                RefreshNodeChildren(Node);

                foreach (VirtualFileSystemNode Child in Node.Children)
                {
                    Result.Add(Child.Path);
                }
            }

            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        public List<VirtualFileSystemNode> GetChildren(string Path)
        {
            List<VirtualFileSystemNode> Result = new List<VirtualFileSystemNode>();

            VirtualFileSystemNode Node = GetNodeByPath(Path);
            if (Node != null)
            {
                RefreshNodeChildren(Node);

                foreach (VirtualFileSystemNode Child in Node.Children)
                {
                    Result.Add(Child);
                }
            }

            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public VirtualFileSystemNode GetNodeByPath(string Path)
        {
            if (Path.Length == 0)
            {
                RefreshNodeChildren(Root);
                return Root;
            }

            VirtualFileSystemNode ParentNode = Root;

            string ParentPath = GetParentPath(Path);
            if (ParentPath.Length > 0)
            {
                ParentNode = GetNodeByPath(ParentPath);
            }

            if (ParentNode == null)
            {
                return null;
            }

            RefreshNodeChildren(ParentNode);

            string NodeName = GetNodeName(Path);
            return ParentNode.GetChildByName(NodeName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public List<string> GetSubPaths(string InPath)
        {
            List<string> Result = new List<string>();

            string[] Segments = InPath.Split('/', '\\');
            for (int i = Segments.Length; i >= 0; i--)
            {
                string SubPath = "";
                for (int j = 0; j < i; j++)
                {
                    if (SubPath.Length > 0)
                    {
                        SubPath += @"\";
                    }
                    SubPath += Segments[j];
                }
                Result.Add(SubPath);
            }

            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public string GetParentPath(string InPath)
        {
            int Index = InPath.LastIndexOfAny(new char[] { '\\', '/' });
            if (Index < 0)
            {
                return "";
            }
            return InPath.Substring(0, Index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public string GetNodeName(string InPath)
        {
            int Index = InPath.LastIndexOfAny(new char[] { '\\', '/' });
            if (Index < 0)
            {
                return InPath;
            }
            return InPath.Substring(Index + 1);
        }
    }
}
