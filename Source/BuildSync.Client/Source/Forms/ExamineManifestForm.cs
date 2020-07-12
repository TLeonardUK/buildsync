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
using BuildSync.Core.Utils;
using BuildSync.Core.Client;
using BuildSync.Core.Manifests;
using BuildSync.Core.Downloads;
using BuildSync.Client.Controls;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ExamineManifestForm : Form
    {
        public Guid ManifestId = Guid.Empty;

        private bool DiffGenerated = false;
        private bool RequestedManifests = false;

        /// <summary>
        /// 
        /// </summary>
        public ExamineManifestForm()
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
            UpdateDiff();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTicked(object sender, EventArgs e)
        {
            UpdateDiff();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateDiff()
        {
            if (DiffGenerated)
            {
                return;
            }

            BuildManifest SourceManifest = Program.BuildRegistry.GetManifestById(ManifestId);

            if (SourceManifest == null)
            {
                if (!RequestedManifests)
                {
                    Program.NetClient.RequestManifest(ManifestId);

                    RequestedManifests = true;
                }

                return;
            }

            // Show manifest info.
            manifestSizeLabel.Text = StringUtils.FormatAsSize(SourceManifest.GetTotalSize());
            manifestBlocksLabel.Text = SourceManifest.BlockCount.ToString();
            manifestGuidLabel.Text = SourceManifest.Guid.ToString();

            // Generate diffs.
            List<BuildManifestFileDiff> SourceDiff = SourceManifest.Diff(null);
            SourceManifestDeltaTree.ApplyDiff(SourceManifest, SourceDiff);

            DiffGenerated = true;
            UpdateStats();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        private void SelectedNodeChanged(string Path)
        {
            UpdateStats();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateStats()
        {
            BuildManifestFileDiff Diff = SourceManifestDeltaTree.SelectedDiff;

            if (Diff == null)
            {
                pathLabel.Text = "...";
                blockLabel.Text = "...";
                checksumLabel.Text = "...";
            }
            else
            {
                pathLabel.Text = Diff.FileInfo.Path;
                blockLabel.Text = (Diff.FileInfo.LastBlockIndex - Diff.FileInfo.FirstBlockIndex).ToString();
                checksumLabel.Text = Diff.FileInfo.Checksum;
            }
        }

        private void ExamineManifestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void ExamineManifestForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
