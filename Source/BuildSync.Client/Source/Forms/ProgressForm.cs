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
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Utils;
using BuildSync.Client.Tasks;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ProgressForm : Form
    {
        private bool Finished = false;
        private Task Work = null;

        /// <summary>
        /// 
        /// </summary>
        public ProgressForm(Task task)
        {
            Work = task;
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormClosingRequested(object sender, FormClosingEventArgs e)
        {
            if (!Finished)
            {
                MessageBox.Show("This process cannot be interrupted.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Progress"></param>
        /// <param name="TotalMessage"></param>
        /// <param name="TotalProgress"></param>
        public void SetProgress(string Message, float Progress)
        {
            Invoke((MethodInvoker)(() =>
            {
                TaskProgressLabel.Text = Message;
                TaskProgressBar.Value = (int)(Progress * 100);
            }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormLoaded(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTimerTick(object sender, EventArgs e)
        {
            SetProgress("Installing ...", 0);

            if (Work.IsCompleted)
            {
                UpdateTimer.Enabled = false;
                Finished = true;
                Close();
            }
        }
    }
}
