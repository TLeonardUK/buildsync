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
            InTags.Sort((Item1, Item2) => -Item1.Name.CompareTo(Item2.Name));

            // Add new tags.
            foreach (Tag Tag in InTags)
            {
                bool Found = false;

                foreach (TagTreeNode Node in Model.Nodes)
                {
                    if (Node.BuildTag.Id == Tag.Id)
                    {
                        Found = true;
                        break;
                    }
                }

                if (!Found)
                {
                    TagTreeNode Node = new TagTreeNode();
                    Node.BuildTag = Tag;
                    Node.Name = Tag.Name;
                    Node.Icon = Resources.appbar_tag;
                    Model.Nodes.Add(Node);
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
                Program.NetClient.CreateTag(form.TagName);
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
            form.TagName = Node.Name;
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                Node.Name = form.TagName;

                Program.NetClient.RenameTag(Node.BuildTag.Id, form.TagName);
                Program.NetClient.RequestTagList();
            }
        }
    }
}
