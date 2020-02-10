using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Manifests;
using BuildSync.Core.Downloads;
using BuildSync.Core.Utils;

namespace BuildSync.Client.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class BlockStatusPanel : UserControl
    {
        /// <summary>
        /// 
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
        /// 
        /// </summary>
        public DownloadState State;

        /// <summary>
        /// 
        /// </summary>
        private CellState[] CellStates = new CellState[0];

        /// <summary>
        /// 
        /// </summary>
        private Brush[] CellStateBrushes = new Brush[(int)CellState.Max];

        /// <summary>
        /// 
        /// </summary>
        private string[] CellStateNames = new string[(int)CellState.Max];

        /// <summary>
        /// 
        /// </summary>
        private const int LegendWidth = 120;

        /// <summary>
        /// 
        /// </summary>
        public BlockStatusPanel()
        {
            InitializeComponent();

            WindowUtils.EnableDoubleBuffering(this);

            CellStateBrushes[(int)CellState.NotDownloaded] = SystemBrushes.ControlLight;
            CellStateBrushes[(int)CellState.Downloading] = Brushes.Orange;
            CellStateBrushes[(int)CellState.Downloaded] = Brushes.LightGreen;
            CellStateBrushes[(int)CellState.Uploading] = Brushes.CornflowerBlue;
            CellStateBrushes[(int)CellState.Validating] = Brushes.Yellow;

            CellStateNames[(int)CellState.NotDownloaded] = "Not Downloaded";
            CellStateNames[(int)CellState.Downloading] = "Downloading";
            CellStateNames[(int)CellState.Downloaded] = "Downloaded";
            CellStateNames[(int)CellState.Uploading] = "Uploading";
            CellStateNames[(int)CellState.Validating] = "Validating";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPaint(object sender, PaintEventArgs e)
        {
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

            int OuterPadding = 5;

            int CellSize = 7;
            int CellPadding = 3;
            int CellOveralSize = CellSize + CellPadding;

            int Rows = (int)Math.Floor((float)(Size.Height - CellPadding * 2 - OuterPadding * 2) / (float)CellOveralSize);
            int Columns = (int)Math.Floor((float)(Size.Width - CellPadding * 2 - OuterPadding * 2 - LegendWidth) / (float)CellOveralSize);

            int MaxCells = Rows * Columns;

            int CellCount = (int)Downloader.Manifest.BlockCount;
            int CellDivision = 1;
            while (CellCount > MaxCells)
            {
                CellDivision++;
                CellCount = ((int)Downloader.Manifest.BlockCount + (CellDivision - 1)) / CellDivision;
            }

            // Calculate states.
            Array.Resize(ref CellStates, CellCount);
            for (int Cell = 0; Cell < CellCount; Cell++)
            {
                CellStates[Cell] = CellState.NotDownloaded;

                int StartBlock = (Cell * CellDivision);
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

            for (int Cell = 0; Cell < CellCount; Cell++)
            {
                int RowIndex = Cell / Columns;
                int ColumnIndex = Cell % Columns;

                int CellX = (ColumnIndex * CellOveralSize) + CellPadding + OuterPadding;
                int CellY = (RowIndex * CellOveralSize) + CellPadding + OuterPadding;

                CellState State = CellStates[Cell];
                Brush FillBrush = CellStateBrushes[(int)State];

                e.Graphics.FillRectangle(FillBrush, CellX, CellY, CellSize, CellSize);
                e.Graphics.DrawRectangle(SystemPens.ActiveBorder, CellX, CellY, CellSize, CellSize);
            }

            // Draw the legend.
            int LegendX = Size.Width - LegendWidth - OuterPadding;
            int LegendY = OuterPadding + 5;

            //e.Graphics.DrawString("Legend", SystemFonts.DefaultFont, Brushes.Black, LegendX, LegendY);
            //LegendY += 20;

            for (int i = 0; i < (int)CellState.Max; i++)
            {
                Brush FillBrush = CellStateBrushes[i];

                e.Graphics.FillRectangle(FillBrush, LegendX, LegendY, CellSize, CellSize);
                e.Graphics.DrawRectangle(SystemPens.ActiveBorder, LegendX, LegendY, CellSize, CellSize);

                e.Graphics.DrawString(CellStateNames[i], SystemFonts.DefaultFont, Brushes.Black, LegendX + CellSize + CellPadding, LegendY - 3);
                LegendY += 16;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Resized(object sender, EventArgs e)
        {
            this.Invalidate();
            this.Refresh();
        }
    }
}
