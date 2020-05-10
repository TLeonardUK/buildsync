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
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using BuildSync.Core.Downloads;
using BuildSync.Core.Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// </summary>
    public partial class ManifestsForm : DockContent
    {
        /// <summary>
        /// </summary>
        private readonly ListViewColumnSorter ColumnSorter = new ListViewColumnSorter();

        /// <summary>
        /// 
        /// </summary>
        private ManifestDownloadState ContextMenuState = null;

        /// <summary>
        /// </summary>
        private readonly IComparer[] ColumnSorterComparers =
        {
            new CaseInsensitiveComparer(), // Id
            new CaseInsensitiveComparer(), // Virtual Path
            new CaseInsensitiveComparer(), // Local Path
            new CaseInsensitiveComparer(), // Progress
            new CaseInsensitiveComparer(), // State
            new FileSizeStringComparer(), // Disk Usage
            new TransferRateStringComparer(), // Download Speed
            new TransferRateStringComparer() // Upload Speed
        };

        /// <summary>
        /// </summary>
        public ManifestsForm()
        {
            InitializeComponent();

            WindowUtils.EnableDoubleBuffering(mainListView);

            mainListView.ListViewItemSorter = ColumnSorter;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColumnClicked(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == ColumnSorter.SortColumn)
            {
                if (ColumnSorter.Order == SortOrder.Ascending)
                {
                    ColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    ColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                ColumnSorter.SortColumn = e.Column;
                ColumnSorter.Order = SortOrder.Ascending;
                ColumnSorter.ObjectCompare = ColumnSorterComparers[e.Column];
            }

            mainListView.Sort();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(object sender, FormClosedEventArgs e)
        {
            UpdateTimer.Enabled = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, EventArgs e)
        {
            RefreshManifests();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            UpdateTimer.Enabled = true;
        }

        /// <summary>
        /// </summary>
        private void RefreshManifests()
        {
            // Add new entries.
            foreach (ManifestDownloadState Manifest in Program.ManifestDownloadManager.States.States)
            {
                bool Exists = false;

                foreach (ListViewItem Item in mainListView.Items)
                {
                    if (Item.Tag as ManifestDownloadState == Manifest)
                    {
                        Exists = true;
                        break;
                    }
                }

                if (!Exists)
                {
                    ListViewItem Item = new ListViewItem(new string[8]);
                    Item.Tag = Manifest;
                    Item.ImageIndex = 0;
                    mainListView.Items.Add(Item);
                }
            }

            // Remove old entries.
            for (int i = 0; i < mainListView.Items.Count; i++)
            {
                ListViewItem Item = mainListView.Items[i];

                ManifestDownloadState Manifest = Item.Tag as ManifestDownloadState;
                if (!Program.ManifestDownloadManager.States.States.Contains(Manifest))
                {
                    mainListView.Items.Remove(Item);
                    i--;
                }
            }

            // Update values.
            foreach (ListViewItem Item in mainListView.Items)
            {
                ManifestDownloadState Manifest = Item.Tag as ManifestDownloadState;
                Item.SubItems[0].Text = Manifest.ManifestId.ToString();
                Item.SubItems[1].Text = Manifest.Manifest != null ? Manifest.Manifest.VirtualPath : "";
                Item.SubItems[2].Text = Manifest.LocalFolder;
                Item.SubItems[3].Text = string.Format("{0:0.##}%", Manifest.Progress * 100);
                Item.SubItems[4].Text = Manifest.Paused && Manifest.State != ManifestDownloadProgressState.Complete ? "Inactive" : Manifest.State.ToString();
                Item.SubItems[5].Text = StringUtils.FormatAsSize(Manifest.Manifest != null ? Manifest.Manifest.GetTotalSize() : 0);
                Item.SubItems[6].Text = StringUtils.FormatAsTransferRate(Manifest.BandwidthStats.RateOut);
                Item.SubItems[7].Text = StringUtils.FormatAsTransferRate(Manifest.BandwidthStats.RateIn);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTimerTick(object sender, EventArgs e)
        {
            RefreshManifests();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenInExplorerClicked(object sender, EventArgs e)
        {
            if (ContextMenuState != null)
            {
                Process.Start("explorer.exe", ContextMenuState.LocalFolder);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteClicked(object sender, EventArgs e)
        {
            if (ContextMenuState.Active)
            {
                MessageBox.Show("Manifest is active for a download. Delete the download using it before deleting this manifest.", "Manifest Active", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Program.ManifestDownloadManager.PruneManifest(ContextMenuState.ManifestId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ManifestDownloadState Manifest = mainListView.SelectedItems.Count == 0 ? null : mainListView.SelectedItems[0].Tag as ManifestDownloadState;

            openInExplorerToolStripMenuItem.Enabled = (Manifest != null && Directory.Exists(Manifest.LocalFolder));
            deleteToolStripMenuItem.Enabled = (Manifest != null);

            ContextMenuState = Manifest;
        }
    }
}