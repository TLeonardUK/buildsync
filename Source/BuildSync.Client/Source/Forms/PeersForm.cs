using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core;
using BuildSync.Core.Utils;
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
        private void Closing(object sender, FormClosingEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListUpdateTimerTick(object sender, EventArgs e)
        {
            BuildSyncClient.Peer[] AllPeers = Program.NetClient.AllPeers;

            List<string> ConnectedAddresses = new List<string>();
            foreach (BuildSyncClient.Peer Peer in AllPeers)
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
                    Item = new ListViewItem(new string[9]);
                    Item.Tag = Peer;
                    Item.ImageIndex = 0;
                    Item.StateImageIndex = 0;
                    MainListView.Items.Add(Item);

                    MainListView.Sort();
                }

                Item.Group = ConnectedAddresses.Contains(Peer.Address) ? MainListView.Groups[0] : MainListView.Groups[1];
                Item.SubItems[0].Text = HostnameCache.GetHostname(Peer.Address);
                Item.SubItems[1].Text = string.Format("{0} ({1})", StringUtils.FormatAsTransferRate((long)Peer.AverageRateIn), StringUtils.FormatAsTransferRate((long)Peer.PeakRateIn));
                Item.SubItems[2].Text = string.Format("{0} ({1})", StringUtils.FormatAsTransferRate((long)Peer.AverageRateOut), StringUtils.FormatAsTransferRate((long)Peer.PeakRateOut));
                Item.SubItems[3].Text = StringUtils.FormatAsSize((long)Peer.TotalIn);
                Item.SubItems[4].Text = StringUtils.FormatAsSize((long)Peer.TotalOut);
                Item.SubItems[5].Text = Peer.LastSeen.ToString("dd/MM/yyyy HH:mm");
                Item.SubItems[6].Text = StringUtils.FormatAsSize((long)Peer.TargetInFlightData);
                Item.SubItems[7].Text = StringUtils.FormatAsSize((long)Peer.CurrentInFlightData);
                Item.SubItems[8].Text = string.Format("{0}ms ({1}ms)", Peer.Ping, Peer.BestPing);
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

                if (e.Column == 1 || // Download Speed
                    e.Column == 2 || // Upload Speed
                    e.Column == 3 || // Total Downloaded
                    e.Column == 4 || // Total Uploaded
                    e.Column == 6 || // Target In-Flight
                    e.Column == 7)   // Current In-Flight
                {
                    ColumnSorter.SortType = typeof(float);
                }
                else
                {
                    ColumnSorter.SortType = typeof(string);
                }
            }

            MainListView.Sort();
        }
    }
}
