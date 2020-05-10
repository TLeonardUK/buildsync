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
using System.Drawing;
using System.Windows.Forms;
using BuildSync.Core.Downloads;
using BuildSync.Core.Utils;

namespace BuildSync.Client.Controls
{
    /// <summary>
    /// </summary>
    public partial class BlockStatusPanelLinear : UserControl
    {
        /// <summary>
        /// </summary>
        public enum CellState
        {
            NotDownloaded,
            Downloading,
            Downloaded,
            Uploading,
            Validating,
            Max
        }

        /// <summary>
        /// </summary>
        public DownloadState State;

        /// <summary>
        /// 
        /// </summary>
        public bool Active = true;

        /// <summary>
        /// </summary>
        private readonly Brush[] CellStateBrushes = new Brush[(int) CellState.Max];

        /// <summary>
        /// </summary>
        private readonly string[] CellStateNames = new string[(int) CellState.Max];

        /// <summary>
        /// </summary>
        private CellState[] CellStates = new CellState[0];

        /// <summary>
        /// </summary>
        public BlockStatusPanelLinear()
        {
            InitializeComponent();

            WindowUtils.EnableDoubleBuffering(this);

            CellStateBrushes[(int) CellState.NotDownloaded] = SystemBrushes.ControlLight;
            CellStateBrushes[(int) CellState.Downloading] = Brushes.Orange;
            CellStateBrushes[(int) CellState.Downloaded] = Brushes.LightGreen;
            CellStateBrushes[(int) CellState.Uploading] = Brushes.CornflowerBlue;
            CellStateBrushes[(int) CellState.Validating] = Brushes.Yellow;

            CellStateNames[(int) CellState.NotDownloaded] = "Not Downloaded";
            CellStateNames[(int) CellState.Downloading] = "Downloading";
            CellStateNames[(int) CellState.Downloaded] = "Downloaded";
            CellStateNames[(int) CellState.Uploading] = "Uploading";
            CellStateNames[(int) CellState.Validating] = "Validating";
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPaint(object sender, PaintEventArgs e)
        {
            if (!Active)
            {
                return;
            }

            e.Graphics.FillRectangle(SystemBrushes.Window, 0, 0, Size.Width - 1, Size.Height - 1);
            ControlPaint.DrawBorder(e.Graphics, new Rectangle(0, 0, Size.Width, Size.Height), SystemColors.ControlLight, ButtonBorderStyle.Solid);
            //            e.Graphics.DrawRectangle(SystemPens.ControlLight, 0, 0, Size.Width - 1, Size.Height - 1);

            if (State == null)
            {
                return;
            }

            ManifestDownloadState Downloader = Program.ManifestDownloadManager.GetDownload(State.ActiveManifestId);
            if (Downloader == null)
            {
                return;
            }

            if (Downloader.Manifest == null)
            {
                return;
            }

            int OuterPadding = 8;

            int BarX = OuterPadding;
            int BarY = OuterPadding;
            int BarWidth = Size.Width - (OuterPadding * 2);
            int BarHeight = Size.Height - (OuterPadding * 2);
            
            int MaxCells = BarWidth / 4;

            int CellCount = (int) Downloader.Manifest.BlockCount;
            int CellDivision = 1;
            while (CellCount > MaxCells)
            {
                CellDivision++;
                CellCount = ((int) Downloader.Manifest.BlockCount + (CellDivision - 1)) / CellDivision;
            }

            // Calculate states.
            Array.Resize(ref CellStates, CellCount);
            for (int Cell = 0; Cell < CellCount; Cell++)
            {
                CellStates[Cell] = CellState.NotDownloaded;

                int StartBlock = Cell * CellDivision;
                int EndBlock = StartBlock + (CellDivision - 1);

                // Has already been downloaded?
                for (int Block = StartBlock; Block <= EndBlock; Block++)
                {
                    if (Downloader.BlockStates.Get(Block))
                    {
                        CellStates[Cell] = CellState.Downloaded;
                    }
                }
            }

            // Is in download queue.
            for (int i = 0; i < Program.ManifestDownloadManager.DownloadQueue.Count; i++)
            {
                ManifestPendingDownloadBlock Item = Program.ManifestDownloadManager.DownloadQueue[i];
                if (Item.ManifestId == State.ActiveManifestId)
                {
                    int CellIndex = Item.BlockIndex / CellDivision;
                    if (CellIndex < CellStates.Length)
                    {
                        CellStates[CellIndex] = CellState.Downloading;
                    }
                }
            }

            // Grab most recent downloads.
            lock (Program.ManifestDownloadManager.RecentBlockChanges)
            {
                for (int i = 0; i < Program.ManifestDownloadManager.RecentBlockChanges.Count; i++)
                {
                    ManifestRecentBlockChange Item = Program.ManifestDownloadManager.RecentBlockChanges[i];
                    if (Item.ManifestId == State.ActiveManifestId)
                    {
                        int CellIndex = Item.BlockIndex / CellDivision;
                        if (CellIndex < CellStates.Length)
                        {
                            if (Item.Type == ManifestBlockChangeType.Upload)
                            {
                                CellStates[CellIndex] = CellState.Uploading;
                            }
                            else if (Item.Type == ManifestBlockChangeType.Validate)
                            {
                                CellStates[CellIndex] = CellState.Validating;
                            }
                        }
                    }
                }
            }

            float Spacing = BarWidth / (float)CellCount;

            for (int Cell = 0; Cell < CellCount; Cell++)
            {
                float CellX = BarX + (Cell * Spacing);
                float CellY = BarY;

                CellState State = CellStates[Cell];
                Brush FillBrush = CellStateBrushes[(int) State];

                e.Graphics.FillRectangle(FillBrush, CellX, CellY, Spacing, BarHeight);
            }

            ControlPaint.DrawBorder(e.Graphics, new Rectangle(BarX, BarY, BarWidth, BarHeight), SystemColors.ActiveBorder, ButtonBorderStyle.Solid);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Resized(object sender, EventArgs e)
        {
            Invalidate();
            Refresh();
        }
    }
}