﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Users;
using WeifenLuo.WinFormsUI.Docking;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Utils;
using BuildSync.Core.Controls.Graph;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ManageServerForm : DockContent
    {
        /// <summary>
        /// 
        /// </summary>
        private bool ApplyingServerState = false;

        /// <summary>
        /// 
        /// </summary>
        public ManageServerForm()
        {
            InitializeComponent();

            WindowUtils.EnableDoubleBuffering(MainListView);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
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
                series.YAxis.Max = 1024l;
                series.YAxis.GridInterval = series.YAxis.Max / 10;
                BandwithGraph.Series = new GraphSeries[1] { series };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(object sender, FormClosedEventArgs e)
        {
            Program.NetClient.OnServerStateRecieved -= ServerStateRecieved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshTicked(object sender, EventArgs e)
        {
            Program.NetClient.RequestServerState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Users"></param>
        private void ServerStateRecieved(NetMessage_GetServerStateResponse Msg)
        {
            ApplyingServerState = true;
            MaxBandwidthBox.Value = Msg.BandwidthLimit / 1024;

            // Add new items.
            foreach (NetMessage_GetServerStateResponse.ClientState State in Msg.ClientStates)
            {
                bool Exists = false;
                foreach (ListViewItem Item in MainListView.Items)
                {
                    if ((Item.Tag as string) == State.Address)
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
                    if ((Item.Tag as string) == State.Address)
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

            ApplyingServerState = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaxBandwidthBoxChanged(object sender, EventArgs e)
        {
            Program.NetClient.SetServerMaxBandwidth((long)MaxBandwidthBox.Value * 1024l);
        }
    }
}
