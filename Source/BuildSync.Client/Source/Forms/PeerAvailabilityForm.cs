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
using BuildSync.Core.Client;
using BuildSync.Core.Downloads;
using BuildSync.Client.Controls;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PeerAvailabilityForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public Peer Peer = null;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, ManifestAvailabilityPanel> Panels = new Dictionary<string, ManifestAvailabilityPanel>();

        /// <summary>
        /// 
        /// </summary>
        public PeerAvailabilityForm()
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
            UpdateAvailability();

            Text = "Manifest Availability - " + Peer.Connection.Handshake.Username;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTicked(object sender, EventArgs e)
        {
            UpdateAvailability();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateAvailability()
        {
            // Add new states.
            foreach (ManifestBlockListState State in Peer.BlockState.States)
            {
                if (State.BlockState.AreAllSet(false))
                {
                    continue;
                }

                if (!Panels.ContainsKey(State.Id.ToString()))
                {
                    ManifestAvailabilityPanel Panel = new ManifestAvailabilityPanel();
                    Panel.Dock = DockStyle.Top;
                    Panel.BlockListState = State;
                    Controls.Add(Panel);

                    Panels.Add(State.Id.ToString(), Panel);
                }
            }

            // Removed states.
            string[] Keys = Panels.Keys.ToArray<string>();
            for (int i = 0; i < Keys.Length; i++)
            {
                ManifestAvailabilityPanel Panel = Panels[Keys[i]];

                bool Exists = false;
                foreach (ManifestBlockListState State in Peer.BlockState.States)
                {
                    if (State.BlockState.AreAllSet(false))
                    {
                        continue;
                    }

                    if (Panel.BlockListState.Id == State.Id)
                    {
                        Exists = true;
                        break;
                    }
                }

                if (!Exists)
                {
                    Controls.Remove(Panel);
                    Panels.Remove(Keys[i]);
                    i--;
                }
            }

            // Update the availability state.
            foreach (var Val in Panels)
            {
                Val.Value.UpdateState();
            }
        }
    }
}
