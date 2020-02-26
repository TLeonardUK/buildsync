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
        public List<User> Users { get; } = new List<User>();

        /// <summary>
        /// </summary>
        /// <param name="InitialUsers"></param>
        public UserManager(List<User> InitialUsers = null)
        {
            if (InitialUsers != null)
            {
                Users = InitialUsers;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public bool CheckPermission(User User, UserPermissionType Permission, string Path, bool IgnoreInheritedPermissions = false, bool AllowIfHavePermissionToSubPath = false)
        {
            return User.Permissions.HasPermission(Permission, Path, IgnoreInheritedPermissions, AllowIfHavePermissionToSubPath);
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
        public User CreateUser(string Username)
        {
            User user = FindUser(Username);
            if (user != null)
            {
                return user;
            }

            user = new User();
            user.Username = Username;
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
        /// <param name="Username"></param>
        /// <returns></returns>
        public void GrantPermission(User User, UserPermissionType Permission, string Path)
        {
            User.Permissions.GrantPermission(Permission, Path);

            Logger.Log(LogLevel.Info, LogCategory.Users, "Granted user '{0}' permission '{1}'", User.Username, Permission.ToString());

            UsersUpdated?.Invoke();
            PermissionsUpdated?.Invoke(User);
        }

        /// <summary>
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public void RevokePermission(User User, UserPermissionType Permission, string Path)
        {
            User.Permissions.RevokePermission(Permission, Path);

            Logger.Log(LogLevel.Info, LogCategory.Users, "Revoked user '{0}' permission '{1}'", User.Username, Permission.ToString());

            UsersUpdated?.Invoke();
            PermissionsUpdated?.Invoke(User);
        }

        /// <summary>
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Permissions"></param>
        /// <returns></returns>
        public bool SetPermissions(string Username, UserPermissionCollection Permissions)
        {
            User user = GetOrCreateUser(Username);
            if (user == null)
            {
                return false;
            }

            user.Permissions = Permissions;

            PermissionsUpdated?.Invoke(user);

            return true;
        }
    }
}