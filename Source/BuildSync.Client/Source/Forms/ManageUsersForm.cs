using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSync.Core.Users;
using WeifenLuo.WinFormsUI.Docking;

namespace BuildSync.Client.Forms
{
    public partial class ManageUsersForm : DockContent
    {
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

        private User NewUser = null;

        public ManageUsersForm()
        {
            InitializeComponent();
        }

        private void CloseClicked(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShown(object sender, EventArgs e)
        {
            Program.NetClient.OnUserListRecieved += UserListRecieved;
            Program.NetClient.RequestUserList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosed(object sender, FormClosedEventArgs e)
        {
            Program.NetClient.OnUserListRecieved -= UserListRecieved;
        }

        /// <summary>
        /// 
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

            UserListView.Sort();

            UpdateState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeginLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (UserListView.Items[e.Item].Tag != NewUser)
            {
                e.CancelEdit = true;
            }
        }

        /// <summary>
        /// 
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
                ListViewItem item = new ListViewItem(new string[] { Permission.Type.ToString(), Permission.VirtualPath });
                item.Tag = Permission;
                PermissionListView.Items.Add(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshUsers(List<User> Users)
        {
            UserListView.Items.Clear();

            foreach (User user in Users)
            {
                ListViewItem item = new ListViewItem(user.Username);
                item.Tag = user;
                item.ImageIndex = 0;
                UserListView.Items.Add(item);
            }

            UserListView.Sort();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Users"></param>
        private void UserListRecieved(List<User> Users)
        {
            RefreshUsers(Users);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserListSelectedItemChanged(object sender, EventArgs e)
        {
            RefreshPermissions();
            UpdateState();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PermissionListSelectedItemChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateState()
        {
            AddUserButton.Enabled = true;
            RemoveUserButton.Enabled = (SelectedUser != null);
            AddPermissionButton.Enabled = (SelectedUser != null);
            RemovePermissionButton.Enabled = (SelectedUser != null && SelectedPermission != null);
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveUser(User ToSave)
        {
            Program.NetClient.SetUserPermissions(ToSave.Username, ToSave.Permissions);
        }
    }
}
