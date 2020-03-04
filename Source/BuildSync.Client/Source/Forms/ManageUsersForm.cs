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
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using BuildSync.Core.Users;
using BuildSync.Core.Utils;
using WeifenLuo.WinFormsUI.Docking;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using BuildSync.Client.Properties;

namespace BuildSync.Client.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ManageUsersForm : DockContent
    {
        public class UserTreeNode : Node
        {
            public Image Icon;
            public string Name;
            public string PermissionPath;
            public UserPermissionType PermissionType;

            public bool IsGroup;
            public bool IsFolder;
            public bool IsUser;
            public bool IsPermission;
            public bool IsPermissionFolder;
            public bool IsUserFolder;
            
            public UserTreeNode GroupPermissionsNode;
            public UserTreeNode GroupUsersNode;

            public UserGroup Group;
        }

        /// <summary>
        /// 
        /// </summary>
        private TreeModel Model = null;

        /// <summary>
        /// 
        /// </summary>
        private List<User> AllUsers = null;

        /// <summary>
        /// 
        /// </summary>
        public ManageUsersForm()
        {
            InitializeComponent();
            
            Model = new TreeModel();
            MainTreeView.Model = Model;

            TreeColumn NameColumn = new TreeColumn();
            NameColumn.Header = "Name";
            NameColumn.Width = 400;
            MainTreeView.Columns.Add(NameColumn);
            
                ScaledNodeIcon IconControl = new ScaledNodeIcon();
                IconControl.ParentColumn = NameColumn;
                IconControl.DataPropertyName = "Icon";
                IconControl.FixedSize = new Size((int)(MainTreeView.RowHeight * 1.5f), (int)(MainTreeView.RowHeight * 1.5f));
                IconControl.Offset = new Size(0, 5);
                MainTreeView.NodeControls.Add(IconControl);

                NodeTextBox TextControl = new NodeTextBox();
                TextControl.ParentColumn = NameColumn;
                TextControl.DataPropertyName = "Name";
                MainTreeView.NodeControls.Add(TextControl);

            TreeColumn PathColumn = new TreeColumn();
            PathColumn.Header = "Path";
            PathColumn.Width = 200;
            MainTreeView.Columns.Add(PathColumn);

                NodeTextBox CreatedControl = new NodeTextBox();
                CreatedControl.ParentColumn = PathColumn;
                CreatedControl.DataPropertyName = "PermissionPath";
                MainTreeView.NodeControls.Add(CreatedControl);
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

            SelectionChanged(null, null);
        }
        
        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshUserList(object sender, EventArgs e)
        {
            if (!Program.NetClient.Permissions.HasPermission(UserPermissionType.ModifyUsers, "", false, true) &&
                !Program.NetClient.Permissions.HasAnyPermissionOfType(UserPermissionType.AddUsersToGroup))
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
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        private UserTreeNode GetUsergroupNode(string Name)
        {
            foreach (UserTreeNode Node in Model.Root.Nodes)
            {
                if (Node.IsGroup && Node.Name == Name)
                {
                    return Node;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        private UserTreeNode GetUserNode(string Name, UserTreeNode ParentNode)
        {
            foreach (UserTreeNode Node in ParentNode.Nodes)
            {
                if (Node.IsUser && Node.Name == Name)
                {
                    return Node;
                }
            }
            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        private UserTreeNode GetPermissionNode(UserPermission Permission, UserTreeNode ParentNode)
        {
            foreach (UserTreeNode Node in ParentNode.Nodes)
            {
                if (Node.IsPermission && Node.PermissionType == Permission.Type && Node.PermissionPath == Permission.VirtualPath)
                {
                    return Node;
                }
            }
            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Group"></param>
        /// <param name="Users"></param>
        private void RefreshGroup(UserGroup Group, UserTreeNode GroupNode, List<User> Users)
        {
            // Add all new users.
            foreach (User User in Users)
            {
                if (!User.Groups.Contains(Group.Name))
                {
                    continue;
                }

                if (GetUserNode(User.Username, GroupNode.GroupUsersNode) != null)
                {
                    continue;
                }

                UserTreeNode UserNode = new UserTreeNode();
                UserNode.Name = User.Username;
                UserNode.Icon = Resources.appbar_people;
                UserNode.IsUser = true;
                UserNode.Group = Group;
                GroupNode.GroupUsersNode.Nodes.Add(UserNode);
            }

            // Remove old users.
            for (int i = 0; i < GroupNode.GroupUsersNode.Nodes.Count; i++)
            {
                UserTreeNode Node = GroupNode.GroupUsersNode.Nodes[i] as UserTreeNode;
                if (Node.IsUser)
                {
                    bool Found = false;
                    foreach (User user in Users)
                    {
                        if (!user.Groups.Contains(Group.Name))
                        {
                            continue;
                        }

                        if (user.Username == Node.Name)
                        {
                            Found = true;
                            break;
                        }
                    }

                    if (!Found)
                    {
                        GroupNode.GroupUsersNode.Nodes.RemoveAt(i);
                        i--;
                    }
                }
            }

            // All all new permissions.
            foreach (UserPermission Permission in Group.Permissions.Permissions)
            {
                if (GetPermissionNode(Permission, GroupNode.GroupPermissionsNode) != null)
                {
                    continue;
                }

                UserTreeNode PermissionNode = new UserTreeNode();
                PermissionNode.Name = Permission.Type.GetAttributeOfType<DescriptionAttribute>().Description;
                PermissionNode.PermissionType = Permission.Type;
                PermissionNode.PermissionPath = Permission.VirtualPath;
                PermissionNode.Icon = Resources.appbar_star;
                PermissionNode.IsPermission = true;
                PermissionNode.Group = Group;
                GroupNode.GroupPermissionsNode.Nodes.Add(PermissionNode);
            }

            // Remove old permissions.
            for (int i = 0; i < GroupNode.GroupPermissionsNode.Nodes.Count; i++)
            {
                UserTreeNode Node = GroupNode.GroupPermissionsNode.Nodes[i] as UserTreeNode;
                if (Node.IsPermission)
                {
                    bool Found = false;
                    foreach (UserPermission Permission in Group.Permissions.Permissions)
                    {
                        if (Permission.Type == Node.PermissionType &&
                            Permission.VirtualPath == Node.PermissionPath)
                        {
                            Found = true;
                            break;
                        }
                    }

                    if (!Found)
                    {
                        GroupNode.GroupPermissionsNode.Nodes.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        private void RefreshUsers(List<User> Users, List<UserGroup> UserGroups)
        {
            // Add all users group.
            foreach (UserGroup Group in UserGroups)
            {
                if (GetUsergroupNode(Group.Name) != null)
                {
                    continue;
                }

                UserTreeNode GroupNode = new UserTreeNode();
                GroupNode.Name = Group.Name;
                GroupNode.Icon = Resources.appbar_group;
                GroupNode.IsGroup = true;
                GroupNode.Group = Group;
                Model.Root.Nodes.Add(GroupNode);

                UserTreeNode GroupPermissionsNode = new UserTreeNode();
                GroupPermissionsNode.Name = "Permissions";
                GroupPermissionsNode.Icon = Resources.appbar_folder_star;
                GroupPermissionsNode.IsFolder = true;
                GroupPermissionsNode.IsPermissionFolder = true;
                GroupPermissionsNode.Group = Group;
                GroupNode.Nodes.Add(GroupPermissionsNode);

                UserTreeNode GroupUsersNode = new UserTreeNode();
                GroupUsersNode.Name = "Users";
                GroupUsersNode.Icon = Resources.appbar_folder_people;
                GroupUsersNode.IsFolder = true;
                GroupUsersNode.IsUserFolder = true;
                GroupUsersNode.Group = Group;
                GroupNode.Nodes.Add(GroupUsersNode);

                GroupNode.GroupPermissionsNode = GroupPermissionsNode;
                GroupNode.GroupUsersNode = GroupUsersNode;
            }

            // Remove old user groups.
            for (int i = 0; i < Model.Root.Nodes.Count; i++)
            {
                UserTreeNode Node = Model.Root.Nodes[i] as UserTreeNode;
                if (Node.IsGroup)
                {
                    bool Found = false;
                    foreach (UserGroup Group in UserGroups)
                    {
                        if (Group.Name == Node.Name)
                        {
                            RefreshGroup(Group, Node, Users);

                            Found = true;
                            break;
                        }
                    }

                    if (!Found)
                    {
                        Model.Root.Nodes.RemoveAt(i);
                        i--;    
                    }
                }
            }
        }
        
        /// <summary>
        /// </summary>
        /// <param name="Users"></param>
        private void UserListRecieved(List<User> Users, List<UserGroup> UserGroups)
        {
            AllUsers = Users;
            RefreshUsers(Users, UserGroups);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteClicked(object sender, EventArgs e)
        {
            UserTreeNode Node = MainTreeView.SelectedNode == null ? null : MainTreeView.SelectedNode.Tag as UserTreeNode;
            if (Node == null)
            {
                return;
            }

            // Remove usergroup.
            if (Node.IsGroup)
            {
                Program.NetClient.DeleteUserGroup(Node.Group.Name);
            }

            // Remove permission.
            else if (Node.IsPermission)
            {
                Program.NetClient.RemoveUserGroupPermission(Node.Group.Name, Node.PermissionType, Node.PermissionPath);
            }

            // Remove user.
            else if (Node.IsUser)
            {
                Program.NetClient.RemoveUserFromUserGroup(Node.Group.Name, Node.Name);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddClicked(object sender, EventArgs e)
        {
            UserTreeNode Node = MainTreeView.SelectedNode == null ? null : MainTreeView.SelectedNode.Tag as UserTreeNode;
          
            // Add usergroup.
            if (Node == null || Node.IsGroup)
            {
                AddUserGroupForm form = new AddUserGroupForm();
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    Program.NetClient.CreateUserGroup(form.GroupName);
                }
            }

            // Add permission.
            else if (Node.IsPermission || Node.IsPermissionFolder)
            {
                AddPermissionForm form = new AddPermissionForm();
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    Program.NetClient.AddUserGroupPermission(Node.Group.Name, form.Permission.Type, form.Permission.VirtualPath);
                }
            }

            // Add user.
            else if (Node.IsUser || Node.IsUserFolder)
            {
                AddUserForm form = new AddUserForm(AllUsers);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    Program.NetClient.AddUserToUserGroup(Node.Group.Name, form.Username);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionChanged(object sender, EventArgs e)
        {
            UserTreeNode Node = MainTreeView.SelectedNode == null ? null : MainTreeView.SelectedNode.Tag as UserTreeNode;

            bool hasFullPermission = Program.NetClient.Permissions.HasPermission(UserPermissionType.ModifyUsers, "", false, true);

            deleteMenuItem.Enabled = false;
            addMenuItem.Text = "Add User Group ...";
            deleteMenuItem.Text = "Delete User Group";

            addMenuItem.Enabled = hasFullPermission;

            if (Node != null)
            {
                if (Node.IsPermission || Node.IsPermissionFolder)
                {
                    deleteMenuItem.Text = "Delete Permission";
                    deleteMenuItem.Enabled = Node.IsPermission && hasFullPermission;

                    addMenuItem.Text = "Add Permission ...";
                    addMenuItem.Enabled = hasFullPermission;
                }
                else if (Node.IsUser || Node.IsUserFolder)
                {
                    deleteMenuItem.Text = "Remove User From Group ...";
                    deleteMenuItem.Enabled = (Node.IsUser && Node.Group.Name != "All Users") && (Program.NetClient.Permissions.HasPermission(UserPermissionType.AddUsersToGroup, Node.Group.Name) || hasFullPermission);

                    addMenuItem.Text = "Add User To Group ...";
                    addMenuItem.Enabled = Program.NetClient.Permissions.HasPermission(UserPermissionType.AddUsersToGroup, Node.Group.Name) || hasFullPermission;
                }
                else if (Node.IsGroup)
                {
                    deleteMenuItem.Enabled = (Node.Name != "All Users") && hasFullPermission;
                    addMenuItem.Enabled = hasFullPermission;
                }
            }
        }
    }
}