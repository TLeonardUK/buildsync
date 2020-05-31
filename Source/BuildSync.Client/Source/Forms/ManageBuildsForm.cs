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
using BuildSync.Client.Controls;
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
        private TagToolstripBuilder TagBuilder = new TagToolstripBuilder();

        /// <summary>
        /// </summary>
        public ManageBuildsForm()
        {
            InitializeComponent();

            downloadFileSystemTree.CanSelectBuildContainers = false;

            ValidateState();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(object sender, FormClosedEventArgs e)
        {
            TagBuilder.OnTagClicked -= TagItemClicked;
            TagBuilder.OnTagsRefreshed -= ValidateState;
            TagBuilder.Detach();
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormShown(object sender, EventArgs e)
        {
            TagBuilder.OnTagClicked += TagItemClicked;
            TagBuilder.OnTagsRefreshed += ValidateState;
            TagBuilder.Attach(addTagToolStripMenuItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TagBuilder.Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TagItemClicked(Tag Tag, bool Checked)
        {
            Guid ManifestId = downloadFileSystemTree.SelectedManifestId;
            if (ManifestId == Guid.Empty)
            {
                return;
            }

            if (!Checked)
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoteInstallClicked(object sender, EventArgs e)
        {
            RemoteInstallForm Form = new RemoteInstallForm();
            Form.ManifestId = downloadFileSystemTree.SelectedManifestId;
            Form.ManifestPath = downloadFileSystemTree.SelectedPath;
            Form.ShowDialog(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiffBuildsClicked(object sender, EventArgs e)
        {
            DiffManifestForm Form = new DiffManifestForm();
            Form.SourceManifestId = downloadFileSystemTree.SelectedManifestIds[0];
            Form.DestinationManifestId = downloadFileSystemTree.SelectedManifestIds[1];
            Form.ShowDialog(this);
        }

        /// <summary>
        /// 
        /// </summary>d
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExampleBuildClicked(object sender, EventArgs e)
        {
            ExamineManifestForm Form = new ExamineManifestForm();
            Form.ManifestId = downloadFileSystemTree.SelectedManifestIds[0];
            Form.ShowDialog(this);
        }

        /// <summary>
        /// </summary>
        private void ValidateState()
        {
            bool CanManage = Program.NetClient.Permissions.HasPermission(UserPermissionType.Write, "", false, true);
            bool CanTag = Program.NetClient.Permissions.HasPermission(UserPermissionType.TagBuilds, "", false, true);

            int SelectedManifestCount = downloadFileSystemTree.SelectedManifestIds.Count;

            deleteToolStripMenuItem.Enabled = CanManage && (downloadFileSystemTree.SelectedManifestId != Guid.Empty);
            addDownloadToolStripMenuItem1.Enabled = CanManage && (downloadFileSystemTree.SelectedManifestId == Guid.Empty);
            addTagToolStripMenuItem.Enabled = CanTag && (downloadFileSystemTree.SelectedManifestId != Guid.Empty);
            downloadToolStripMenuItem.Enabled = (downloadFileSystemTree.SelectedManifestId != Guid.Empty || downloadFileSystemTree.IsSelectedBuildContainer);
            remoteInstallToolStripMenuItem.Enabled = (downloadFileSystemTree.SelectedManifestId != Guid.Empty);
            diffManifestsToolStripMenuItem.Enabled = (SelectedManifestCount == 2);
            examineBuildToolStripMenuItem.Enabled = (downloadFileSystemTree.SelectedManifestId != Guid.Empty);

            List<Tag> Tags = downloadFileSystemTree.SelectedManifestTags;
            if (Tags != null)
            {
                List<Guid> TagIds = new List<Guid>();
                foreach (Tag BuildTag in Tags)
                {
                    TagIds.Add(BuildTag.Id);
                }
                TagBuilder.SetCheckedTags(TagIds);
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