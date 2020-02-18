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
using WeifenLuo.WinFormsUI.Docking;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ManageUsersForm : DockContent
    {
        /// <summary>
        /// 
        /// </summary>
        private User NewUser;

        /// <summary>
        /// 
        /// </summary>
        private bool IsEditingLabel = false;

        /// <summary>
        /// 
        /// </summary>
        private UserPermission SelectedPermission
        {
            get
            {
                if (PermissionListView.SelectedItems.Count == 0)
                {
                    return null;
                }

                return PermissionListView.SelectedItems[0].Tag as UserPermission;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private User SelectedUser
        {
            get
            {
                if (UserListView.SelectedItems.Count == 0)
                {
                    return null;
                }

                return UserListView.SelectedItems[0].Tag as User;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ManageUsersForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPermissionButtonClicked(object sender, EventArgs e)
        {
            User AddUser = SelectedUser;

            AddPermissionForm Dialog = new AddPermissionForm();
            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                AddUser.Permissions.Permissions.Add(Dialog.Permission);
                SaveUser(SelectedUser);
                RefreshPermissions();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddUserButtonClicked(object sender, EventArgs e)
        {
            NewUser = new User();
            NewUser.Username = "Untitled";

            ListViewItem item = new ListViewItem(NewUser.Username);
            item.Tag = NewUser;
            item.ImageIndex = 0;
            UserListView.Items.Add(item);

            item.BeginEdit();

            foreach (ListViewItem SubItem in UserListView.SelectedItems)
            {
                SubItem.Selected = false;
            }

            item.Selected = true;

            UpdateState();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeginLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (UserListView.Items[e.Item].Tag != NewUser)
            {
                e.CancelEdit = true;
            }
            else
            {
                IsEditingLabel = true;
            }
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
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinishLabelEdit(object sender, LabelEditEventArgs e)
        {
            string newUsername = e.Label == null ? "" : e.Label.Trim();

            if (newUsername == "Untitled" || newUsername.Length <= 0)
            {
                UserListView.Items.RemoveAt(e.Item);
            }
            else
            {
                NewUser.Username = newUsername;
                SaveUser(NewUser);
            }

            NewUser = null;
            IsEditingLabel = false;

            UserListView.Sort();

            UpdateState();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(object sender, FormClosedEventArgs e)
        {
            Program.NetClient.OnUserListRecieved -= UserListRecieved;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            Program.NetClient.OnUserListRecieved += UserListRecieved;
            Program.NetClient.RequestUserList();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PermissionListSelectedItemChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        /// <summary>
        /// </summary>
        private void RefreshPermissions()
        {
            PermissionListView.Items.Clear();

            User user = SelectedUser;
            if (user == null)
            {
                return;
            }

            foreach (UserPermission Permission in user.Permissions.Permissions)
            {
                ListViewItem item = new ListViewItem(new[] {Permission.Type.ToString(), Permission.VirtualPath});
                item.Tag = Permission;
                PermissionListView.Items.Add(item);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshUserList(object sender, EventArgs e)
        {
            if (!Program.NetClient.Permissions.HasPermission(UserPermissionType.ManageUsers, "", false, true))
            {
                Hide();
                return;
            }

            if (Visible)
            {
                Program.NetClient.RequestUserList();
            }
        }

        /// <summary>
        /// </summary>
        private void RefreshUsers(List<User> Users)
        {
            if (IsEditingLabel)
            {
                return;
            }

            foreach (User user in Users)
            {
                if (UserListView.Items[user.Username] != null)
                {
                    continue;
                }

                ListViewItem item = new ListViewItem(user.Username);
                item.Tag = user;
                item.Name = user.Username;
                item.ImageIndex = 0;
                UserListView.Items.Add(item);
            }

            List<ListViewItem> OldItems = new List<ListViewItem>();
            foreach (ListViewItem item in UserListView.Items)
            {
                bool Exists = false;
                foreach (User user in Users)
                {
                    if (user.Username == item.Name)
                    {
                        Exists = true;
                        break;
                    }
                }

                if (!Exists)
                {
                    OldItems.Add(item);
                }
            }

            foreach (ListViewItem item in OldItems)
            {
                UserListView.Items.Remove(item);
            }

            UserListView.Sort();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemovePermissionButtonClicked(object sender, EventArgs e)
        {
            SelectedUser.Permissions.Permissions.Remove(SelectedPermission);
            RefreshPermissions();
            SaveUser(SelectedUser);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveUserButtonClicked(object sender, EventArgs e)
        {
            User User = SelectedUser;

            ListViewItem Selected = UserListView.SelectedItems[0];
            Selected.Selected = false;

            UserListView.Items.Remove(Selected);

            Program.NetClient.DeleteUser(User.Username);
        }

        /// <summary>
        /// </summary>
        private void SaveUser(User ToSave)
        {
            Program.NetClient.SetUserPermissions(ToSave.Username, ToSave.Permissions);
        }

        /// <summary>
        /// </summary>
        private void UpdateState()
        {
            AddUserButton.Enabled = true;
            RemoveUserButton.Enabled = SelectedUser != null;
            AddPermissionButton.Enabled = SelectedUser != null;
            RemovePermissionButton.Enabled = SelectedUser != null && SelectedPermission != null;
        }

        /// <summary>
        /// </summary>
        /// <param name="Users"></param>
        private void UserListRecieved(List<User> Users)
        {
            RefreshUsers(Users);
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserListSelectedItemChanged(object sender, EventArgs e)
        {
            RefreshPermissions();
            UpdateState();
        }
    }
}