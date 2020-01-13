using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Licensing;

namespace BuildSync.Client.Forms
{
    public partial class LicenseForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public LicenseForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoad(object sender, EventArgs e)
        {
            Program.NetClient.OnLicenseInfoRecieved += LicenseInfoRecieved;
            Program.NetClient.RequestLicenseInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LicenseInfo"></param>
        private void LicenseInfoRecieved(Core.Licensing.License LicenseInfo)
        {
            licensedToLabel.Text = LicenseInfo.LicensedTo;
            seatsLabel.Text = LicenseInfo.MaxSeats == int.MaxValue ? "Unlimited" : LicenseInfo.MaxSeats.ToString();
            expirationLabel.Text = LicenseInfo.ExpirationTime == Core.Licensing.License.InfiniteExpirationTime ? "Never" : LicenseInfo.ExpirationTime.ToString("r");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(object sender, FormClosedEventArgs e)
        {
            Program.NetClient.OnLicenseInfoRecieved -= LicenseInfoRecieved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseClicked(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyClicked(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "License files (*.dat)|*.dat|All files (*.*)|*.*";
            dialog.Title = "Select license file ...";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Core.Licensing.License license = Core.Licensing.License.Load(dialog.FileName);
                if (license == null)
                {
                    MessageBox.Show("File does not appear to be a valid license file", "License file invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    Program.NetClient.RequestApplyLicense(license);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuyClicked(object sender, EventArgs e)
        {
            Process.Start("http://www.buildsync.com");
        }
    }
}
