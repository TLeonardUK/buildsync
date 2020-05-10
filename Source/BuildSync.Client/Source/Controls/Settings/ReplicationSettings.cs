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
using BuildSync.Client.Forms;
using BuildSync.Core.Scm;

namespace BuildSync.Client.Controls.Settings
{
    /// <summary>
    ///     Control thats displayed in the <see cref="SettingsForm" /> to allow the user to configure all
    ///     replication settings.
    /// </summary>
    public partial class ReplicationSettings : SettingsControlBase
    {
        private readonly bool SkipValidity;

        /// <summary>
        ///     Gets the title displayed over the settings when this control is displayed.
        /// </summary>
        public override string GroupName => "Replication Settings";

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerSettings" /> class.
        /// </summary>
        public ReplicationSettings()
        {
            InitializeComponent();

            SkipValidity = true;
            AutoDownloadBuildsCheckbox.Checked = Program.Settings.AutoReplicate;
            selectTagsTextBox.TagIds = new List<Guid>(Program.Settings.ReplicateSelectTags);
            ignoreTagsTextBox.TagIds = new List<Guid>(Program.Settings.ReplicateIgnoreTags);
            SkipValidity = false;

            UpdateValidityState();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StateChanged(object sender, EventArgs e)
        {
            UpdateValidityState();
        }

        /// <summary>
        /// </summary>
        private void UpdateValidityState()
        {
            if (SkipValidity)
            {
                return;
            }

            Program.Settings.AutoReplicate = AutoDownloadBuildsCheckbox.Checked;
            Program.Settings.ReplicateSelectTags = selectTagsTextBox.TagIds;
            Program.Settings.ReplicateIgnoreTags = ignoreTagsTextBox.TagIds;
        }
    }
}