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
        /// <summary>
        /// 
        /// </summary>
        private List<Tag> Tags = new List<Tag>();

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
        /// </summary>
        /// <param name="Users"></param>
        private void TagsRecieved(List<Tag> InTags)
        {
            Tags = InTags;

            // Add new tags.
            foreach (Tag Tag in InTags)
            {
                bool Found = false;
                foreach (ToolStripMenuItem Item in addTagToolStripMenuItem.DropDownItems)
                {
                    if ((Item.Tag as Tag).Id == Tag.Id)
                    {
                        Found = true;
                        break;
                    }
                }

                if (!Found)
                {
                    ToolStripMenuItem TagItem = new ToolStripMenuItem();
                    TagItem.Text = Tag.Name;
                    TagItem.Tag = Tag;
                    TagItem.ImageScaling = ToolStripItemImageScaling.None;
                    TagItem.Click += TagItemClicked;
                    addTagToolStripMenuItem.DropDownItems.Add(TagItem);

                    Logger.Log(LogLevel.Info, LogCategory.Main, "Added tag menu: {0}", Tag.Name);
                }
            }

            // Remove old tags.
            List<ToolStripMenuItem> RemovedItems = new List<ToolStripMenuItem>();
            foreach (ToolStripMenuItem Item in addTagToolStripMenuItem.DropDownItems)
            {
                bool Found = false;

                foreach (Tag Tag in InTags)
                {
                    if ((Item.Tag as Tag).Id == Tag.Id)
                    {
                        Found = true;
                        break;
                    }
                }

                if (!Found)
                {
                    RemovedItems.Add(Item);
                }
            }

            foreach (ToolStripMenuItem Item in RemovedItems)
            {
                Logger.Log(LogLevel.Info, LogCategory.Main, "Removing tag menu: {0}", Item.Text);
                addTagToolStripMenuItem.DropDownItems.Remove(Item);
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

            Guid ManifestId = downloadFileSystemTree.SelectedManifestId;
            if (ManifestId == Guid.Empty)
            {
                return;
            }

            Tag Tag = TagItem.Tag as Tag;

            if (!TagItem.Checked)
            {
                Program.NetClient.AddTagToManifest(ManifestId, Tag.Id);
            }
            else
            {
                Program.NetClient.RemoveTagFromManifest(ManifestId, Tag.Id);
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
                foreach (ToolStripMenuItem Item in addTagToolStripMenuItem.DropDownItems)
                {
                    Tag MenuTag = (Item.Tag as Tag);

                    bool Found = false;
                    foreach (Tag BuildTag in Tags)
                    {
                        if (MenuTag.Id == BuildTag.Id)
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