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

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PeersForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public PeersForm()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(object sender, FormClosedEventArgs e)
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

            List<BuildSyncClient.Peer> ConnectedPeers = new List<BuildSyncClient.Peer>();
            foreach (BuildSyncClient.Peer Peer in AllPeers)
            {
                if (Peer.Connection.IsConnected)
                {
                    ConnectedPeers.Add(Peer);
                }
            }

            // Remove old peers.
            List<ListViewItem> ItemsToRemove = new List<ListViewItem>();
            foreach (ListViewItem Item in MainListView.Items)
            {
                BuildSyncClient.Peer ItemPeer = Item.Tag as BuildSyncClient.Peer;
                if (!ConnectedPeers.Contains(ItemPeer))
                {
                    ItemsToRemove.Add(Item);
                }
            }
            foreach (ListViewItem Item in ItemsToRemove)
            {
                MainListView.Items.Remove(Item);
            }

            // Add new peers.
            foreach (BuildSyncClient.Peer Peer in ConnectedPeers)
            {
                ListViewItem Item = null;
                foreach (ListViewItem SubItem in MainListView.Items)
                {
                    BuildSyncClient.Peer ItemPeer = SubItem.Tag as BuildSyncClient.Peer;
                    if (ItemPeer == Peer)
                    {
                        Item = SubItem;
                        break;
                    }
                }

                if (Item == null)
                {
                    Item = new ListViewItem(new string[5]);
                    Item.Tag = Peer;
                    MainListView.Items.Add(Item);
                }

                Item.SubItems[0].Text = (Peer.RemoteInitiated ? "[Remote Initiated] " : "") + (Peer.Connection.Address == null ? "" : HostnameCache.GetHostname(Peer.Connection.Address.Address.ToString()));
                Item.SubItems[1].Text = StringUtils.FormatAsTransferRate(Peer.Connection.BandwidthStats.RateIn);
                Item.SubItems[2].Text = StringUtils.FormatAsTransferRate(Peer.Connection.BandwidthStats.RateOut);
                Item.SubItems[3].Text = StringUtils.FormatAsSize(Peer.Connection.BandwidthStats.TotalIn);
                Item.SubItems[4].Text = StringUtils.FormatAsSize(Peer.Connection.BandwidthStats.TotalOut);
            }
        }
    }
}
