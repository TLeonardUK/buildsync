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
    public partial class DiffManifestForm : Form
    {
        public Guid SourceManifestId = Guid.Empty;
        public Guid DestinationManifestId = Guid.Empty;

        private bool DiffGenerated = false;
        private bool RequestedManifests = false;

        /// <summary>
        /// 
        /// </summary>
        public DiffManifestForm()
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

            BuildManifest SourceManifest = Program.BuildRegistry.GetManifestById(SourceManifestId);
            BuildManifest DestinationManifest = Program.BuildRegistry.GetManifestById(DestinationManifestId);

            if (SourceManifest == null || DestinationManifest == null)
            {
                if (!RequestedManifests)
                {
                    Program.NetClient.RequestManifest(SourceManifestId);
                    Program.NetClient.RequestManifest(DestinationManifestId);

                    RequestedManifests = true;
                }

                return;
            }

            int MatchingFiles = 0;
            long MatchingFilesSize = 0;
            int MatchingBlocks = 0;
            long MatchingBlocksSize = 0;

            HashSet<string> FileHashes = new HashSet<string>();
            HashSet<uint> BlockHashes = new HashSet<uint>();

            foreach (BuildManifestFileInfo File in SourceManifest.GetFiles())
            {
                FileHashes.Add(File.Checksum);
            }

            for (int i = 0; i < SourceManifest.BlockCount; i++)
            {
                BlockHashes.Add(SourceManifest.GetBlockChecksum(i));
            }

            foreach (BuildManifestFileInfo File in DestinationManifest.GetFiles())
            {
                if (FileHashes.Contains(File.Checksum))
                {
                    MatchingFiles++;
                    MatchingFilesSize += File.Size;
                }
            }

            for (int i = 0; i < DestinationManifest.BlockCount; i++)
            {
                if (BlockHashes.Contains(DestinationManifest.GetBlockChecksum(i)))
                {
                    BuildManifestBlockInfo Info = new BuildManifestBlockInfo();
                    if (DestinationManifest.GetBlockInfo(i, ref Info))
                    {
                        MatchingBlocks++;
                        MatchingBlocksSize += Info.TotalSize;
                    }
                }
            }

            // Show manifest info.
            manifestSizeLabel.Text = string.Format("{0} (Delta {1})", 
                StringUtils.FormatAsSize(DestinationManifest.GetTotalSize()),
                StringUtils.FormatAsSize(DestinationManifest.GetTotalSize() - SourceManifest.GetTotalSize()));

            manifestBlocksLabel.Text = string.Format("{0} (Delta {1})",
                DestinationManifest.BlockCount,
                DestinationManifest.BlockCount - SourceManifest.BlockCount);

            manifestGuidLabel.Text = SourceManifest.Guid + " / " + DestinationManifest.Guid;
            matchingFilesLabel.Text = MatchingFiles + " (" + StringUtils.FormatAsSize(MatchingFilesSize) + ")";
            matchingBlocksLabel.Text = MatchingBlocks + " (" + StringUtils.FormatAsSize(MatchingBlocksSize) + ")";

            // Generate diffs.
            List<BuildManifestFileDiff> SourceDiff = SourceManifest.Diff(null);
            SourceManifestDeltaTree.ApplyDiff(SourceManifest, SourceDiff);

            List<BuildManifestFileDiff> DestinationDiff = SourceManifest.Diff(DestinationManifest);
            DestinationManifestDeltaTree.ApplyDiff(DestinationManifest, DestinationDiff);

            DiffGenerated = true;
            UpdateStats();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        private void SelectedNodeChanged(string Path)
        {
            SourceManifestDeltaTree.SelectPath(Path);
            DestinationManifestDeltaTree.SelectPath(Path);

            UpdateStats();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        private void NodeExpanded(string Path)
        {
            SourceManifestDeltaTree.ExpandNode(Path);
            DestinationManifestDeltaTree.ExpandNode(Path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        private void NodeCollapsed(string Path)
        {
            SourceManifestDeltaTree.CollapseNode(Path);
            DestinationManifestDeltaTree.CollapseNode(Path);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateStats()
        {
            BuildManifestFileDiff Diff = DestinationManifestDeltaTree.SelectedDiff;
            if (Diff == null)
            {
                Diff = SourceManifestDeltaTree.SelectedDiff;
            }

            if (Diff == null)
            {
                pathLabel.Text = "...";
                blockLabel.Text = "...";
                checksumLabel.Text = "...";
                stateLabel.Text = "...";
            }
            else
            {
                pathLabel.Text = Diff.FileInfo.Path;
                blockLabel.Text = (Diff.FileInfo.LastBlockIndex - Diff.FileInfo.FirstBlockIndex).ToString();
                checksumLabel.Text = Diff.FileInfo.Checksum;
                stateLabel.Text = Diff.Type.ToString();
            }
        }
    }
}
