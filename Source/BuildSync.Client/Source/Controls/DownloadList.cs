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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Downloads;
using WeifenLuo.WinFormsUI.Docking;
using BuildSync.Core.Utils;

namespace BuildSync.Client.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DownloadList : DockContent
    {
        /// <summary>
        /// 
        /// </summary>
        public DownloadListItem SelectedItem
        {
            get
            {
                foreach (DownloadListItem Ctl in Controls)
                {
                    Point TopLeft = Ctl.PointToScreen(Point.Empty);
                    Point BottomRight = Ctl.PointToScreen(new Point(Ctl.ClientSize.Width, Ctl.ClientSize.Height));
                    Point CursorPos = Cursor.Position;

                    if (CursorPos.X < TopLeft.X ||
                        CursorPos.Y < TopLeft.Y ||
                        CursorPos.X > BottomRight.X ||
                        CursorPos.Y > BottomRight.Y)
                    {
                        continue;
                    }

                    return Ctl;
                }

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private DownloadListItem OldSelectedItem = null;

        /// <summary>
        /// 
        /// </summary>
        public DownloadList()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTimerTick(object sender, EventArgs e)
        {
            if (DesignMode)
            {
                return;
            }

            DownloadListItem NewSelectedItem = SelectedItem;
            if (OldSelectedItem != NewSelectedItem)
            {
                if (OldSelectedItem != null)
                {
                    OldSelectedItem.Selected = false;
                }
                if (NewSelectedItem != null)
                {
                    NewSelectedItem.Selected = true;
                }
                OldSelectedItem = NewSelectedItem;
            }

            List<DownloadListItem> ExistingItems = new List<DownloadListItem>();
            foreach (DownloadListItem Ctl in Controls)
            {
                ExistingItems.Add(Ctl);
            }

            // Add new states.
            foreach (DownloadState State in Program.DownloadManager.States.States)
            {
                bool Exists = false;

                foreach (DownloadListItem Ctl in ExistingItems)
                {
                    if (Ctl.State == State)
                    {
                        Exists = true;
                        break;
                    }
                }

                if (!Exists && (!State.IsInternal || Program.Settings.ShowInternalDownloads))
                {
                    DownloadListItem NewItem = new DownloadListItem();
                    NewItem.State = State;
                    NewItem.Dock = DockStyle.Top;
                    ExistingItems.Add(NewItem);
                    Controls.Add(NewItem);
                }
            }

            // Remove old states.
            foreach (DownloadListItem Ctl in ExistingItems)
            {
                bool Exists = false;

                foreach (DownloadState State in Program.DownloadManager.States.States)
                {
                    if (Ctl.State == State)
                    {
                        Exists = true;
                        break;
                    }
                }

                if (!Exists)
                {
                    Controls.Remove(Ctl);
                }
            }

            // Update all items.
            foreach (DownloadListItem Ctl in ExistingItems)
            {
                Ctl.RefreshState();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseClicked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //Program.AppForm.ShowDownloadListContextMenu();
            }
        }
    }
}
