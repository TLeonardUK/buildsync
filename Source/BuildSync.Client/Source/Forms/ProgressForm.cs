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
