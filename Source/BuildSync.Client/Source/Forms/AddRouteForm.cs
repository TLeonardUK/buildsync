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
using BuildSync.Core.Users;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// </summary>
    public partial class AddRouteForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid SourceTagId = Guid.Empty;

        /// <summary>
        /// 
        /// </summary>
        public Guid DestinationTagId = Guid.Empty;

        /// <summary>
        /// 
        /// </summary>
        public bool Blacklisted = false;

        /// <summary>
        /// 
        /// </summary>
        public long BandwidthLimit = 0;

        /// <summary>
        /// 
        /// </summary>
        private bool ChangingInternal = false;

        /// <summary>
        /// </summary>
        public AddRouteForm()
        {
            InitializeComponent();

            sourceTagTextBox.MultipleTags = false;
            destinationTagTextBox.MultipleTags = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkClicked(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            ChangingInternal = true;

            sourceTagTextBox.TagIds = new List<Guid>();
            if (SourceTagId != Guid.Empty)
            {
                sourceTagTextBox.TagIds.Add(SourceTagId);
            }

            destinationTagTextBox.TagIds = new List<Guid>();
            if (DestinationTagId != Guid.Empty)
            {
                destinationTagTextBox.TagIds.Add(DestinationTagId);
            }

            blacklistCheckBox.Checked = Blacklisted;
            bandwidthLimitTextBox.Value = BandwidthLimit;

            ChangingInternal = false;

            UpdateState();
        }

        /// <summary>
        /// </summary>
        private void UpdateState()
        {
            addGroupButton.Enabled = (
                sourceTagTextBox.TagIds.Count == 1 &&
                destinationTagTextBox.TagIds.Count == 1 &&
                bandwidthLimitTextBox.Value >= 0
            );

            SourceTagId = sourceTagTextBox.TagIds.Count > 0 ? sourceTagTextBox.TagIds[0] : Guid.Empty;
            DestinationTagId = destinationTagTextBox.TagIds.Count > 0 ? destinationTagTextBox.TagIds[0] : Guid.Empty;
            Blacklisted = blacklistCheckBox.Checked;
            BandwidthLimit = bandwidthLimitTextBox.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StateChanged(object sender, EventArgs e)
        {
            if (ChangingInternal)
            {
                return;
            }

            UpdateState();
        }
    }
}