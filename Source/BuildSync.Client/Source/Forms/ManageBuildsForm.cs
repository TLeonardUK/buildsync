using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ManageBuildsForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public ManageBuildsForm()
        {
            InitializeComponent();

            downloadFileSystemTree.CanSelectBuildContainers = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ValidateState()
        {
            RemoveBuildButton.Enabled = (
                downloadFileSystemTree.SelectedManifestId != Guid.Empty
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBuildClicked(object sender, EventArgs e)
        {
            (new PublishBuildForm()).ShowDialog(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveBuildClicked(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete '" + downloadFileSystemTree.SelectedPath + "'.", "Delete Build", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (!Program.NetClient.DeleteManifest(downloadFileSystemTree.SelectedManifestId))
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DateStateChanged(object sender, EventArgs e)
        {
            ValidateState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButtonClicked(object sender, EventArgs e)
        {
            Close();
        }
    }
}
