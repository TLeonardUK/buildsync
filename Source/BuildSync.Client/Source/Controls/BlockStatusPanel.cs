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
            Idle,
            Downloading,
            Downloaded
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
        public BlockStatusPanel()
        {
            InitializeComponent();

            WindowUtils.EnableDoubleBuffering(this);
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
            int Columns = (int)Math.Floor((float)(Size.Width - CellPadding * 2 - OuterPadding * 2) / (float)CellOveralSize);

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
                CellStates[Cell] = CellState.Idle;

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

            for (int Cell = 0; Cell < CellCount; Cell++)
            {
                int RowIndex = Cell / Columns;
                int ColumnIndex = Cell % Columns;

                int CellX = (ColumnIndex * CellOveralSize) + CellPadding + OuterPadding;
                int CellY = (RowIndex * CellOveralSize) + CellPadding + OuterPadding;

                CellState State = CellStates[Cell];
                Brush FillBrush = Brushes.Black;
                switch (State)
                {
                    case CellState.Idle:        FillBrush = SystemBrushes.ControlLight;  break;
                    case CellState.Downloading: FillBrush = Brushes.Orange;     break;
                    case CellState.Downloaded:  FillBrush = Brushes.LightGreen; break;
                }

                e.Graphics.FillRectangle(FillBrush, CellX, CellY, CellSize, CellSize);
                e.Graphics.DrawRectangle(SystemPens.ActiveBorder, CellX, CellY, CellSize, CellSize);
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
