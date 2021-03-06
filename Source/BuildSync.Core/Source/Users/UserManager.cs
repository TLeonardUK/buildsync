﻿/*
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
using System.Text.Json.Serialization;
using BuildSync.Core.Networking;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Users
{
    /// <summary>
    /// </summary>
    public delegate void PermissionUpdatedEventHandler(User user);

    /// <summary>
    /// </summary>
    public delegate void UsersUpdatedEventHandler();

    /// <summary>
    /// </summary>
    public delegate void UserGroupsUpdatedEventHandler();

    /// <summary>
    /// </summary>
    public class UserManager
    {
        /// <summary>
        /// </summary>
        public event PermissionUpdatedEventHandler PermissionsUpdated;

        /// <summary>
        /// </summary>
        public event UsersUpdatedEventHandler UsersUpdated;

        /// <summary>
        /// </summary>
        public event UserGroupsUpdatedEventHandler UserGroupsUpdated;

        /// <summary>
        /// </summary>
        public List<User> Users { get; } = new List<User>();

        /// <summary>
        /// </summary>
        public List<UserGroup> UserGroups { get; } = new List<UserGroup>();

        /// <summary>
        /// </summary>
        /// <param name="InitialUsers"></param>
        public UserManager(List<User> InitialUsers = null, List<UserGroup> InitialGroups = null)
        {
            if (InitialUsers != null)
            {
                Users = InitialUsers;
            }
            if (InitialGroups != null)
            {
                UserGroups = InitialGroups;
            }

            if (UserGroups.Count == 0)
            {
                UserGroup AllUsersGroup = new UserGroup();
                AllUsersGroup.Name = "All Users";
                UserGroups.Add(AllUsersGroup);

                UserGroup AdminsGroup = new UserGroup();
                AdminsGroup.Name = "Administrators";
                AdminsGroup.Permissions.GrantPermission(UserPermissionType.Write, "");
                AdminsGroup.Permissions.GrantPermission(UserPermissionType.ModifyUsers, "");
                AdminsGroup.Permissions.GrantPermission(UserPermissionType.Read, "");
                AdminsGroup.Permissions.GrantPermission(UserPermissionType.PushUpdate, "");
                AdminsGroup.Permissions.GrantPermission(UserPermissionType.ModifyServer, "");
                AdminsGroup.Permissions.GrantPermission(UserPermissionType.TagBuilds, "");
                AdminsGroup.Permissions.GrantPermission(UserPermissionType.ModifyTags, "");
                AdminsGroup.Permissions.GrantPermission(UserPermissionType.ModifyRoutes, "");
                UserGroups.Add(AdminsGroup);
            }

            foreach (User user in Users)
            {
                if (!user.Groups.Contains("All Users"))
                {
                    user.Groups.Add("All Users");
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public bool CheckPermission(User User, UserPermissionType Permission, string Path, bool IgnoreInheritedPermissions = false, bool AllowIfHavePermissionToSubPath = false)
        {
            foreach (string GroupName in User.Groups)
            {
                UserGroup group = FindGroup(GroupName);   
                if (group.Permissions.HasPermission(Permission, Path, IgnoreInheritedPermissions, AllowIfHavePermissionToSubPath))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public bool CheckPermission(string Username, UserPermissionType Permission, string Path, bool IgnoreInheritedPermissions = false, bool AllowIfHavePermissionToSubPath = false)
        {
            User user = FindUser(Username);
            if (user == null)
            {
                return false;
            }

            return CheckPermission(user, Permission, Path, IgnoreInheritedPermissions, AllowIfHavePermissionToSubPath);
        }

        /// <summary>
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public bool HasAnyPermission(User User, UserPermissionType Permission)
        {
            foreach (string GroupName in User.Groups)
            {
                UserGroup group = FindGroup(GroupName);
                if (group.Permissions.HasAnyPermissionOfType(Permission))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public bool HasAnyPermission(string Username, UserPermissionType Permission)
        {
            User user = FindUser(Username);
            if (user == null)
            {
                return false;
            }

            return HasAnyPermission(user, Permission);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public UserPermissionCollection GetUserCombinedPermissions(string Username)
        {
            User user = GetOrCreateUser(Username);

            UserPermissionCollection Collection = new UserPermissionCollection();

            foreach (string GroupName in user.Groups)
            {
                UserGroup group = FindGroup(GroupName);
                if (group != null)
                {
                    Collection.Merge(group.Permissions);
                }
            }

            return Collection;
        }

        /// <summary>
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public User CreateUser(string Username)
        {
            User user = FindUser(Username);
            if (user != null)
            {
                return user;
            }

            user = new User();
            user.Username = Username;
            user.Groups.Add("All Users");

            // First user is always an administrator.
            if (Users.Count == 0)
            {
                user.Groups.Add("Administrators");
            }

            Users.Add(user);

            Logger.Log(LogLevel.Info, LogCategory.Users, "Added new user '{0}'", Username);

            UsersUpdated?.Invoke();

            return user;
        }

        /// <summary>
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public void DeleteUser(User user)
        {
            Users.Remove(user);

            Logger.Log(LogLevel.Info, LogCategory.Users, "Deleted user '{0}'", user.Username);

            PermissionsUpdated?.Invoke(user);
            UsersUpdated?.Invoke();
        }

        /// <summary>
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public void DeleteUser(string Username)
        {
            User user = FindUser(Username);
            if (user == null)
            {
                return;
            }

            DeleteUser(user);
        }

        /// <summary>
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public User FindUser(string Username)
        {
            foreach (User user in Users)
            {
                if (user.Username.ToLower() == Username.ToLower())
                {
                    return user;
                }
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public User GetOrCreateUser(string Username)
        {
            User user = FindUser(Username);
            if (user == null)
            {
                user = CreateUser(Username);
            }

            return user;
        }

        /// <summary>
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public UserGroup CreateGroup(string Name)
        {
            UserGroup group = FindGroup(Name);
            if (group != null)
            {
                return group;
            }

            group = new UserGroup();
            group.Name = Name;
            UserGroups.Add(group);

            Logger.Log(LogLevel.Info, LogCategory.Users, "Added new user group '{0}'", Name);

            UserGroupsUpdated?.Invoke();

            return group;
        }

        /// <summary>
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public void DeleteGroup(UserGroup group)
        {
            UserGroups.Remove(group);

            Logger.Log(LogLevel.Info, LogCategory.Users, "Deleted user group '{0}'", group.Name);

            foreach (User user in Users)
            {
                if (user.Groups.Contains(group.Name))
                {
                    user.Groups.Remove(group.Name);
                    PermissionsUpdated?.Invoke(user);
                }
            }

            UserGroupsUpdated?.Invoke();
        }

        /// <summary>
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public void DeleteGroup(string Name)
        {
            UserGroup group = FindGroup(Name);
            if (group == null)
            {
                return;
            }

            DeleteGroup(group);
        }

        /// <summary>
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public UserGroup FindGroup(string Name)
        {
            foreach (UserGroup group in UserGroups)
            {
                if (group.Name.ToLower() == Name.ToLower())
                {
                    return group;
                }
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public UserGroup GetOrCreateGroup(string Name)
        {
            UserGroup group = FindGroup(Name);
            if (group == null)
            {
                group = CreateGroup(Name);
            }

            return group;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="group"></param>
        public void AddUserToGroup(string userName, string groupName)
        {
            User user = GetOrCreateUser(userName);
            if (user == null)
            {
                return;
            }

            UserGroup group = FindGroup(groupName);
            if (group == null)
            {
                return;
            }

            AddUserToGroup(user, group);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="group"></param>
        public void AddUserToGroup(User user, UserGroup group)
        {
            if (!user.Groups.Contains(group.Name))
            {
                user.Groups.Add(group.Name);
                Logger.Log(LogLevel.Info, LogCategory.Users, "Adding user '{0}' to group '{1}'", user.Username, group.Name);

                PermissionsUpdated?.Invoke(user);
                UserGroupsUpdated?.Invoke();
                UsersUpdated?.Invoke();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="group"></param>
        public void RemoveUserFromGroup(string userName, string groupName)
        {
            User user = GetOrCreateUser(userName);
            if (user == null)
            {
                return;
            }

            UserGroup group = FindGroup(groupName);
            if (group == null)
            {
                return;
            }

            RemoveUserFromGroup(user, group);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="group"></param>
        public void RemoveUserFromGroup(User user, UserGroup group)
        {
            if (user.Groups.Contains(group.Name))
            {
                user.Groups.Remove(group.Name);
                Logger.Log(LogLevel.Info, LogCategory.Users, "Removing user '{0}' from group '{1}'", user.Username, group.Name);

                PermissionsUpdated?.Invoke(user);
                UserGroupsUpdated?.Invoke();
                UsersUpdated?.Invoke();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public void GrantPermission(string GroupName, UserPermissionType Permission, string Path)
        {
            UserGroup group = FindGroup(GroupName);
            if (group == null)
            {
                return;
            }

            GrantPermission(group, Permission, Path);
        }

        /// <summary>
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public void GrantPermission(UserGroup Group, UserPermissionType Permission, string Path)
        {
            Group.Permissions.GrantPermission(Permission, Path);

            Logger.Log(LogLevel.Info, LogCategory.Users, "Granted group '{0}' permission '{1}'", Group.Name, Permission.ToString());

            UsersUpdated?.Invoke();
            UserGroupsUpdated?.Invoke();

            foreach (User user in Users)
            {
                if (user.Groups.Contains(Group.Name))
                {
                    PermissionsUpdated?.Invoke(user);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public void RevokePermission(string GroupName, UserPermissionType Permission, string Path)
        {
            UserGroup group = FindGroup(GroupName);
            if (group == null)
            {
                return;
            }

            RevokePermission(group, Permission, Path);
        }

        /// <summary>
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public void RevokePermission(UserGroup Group, UserPermissionType Permission, string Path)
        {
            Group.Permissions.RevokePermission(Permission, Path);

            Logger.Log(LogLevel.Info, LogCategory.Users, "Revoked group '{0}' permission '{1}'", Group.Name, Permission.ToString());

            UsersUpdated?.Invoke();
            UserGroupsUpdated?.Invoke();

            foreach (User user in Users)
            {
                if (user.Groups.Contains(Group.Name))
                {
                    PermissionsUpdated?.Invoke(user);
                }
            }
        }
    }
}