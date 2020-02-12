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
using System.Diagnostics;
using System.Windows.Forms;

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
