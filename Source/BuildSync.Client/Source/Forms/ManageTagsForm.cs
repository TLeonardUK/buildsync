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
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Windows.Forms;
using BuildSync.Core.Utils;
using BuildSync.Core.Manifests;
using BuildSync.Core.Users;
using BuildSync.Core.Tags;
using WeifenLuo.WinFormsUI.Docking;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using BuildSync.Client.Properties;
using BuildSync.Client.Controls;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ManageTagsForm : DockContent
    {
        /// <summary>
        /// 
        /// </summary>
        public class TagTreeNode : Node
        {
            public Image Icon;
            public string Name;

            public Tag BuildTag;
            public Tag[] BuildTags;

            public string Unique;
            public Tag[] DecayTags;
        }

        /// <summary>
        /// 
        /// </summary>
        private TreeModel Model = null;

        /// <summary>
        /// 
        /// </summary>
        public ManageTagsForm()
        {
            InitializeComponent();

            Program.NetClient.OnTagListRecieved += TagsRecieved;
                        
            Model = new TreeModel();
            MainTreeView.Model = Model;

            TreeColumn NameColumn = new TreeColumn();
            NameColumn.Header = "Name";
            NameColumn.Width = 400;
            MainTreeView.Columns.Add(NameColumn);
            
                TagListTreeNode TextControl = new TagListTreeNode();
                TextControl.ParentColumn = NameColumn;
                TextControl.ShowFullName = true;
                TextControl.DataPropertyName = "BuildTags";
                MainTreeView.NodeControls.Add(TextControl);
            
            TreeColumn UniqueColumn = new TreeColumn();
            UniqueColumn.Header = "Is Unique";
            UniqueColumn.Width = 100;
            MainTreeView.Columns.Add(UniqueColumn);
            
                NodeTextBox UniqueControl = new NodeTextBox();
                UniqueControl.ParentColumn = UniqueColumn;
                UniqueControl.DataPropertyName = "Unique";
                MainTreeView.NodeControls.Add(UniqueControl);
            
            TreeColumn DecayColumn = new TreeColumn();
            DecayColumn.Header = "Decay Into";
            DecayColumn.Width = 400;
            MainTreeView.Columns.Add(DecayColumn);
            
                TagListTreeNode DecayControl = new TagListTreeNode();
                DecayControl.ParentColumn = DecayColumn;
                DecayControl.ShowFullName = true;
                DecayControl.DataPropertyName = "DecayTags";
                MainTreeView.NodeControls.Add(DecayControl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            Program.NetClient.RequestTagList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshTagList(object sender, EventArgs e)
        {
            Program.NetClient.RequestTagList();
        }

        /// <summary>
        /// </summary>
        /// <param name="Users"></param>
        private void TagsRecieved(List<Tag> InTags)
        {
            bool ForceUpdate = false;

            InTags.Sort((Item1, Item2) => -Item1.Name.CompareTo(Item2.Name));

            // Add new tags.
            foreach (Tag Tag in InTags)
            {
                bool Found = false;

                foreach (TagTreeNode Node in Model.Nodes)
                {
                    if (Node.BuildTag.Id == Tag.Id)
                    {
                        if (!Node.BuildTag.EqualTo(Tag))
                        {
                            Node.BuildTag = Tag;
                            Node.BuildTags = new Tag[1];
                            Node.BuildTags[0] = Tag;
                            Node.Name = Tag.Name;
                            Node.Unique = Tag.Unique ? "True" : "False";

                            if (Tag.DecayTagId != Guid.Empty)
                            {
                                Node.DecayTags = new Tag[1];
                                Node.DecayTags[0] = Program.TagRegistry.GetTagById(Tag.DecayTagId);
                            }
                            else
                            {
                                Node.DecayTags = new Tag[0];
                            }

                            ForceUpdate = true;
                        }

                        Found = true;
                        break;
                    }
                }

                if (!Found)
                {
                    TagTreeNode Node = new TagTreeNode();
                    Node.BuildTag = Tag;
                    Node.BuildTags = new Tag[1];
                    Node.BuildTags[0] = Tag;
                    Node.Unique = Tag.Unique ? "True" : "False";
                    Node.Name = Tag.Name;
                    Node.Icon = Resources.appbar_tag;
                    if (Tag.DecayTagId != Guid.Empty)
                    {
                        Node.DecayTags = new Tag[1];
                        Node.DecayTags[0] = Program.TagRegistry.GetTagById(Tag.DecayTagId);
                    }
                    else
                    {
                        Node.DecayTags = new Tag[0];
                    }
                    Model.Nodes.Add(Node);

                    ForceUpdate = true;
                }
            }

            // Remove old tags.
            List<TagTreeNode> RemovedNodes = new List<TagTreeNode>();
            foreach (TagTreeNode Node in Model.Nodes)
            {
                bool Found = false;

                foreach (Tag Tag in InTags)
                {
                    if (Node.BuildTag.Id == Tag.Id)
                    {
                        Found = true;
                        break;
                    }
                }

                if (!Found)
                {
                    RemovedNodes.Add(Node);
                }
            }

            foreach (TagTreeNode Node in RemovedNodes)
            {
                Model.Nodes.Remove(Node);
                ForceUpdate = true;
            }

            if (ForceUpdate)
            {
                TagRenderer.InvalidateResources();
                Invalidate();
                Refresh();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectedChanged(object sender, EventArgs e)
        {
            ValidateState();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ValidateState()
        {
            TagTreeNode Node = MainTreeView.SelectedNode == null ? null : MainTreeView.SelectedNode.Tag as TagTreeNode;

            deleteToolStripMenuItem.Enabled = (Node != null);
            editToolStripMenuItem.Enabled = (Node != null);
            addTagToolStripMenuItem.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteClicked(object sender, EventArgs e)
        {
            TagTreeNode Node = MainTreeView.SelectedNode == null ? null : MainTreeView.SelectedNode.Tag as TagTreeNode;
            if (Node == null)
            {
                return;
            }

            Program.NetClient.DeleteTag(Node.BuildTag.Id);
            Program.NetClient.RequestTagList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddClicked(object sender, EventArgs e)
        {
            AddTagForm form = new AddTagForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                Program.NetClient.CreateTag(form.TagName, form.TagColor, form.TagUnique, form.TagDecayTagId);
                Program.NetClient.RequestTagList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditClicked(object sender, EventArgs e)
        {
            TagTreeNode Node = MainTreeView.SelectedNode == null ? null : MainTreeView.SelectedNode.Tag as TagTreeNode;
            if (Node == null)
            {
                return;
            }

            AddTagForm form = new AddTagForm();
            form.TagName = Node.BuildTag.Name;
            form.TagColor = Node.BuildTag.Color;
            form.TagUnique = Node.BuildTag.Unique;
            form.TagDecayTagId = Node.BuildTag.DecayTagId;
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                Node.Name = form.TagName;

                Program.NetClient.RenameTag(Node.BuildTag.Id, form.TagName, form.TagColor, form.TagUnique, form.TagDecayTagId);
                Program.NetClient.RequestTagList();
            }
        }
    }
}
