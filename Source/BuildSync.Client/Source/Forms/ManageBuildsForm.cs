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
using System.Windows.Forms;
using BuildSync.Core.Utils;
using BuildSync.Core.Manifests;
using BuildSync.Core.Users;
using BuildSync.Core.Tags;
using WeifenLuo.WinFormsUI.Docking;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// </summary>
    public partial class ManageBuildsForm : DockContent
    {
        private class ItemContext
        {
            public Tag Tag;
            public ToolStripMenuItem Parent;
            public string Path;
        }

        /// <summary>
        /// 
        /// </summary>
        private List<Tag> Tags = new List<Tag>();

        /// <summary>
        /// 
        /// </summary>
        private List<ToolStripMenuItem> MenuItems = new List<ToolStripMenuItem>();

        /// <summary>
        /// </summary>
        public ManageBuildsForm()
        {
            InitializeComponent();

            downloadFileSystemTree.CanSelectBuildContainers = false;

            ValidateState();

            Program.NetClient.OnTagListRecieved += TagsRecieved;
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormShown(object sender, EventArgs e)
        {
            Program.NetClient.RequestTagList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Program.NetClient.RequestTagList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        private ToolStripMenuItem CreateMenuItem(string Name, Tag tag, List<ToolStripMenuItem> TouchedItems)
        {
            ToolStripMenuItem Parent = null;

            string[] Split = Name.Split('/');
            if (Split.Length > 1)
            {
                string ParentName = "";
                for (int i = 0; i < Split.Length - 1; i++)
                {
                    if (ParentName.Length > 0)
                    {
                        ParentName += "/";
                    }
                    ParentName += Split[i];
                }

                Parent = CreateMenuItem(ParentName, null, TouchedItems);
            }

            foreach (ToolStripMenuItem ExistingItem in MenuItems)
            {
                if ((ExistingItem.Tag as ItemContext).Path == Name)
                {
                    if (tag != null)
                    {
                        (ExistingItem.Tag as ItemContext).Tag = tag;
                    }
                    TouchedItems.Add(ExistingItem);
                    return ExistingItem;
                }
            }

            ItemContext Context = new ItemContext();
            Context.Parent = Parent;
            Context.Path = Name;
            Context.Tag = tag;

            ToolStripMenuItem Item = new ToolStripMenuItem();
            Item.Text = Split[Split.Length - 1];
            Item.Tag = Context;
            Item.ImageScaling = ToolStripItemImageScaling.None;
            Item.Click += TagItemClicked;
            addTagToolStripMenuItem.DropDownItems.Add(Item);
            
            if (Parent == null)
            {
                addTagToolStripMenuItem.DropDownItems.Add(Item);
            }
            else
            {
                Parent.DropDownItems.Add(Item);
            }

            MenuItems.Add(Item);
            TouchedItems.Add(Item);

            return Item;
        }

        /// <summary>
        /// </summary>
        /// <param name="Users"></param>
        private void TagsRecieved(List<Tag> InTags)
        {
            Tags = InTags;

            List<ToolStripMenuItem> ValidMenuItems = new List<ToolStripMenuItem>();

            // Add each tag.
            foreach (Tag tag in InTags)
            {
                tag.Name = tag.Name.Replace("\\", "/");
                CreateMenuItem(tag.Name.Replace("\\", "/"), tag, ValidMenuItems);
            }

            // Remove tags that no longer exist.
            for (int i = 0; i < MenuItems.Count; i++)
            {
                ToolStripMenuItem Menu = MenuItems[i];
                if (!ValidMenuItems.Contains(Menu))
                {
                    ItemContext Context = Menu.Tag as ItemContext;

                    if (Context.Parent == null)
                    {
                        addTagToolStripMenuItem.DropDownItems.Remove(Menu);
                    }
                    else
                    {
                        Context.Parent.DropDownItems.Remove(Menu);
                    }

                    Logger.Log(LogLevel.Info, LogCategory.Main, "Removing tag menu: {0}", Menu.Text);
                    MenuItems.RemoveAt(i);
                    i--;
                }
            }

            ValidateState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TagItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem TagItem = sender as ToolStripMenuItem;
            if (TagItem == null)
            {
                return;
            }

            ItemContext Context = TagItem.Tag as ItemContext;
            if (Context == null || Context.Tag == null)
            {
                return;
            }

            Guid ManifestId = downloadFileSystemTree.SelectedManifestId;
            if (ManifestId == Guid.Empty)
            {
                return;
            }

            Guid TagId = Context.Tag.Id;

            if (!TagItem.Checked)
            {
                Program.NetClient.AddTagToManifest(ManifestId, TagId);
            }
            else
            {
                Program.NetClient.RemoveTagFromManifest(ManifestId, TagId);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBuildClicked(object sender, EventArgs e)
        {
            PublishBuildForm Form = new PublishBuildForm();
            Form.VirtualPath = downloadFileSystemTree.SelectedPathRaw != "" ? downloadFileSystemTree.SelectedPathRaw + "/1" : "";
            Form.ShowDialog(this);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButtonClicked(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DateStateChanged(object sender, EventArgs e)
        {
            ValidateState();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveBuildClicked(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete '" + downloadFileSystemTree.SelectedPath + "'.", "Delete Build", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (!Program.NetClient.DeleteManifest(downloadFileSystemTree.SelectedManifestId))
                {
                    /// ???
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadClicked(object sender, EventArgs e)
        {
            AddDownloadForm Form = new AddDownloadForm();
            Form.SelectedPath = downloadFileSystemTree.SelectedPathRaw;
            Form.ShowDialog(this);
        }

        /// <summary>
        /// </summary>
        private void ValidateState()
        {
            bool CanManage = Program.NetClient.Permissions.HasPermission(UserPermissionType.Write, "", false, true);
            bool CanTag = Program.NetClient.Permissions.HasPermission(UserPermissionType.TagBuilds, "", false, true);

            deleteToolStripMenuItem.Enabled = CanManage && (downloadFileSystemTree.SelectedManifestId != Guid.Empty);
            addDownloadToolStripMenuItem1.Enabled = CanManage && (downloadFileSystemTree.SelectedManifestId == Guid.Empty);
            addTagToolStripMenuItem.Enabled = CanTag && (downloadFileSystemTree.SelectedManifestId != Guid.Empty);
            downloadToolStripMenuItem.Enabled = (downloadFileSystemTree.SelectedManifestId != Guid.Empty || downloadFileSystemTree.IsSelectedBuildContainer);

            List<Tag> Tags = downloadFileSystemTree.SelectedManifestTags;
            if (Tags != null)
            {
                foreach (ToolStripMenuItem Item in MenuItems)
                {
                    ItemContext Context = Item.Tag as ItemContext;
                    if (Context == null || Context.Tag == null)
                    {
                        continue;
                    }

                    bool Found = false;
                    foreach (Tag BuildTag in Tags)
                    {
                        if (Context.Tag.Id == BuildTag.Id)
                        {
                            Found = true;
                            break;
                        }
                    }

                    Item.Checked = Found;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
        }
    }
}