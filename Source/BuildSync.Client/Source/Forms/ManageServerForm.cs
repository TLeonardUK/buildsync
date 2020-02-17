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
using System.Windows.Forms;
using BuildSync.Core.Controls.Graph;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Users;
using BuildSync.Core.Utils;
using WeifenLuo.WinFormsUI.Docking;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// </summary>
    public partial class ManageServerForm : DockContent
    {
        /// <summary>
        /// </summary>
        private readonly ListViewColumnSorter ColumnSorter = new ListViewColumnSorter();

        /// <summary>
        /// </summary>
        private readonly IComparer[] ColumnSorterComparers =
        {
            new CaseInsensitiveComparer(), // Hostname
            new TransferRateStringComparer(), // Download Speed
            new TransferRateStringComparer(), // Upload Speed
            new FileSizeStringComparer(), // Total Downloaded
            new FileSizeStringComparer(), // Total Uploaded
            new CaseInsensitiveComparer(), // Connected Peer Count
            new FileSizeStringComparer(), // Disk Usage
            new CaseInsensitiveComparer() // Version
        };

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
            Program.NetClient.SetServerMaxBandwidth((long) MaxBandwidthBox.Value * 1024L);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(object sender, FormClosedEventArgs e)
        {
            Program.NetClient.OnServerStateRecieved -= ServerStateRecieved;

            RefreshTimer.Enabled = false;
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
                series.YAxis.GridInterval = series.YAxis.Max / 10;
                BandwithGraph.Series = new GraphSeries[1] {series};
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshTicked(object sender, EventArgs e)
        {
            if (!Program.NetClient.Permissions.HasPermission(UserPermissionType.ManageServer, "", false, true))
            {
                Hide();
                return;
            }

            Program.NetClient.RequestServerState();
        }

        /// <summary>
        /// </summary>
        /// <param name="Users"></param>
        private void ServerStateRecieved(NetMessage_GetServerStateResponse Msg)
        {
            MaxBandwidthBox.Value = Msg.BandwidthLimit / 1024;

            // Add new items.
            foreach (NetMessage_GetServerStateResponse.ClientState State in Msg.ClientStates)
            {
                bool Exists = false;
                foreach (ListViewItem Item in MainListView.Items)
                {
                    if (Item.Tag as string == State.Address)
                    {
                        Exists = true;
                        break;
                    }
                }

                if (!Exists)
                {
                    ListViewItem Item = new ListViewItem(new string[8]);
                    Item.Tag = State.Address;
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
                    if (Item.Tag as string == State.Address)
                    {
                        Item.SubItems[0].Text = HostnameCache.GetHostname(State.Address);
                        Item.SubItems[1].Text = StringUtils.FormatAsTransferRate(State.DownloadRate);
                        Item.SubItems[2].Text = StringUtils.FormatAsTransferRate(State.UploadRate);
                        Item.SubItems[3].Text = StringUtils.FormatAsSize(State.TotalDownloaded);
                        Item.SubItems[4].Text = StringUtils.FormatAsSize(State.TotalUploaded);
                        Item.SubItems[5].Text = State.ConnectedPeerCount.ToString();
                        Item.SubItems[6].Text = StringUtils.FormatAsSize(State.DiskUsage);
                        Item.SubItems[7].Text = State.Version;

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