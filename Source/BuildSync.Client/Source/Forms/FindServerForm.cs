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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Networking;
using BuildSync.Core;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FindServerForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        private NetDiscoveryClient DiscoveryClient = new NetDiscoveryClient();

        /// <summary>
        /// 
        /// </summary>
        public string SelectedHostname { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int SelectedPort { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public FindServerForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoaded(object sender, EventArgs e)
        {
            DiscoveryClient.OnResposeRecieved += ResponseRecieved;
            DiscoveryClient.Discover();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="Response"></param>
        private void ResponseRecieved(NetDiscoveryClient Client, NetDiscoveryData Response)
        {
            Invoke((MethodInvoker)(() =>
            {
                foreach (ListViewItem Item in serverListView.Items)
                {
                    if (Item.SubItems[1].Text == Response.Address + ":" + Response.Port)
                    {
                        return;
                    }
                }

                string Version = Response.Version;
                if (Response.ProtocolVersion != AppVersion.ProtocolVersion)
                {
                    Version += " (Incompatible)";
                }

                ListViewItem item = new ListViewItem(new string[] { 
                    Response.Name, 
                    Response.Address + ":" + Response.Port, 
                    Version 
                });
                serverListView.Items.Add(item);
            }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(object sender, FormClosedEventArgs e)
        {
            DiscoveryClient.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedServerChanged(object sender, EventArgs e)
        {
            addServerButton.Enabled = false;

            if (serverListView.SelectedItems.Count == 0)
            {
                return;
            }

            ListViewItem SelectedItem = serverListView.SelectedItems[0];
            if (SelectedItem.SubItems[2].Text.Contains("Incompatible"))
            {
                return;
            }

            string[] split = SelectedItem.SubItems[1].Text.Split(':');

            SelectedHostname = split[0];
            SelectedPort = int.Parse(split[1]);
            addServerButton.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UseServerClicked(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
