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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using BuildSync.Core.Controls.Graph;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Users;
using BuildSync.Core.Utils;
using BuildSync.Core.Tags;
using BuildSync.Client.Controls;
using WeifenLuo.WinFormsUI.Docking;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// </summary>
    public partial class ManageServerForm : DockContent
    {
        /// <summary>
        /// 
        /// </summary>
        private NetMessage_GetServerStateResponse.ClientState SelectedClient;

        /// <summary>
        /// </summary>
        private readonly ListViewColumnSorter ColumnSorter = new ListViewColumnSorter();

        /// <summary>
        /// </summary>
        private readonly IComparer[] ColumnSorterComparers =
        {
            new CaseInsensitiveComparer(), // Username
            new CaseInsensitiveComparer(), // Hostname
            new CaseInsensitiveComparer(), // Tags
            new TransferRateStringComparer(), // Download Speed
            new TransferRateStringComparer(), // Upload Speed
            new FileSizeStringComparer(), // Total Downloaded
            new FileSizeStringComparer(), // Total Uploaded
            new CaseInsensitiveComparer(), // Connected Peer Count
            new FileSizeStringComparer(), // Disk Usage
            new FileSizeStringComparer(), // Disk Quota
            new CaseInsensitiveComparer() // Version
        };

        /// <summary>
        /// 
        /// </summary>
        private TagToolstripBuilder TagBuilder = new TagToolstripBuilder();

        /// <summary>
        /// </summary>
        public ManageServerForm()
        {
            InitializeComponent();

            WindowUtils.EnableDoubleBuffering(MainListView);

            MainListView.ListViewItemSorter = ColumnSorter;
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

            MainListView.Sort();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaxBandwidthBoxChanged(object sender, EventArgs e)
        {
            Program.NetClient.SetServerMaxBandwidth((long) MaxBandwidthBox.Value);
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

            Program.NetClient.OnServerStateRecieved -= ServerStateRecieved;

            RefreshTimer.Enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MainListView.SelectedItems.Count > 0)
            {
                SelectedClient = MainListView.SelectedItems[0].Tag as NetMessage_GetServerStateResponse.ClientState;
            }

            TagBuilder.Refresh();
        }

        /// <summary>
        /// </summary>
        private void ValidateState()
        {
            addTagToolStripMenuItem.Enabled = SelectedClient != null && addTagToolStripMenuItem.DropDownItems.Count > 0;

            if (SelectedClient != null)
            { 
                TagBuilder.SetCheckedTags(SelectedClient.TagIds);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TagItemClicked(Tag Tag, bool Checked)
        {
            if (SelectedClient == null)
            {
                return;
            }

            if (!Checked)
            {
                Program.NetClient.AddTagToClient(SelectedClient.Address, Tag.Id);
            }
            else
            {
                Program.NetClient.RemoveTagFromClient(SelectedClient.Address, Tag.Id);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            RefreshTimer.Enabled = true;

            Program.NetClient.OnServerStateRecieved += ServerStateRecieved;
            Program.NetClient.RequestServerState();

            TagBuilder.OnTagClicked += TagItemClicked;
            TagBuilder.OnTagsRefreshed += ValidateState;
            TagBuilder.Attach(addTagToolStripMenuItem);

            if (BandwithGraph.Series.Length == 0 || BandwithGraph.Series[0] == null)
            {
                GraphSeries series = new GraphSeries();
                series.Name = "Bandwidth Usage";
                series.SlidingWindow = true;
                series.XAxis.MinLabel = "";
                series.XAxis.MaxLabel = "5 Minutes";
                series.XAxis.Min = 0;
                series.XAxis.Max = 5 * 60;
                series.XAxis.GridInterval = 30;
                series.YAxis.MinLabel = "";
                series.YAxis.MaxLabel = "1 KB/s";
                series.YAxis.AutoAdjustMax = true;
                series.YAxis.FormatMaxLabelAsTransferRate = true;
                series.MinimumInterval = 1.0f / 2.0f;
                series.YAxis.Max = 1024L;
                series.YAxis.GridInterval = series.YAxis.Max / 5;
                BandwithGraph.Series = new GraphSeries[1] {series};
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshTicked(object sender, EventArgs e)
        {
            if (!Program.NetClient.Permissions.HasPermission(UserPermissionType.ModifyServer, "", false, true))
            {
                Hide();
                return;
            }

            Program.NetClient.RequestServerState();
            TagBuilder.Refresh();
        }

        /// <summary>
        /// </summary>
        /// <param name="Users"></param>
        private void ServerStateRecieved(NetMessage_GetServerStateResponse Msg)
        {
            MaxBandwidthBox.Value = Msg.BandwidthLimit;

            // Add new items.
            foreach (NetMessage_GetServerStateResponse.ClientState State in Msg.ClientStates)
            {
                bool Exists = false;
                foreach (ListViewItem Item in MainListView.Items)
                {
                    if ((Item.Tag as NetMessage_GetServerStateResponse.ClientState).Address == State.Address)
                    {
                        Exists = true;
                        break;
                    }
                }

                if (!Exists)
                {
                    ListViewItem Item = new ListViewItem(new string[11]);
                    Item.Tag = State;
                    Item.ImageIndex = 0;

                    MainListView.Items.Add(Item);
                }
            }

            // Remove old items or update existing updates.
            for (int i = 0; i < MainListView.Items.Count; i++)
            {
                ListViewItem Item = MainListView.Items[i];

                bool Exists = false;
                foreach (NetMessage_GetServerStateResponse.ClientState State in Msg.ClientStates)
                {
                    if ((Item.Tag as NetMessage_GetServerStateResponse.ClientState).Address == State.Address)
                    {
                        Item.SubItems[0].Text = State.Username;
                        Item.SubItems[1].Text = HostnameCache.GetHostname(State.Address);
                        Item.SubItems[2].Text = Program.TagRegistry.IdsToString(State.TagIds);
                        Item.SubItems[3].Text = StringUtils.FormatAsTransferRate(State.DownloadRate);
                        Item.SubItems[4].Text = StringUtils.FormatAsTransferRate(State.UploadRate);
                        Item.SubItems[5].Text = StringUtils.FormatAsSize(State.TotalDownloaded);
                        Item.SubItems[6].Text = StringUtils.FormatAsSize(State.TotalUploaded);
                        Item.SubItems[7].Text = State.ConnectedPeerCount.ToString();
                        Item.SubItems[8].Text = StringUtils.FormatAsSize(State.DiskUsage);
                        Item.SubItems[9].Text = StringUtils.FormatAsSize(State.DiskQuota);
                        Item.SubItems[10].Text = State.Version;
                        Item.Tag = State;

                        Exists = true;
                        break;
                    }
                }

                if (!Exists)
                {
                    MainListView.Items.Remove(Item);
                    i--;
                }
            }

            // Update total bandwidth.
            long TotalBandwidth = 0;
            foreach (NetMessage_GetServerStateResponse.ClientState State in Msg.ClientStates)
            {
                TotalBandwidth += State.DownloadRate;
            }

            BandwithGraph.Series[0].AddDataPoint(Environment.TickCount / 1000.0f, TotalBandwidth);
        }
    }
}