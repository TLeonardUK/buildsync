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
using System.Windows.Forms;
using BuildSync.Core.Users;
using BuildSync.Core.Utils;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// </summary>
    public partial class AddUserForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public string Username = "";

        /// <summary>
        /// 
        /// </summary>
        public List<string> PotentialUsers = new List<string>();

        /// <summary>
        /// </summary>
        public AddUserForm(List<User> AllUsers)
        {
            foreach (User user in AllUsers)
            {
                PotentialUsers.Add(user.Username);
            }

            List<string> DomainUsers = DomainUtils.GetDomainUsers();
            foreach (string user in DomainUsers)
            {
                if (!PotentialUsers.Contains(user))
                {
                    PotentialUsers.Add(user);
                }
            }

            InitializeComponent();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkClicked(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            nameTextBox.Text = Username;

            UpdateState();
        }

        /// <summary>
        /// </summary>
        private void UpdateState()
        {
            addGroupButton.Enabled = (nameTextBox.Text.Trim().Length > 0);
            Username = nameTextBox.Text;

            potentialListView.Items.Clear();
            foreach (string potential in PotentialUsers)
            {
                if (potential.ToLower().Contains(Username.ToLower()) || Username.Length == 0)
                {
                    potentialListView.Items.Add(potential, 0);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameTextChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickUser(object sender, EventArgs e)
        {
            /*if (potentialListView.SelectedItems.Count > 0)
            {
                nameTextBox.Text = potentialListView.SelectedItems[0].Text;
            }*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDoubleClickUser(object sender, EventArgs e)
        {
            if (potentialListView.SelectedItems.Count > 0)
            {
                nameTextBox.Text = potentialListView.SelectedItems[0].Text;

                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}