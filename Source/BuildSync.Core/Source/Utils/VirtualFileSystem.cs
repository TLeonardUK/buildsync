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
        public ulong LastChildRefreshTime = 0;

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime = DateTime.UtcNow;

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
        public void SortChildren()
        {
            Children.Sort((Item1, Item2) => -Item1.CreateTime.CompareTo(Item2.CreateTime));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Naem"></param>
        public VirtualFileSystemNode(string InName, string InPath, DateTime InCreateTime, VirtualFileSystemNode InParent = null, object InMetadata = null)
        {
            Name = InName;
            Path = InPath;
            Parent = InParent;
            Metadata = InMetadata;
            CreateTime = InCreateTime;
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
        public DateTime CreateTime;
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
        public ulong ChildrenRefreshInterval = 60 * 1000;

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
            Root = new VirtualFileSystemNode("", "", DateTime.UtcNow, null, null);
            RefreshNodeChildren(Root);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Node"></param>
        private void RefreshNodeChildren(VirtualFileSystemNode Node)
        {
            ulong ElapsedTime = TimeUtils.Ticks - Node.LastChildRefreshTime;
            if (Node.LastChildRefreshTime == 0 || (ElapsedTime > ChildrenRefreshInterval && AutoRefreshChildren))
            {
                OnRequestChildren?.Invoke(this, Node.Path);
                Node.LastChildRefreshTime = TimeUtils.Ticks;
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
            if (BaseNode == null)
            {
                BaseNode = InsertNode(Path, DateTime.UtcNow, null);
            }

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
                        Child.CreateTime = NewChild.CreateTime;
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
                BaseNode.Children.Remove(Child);
            }

            // Add children.
            List<VirtualFileSystemNode> NewChildren = new List<VirtualFileSystemNode>();
            foreach (VirtualFileSystemInsertChild NewChild in ChildrenToAdd)
            {
                string NodeName = GetNodeName(NewChild.VirtualPath);

                VirtualFileSystemNode Child = new VirtualFileSystemNode(NodeName, NewChild.VirtualPath, NewChild.CreateTime, BaseNode, NewChild.Metadata);
                BaseNode.Children.Add(Child);
                NewChildren.Add(Child);
            }

            // Sort all children by create time.
            BaseNode.Children.Sort((Item1, Item2) => -Item1.CreateTime.CompareTo(Item2.CreateTime));

            // Fire events.
            foreach (VirtualFileSystemNode Child in ChildrenToRemove)
            {
                OnNodeRemoved?.Invoke(this, Child);
            }
            foreach (VirtualFileSystemNode Child in NewChildren)
            {
                OnNodeAdded?.Invoke(this, Child);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        public VirtualFileSystemNode InsertNode(string Path, DateTime CreateTime, object Metadata = null)
        {
            VirtualFileSystemNode ParentNode = Root;

            string ParentPath = GetParentPath(Path);
            if (ParentPath.Length > 0)
            {
                ParentNode = InsertNode(ParentPath, CreateTime, null);
            }

            string NodeName = GetNodeName(Path);
            VirtualFileSystemNode Child = ParentNode.GetChildByName(NodeName);
            if (Child == null)
            {
                Child = new VirtualFileSystemNode(NodeName, Path, CreateTime, ParentNode, Metadata);
                ParentNode.Children.Add(Child);
                ParentNode.SortChildren();

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
        public void RemoveNode(string Path)
        {
            VirtualFileSystemNode Node = GetNodeByPath(Path);
            if (Node != null && Node.Parent != null)
            {
                OnNodeRemoved?.Invoke(this, Node);
                Node.Parent.Children.Remove(Node);
            }
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
        public static string GetParentPath(string InPath)
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
        /// <param name="InPath"></param>
        /// <returns></returns>
        public static string Normalize(string InPath)
        {
            return InPath.Replace('\\', '/').TrimEnd('/');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static string GetNodeName(string InPath)
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
