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

using BuildSync.Core;
using BuildSync.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PeersForm : DockContent
    {
        /// <summary>
        /// 
        /// </summary>
        private ListViewColumnSorter ColumnSorter = new ListViewColumnSorter();

        /// <summary>
        /// 
        /// </summary>
        private IComparer[] ColumnSorterComparers = new IComparer[] {
            new CaseInsensitiveComparer(),      // Hostname
            new TransferRateStringComparer(),   // Down Rate
            new TransferRateStringComparer(),   // Peak Down Rate
            new TransferRateStringComparer(),   // Up Rate
            new TransferRateStringComparer(),   // Peak Up Rate
            new FileSizeStringComparer(),       // Total Down
            new FileSizeStringComparer(),       // Total Up
            new CaseInsensitiveComparer(),      // Last Seen
            new FileSizeStringComparer(),       // Target In-Flight
            new FileSizeStringComparer(),       // Current In-Flight
            new CaseInsensitiveComparer(),      // RTT
            new CaseInsensitiveComparer()       // Min RTT
        };

        /// <summary>
        /// 
        /// </summary>
        public PeersForm()
        {
            InitializeComponent();

            WindowUtils.EnableDoubleBuffering(MainListView);

            MainListView.ListViewItemSorter = ColumnSorter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            ListUpdateTimer.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(object sender, FormClosedEventArgs e)
        {
            ListUpdateTimer.Enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartClosing(object sender, FormClosingEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListUpdateTimerTick(object sender, EventArgs e)
        {
            Peer[] AllPeers = Program.NetClient.AllPeers;

            List<string> ConnectedAddresses = new List<string>();
            foreach (Peer Peer in AllPeers)
            {
                if (Peer.Connection.IsConnected && Peer.Connection.Address != null)
                {
                    ConnectedAddresses.Add(Peer.Connection.Address.Address.ToString());
                }
            }

            foreach (PeerSettingsRecord Peer in Program.Settings.PeerRecords)
            {
                ListViewItem Item = null;
                foreach (ListViewItem SubItem in MainListView.Items)
                {
                    PeerSettingsRecord ItemPeer = SubItem.Tag as PeerSettingsRecord;
                    if (ItemPeer == Peer)
                    {
                        Item = SubItem;
                        break;
                    }
                }

                if (Item == null)
                {
                    Item = new ListViewItem(new string[12]);
                    Item.Tag = Peer;
                    Item.ImageIndex = 0;
                    Item.StateImageIndex = 0;
                    MainListView.Items.Add(Item);

                    MainListView.Sort();
                }

                Item.Group = ConnectedAddresses.Contains(Peer.Address) ? MainListView.Groups[0] : MainListView.Groups[1];
                Item.SubItems[0].Text = HostnameCache.GetHostname(Peer.Address);
                Item.SubItems[1].Text = StringUtils.FormatAsTransferRate((long)Peer.AverageRateIn);
                Item.SubItems[2].Text = StringUtils.FormatAsTransferRate((long)Peer.PeakRateIn);
                Item.SubItems[3].Text = StringUtils.FormatAsTransferRate((long)Peer.AverageRateOut);
                Item.SubItems[4].Text = StringUtils.FormatAsTransferRate((long)Peer.PeakRateOut);
                Item.SubItems[5].Text = StringUtils.FormatAsSize(Peer.TotalIn);
                Item.SubItems[6].Text = StringUtils.FormatAsSize(Peer.TotalOut);
                Item.SubItems[7].Text = Peer.LastSeen.ToString("dd/MM/yyyy HH:mm");
                Item.SubItems[8].Text = StringUtils.FormatAsSize((long)Peer.TargetInFlightData);
                Item.SubItems[9].Text = StringUtils.FormatAsSize((long)Peer.CurrentInFlightData);
                Item.SubItems[10].Text = string.Format("{0} ms", Peer.Ping);
                Item.SubItems[10].Text = string.Format("{0} ms", Peer.BestPing);
            }
        }

        /// <summary>
        /// 
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
    }
}
