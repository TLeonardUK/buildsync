using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Users
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable, JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserPermission
    {
        ManageBuilds,
        ManageUsers,
        Unknown
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class User
    {
        public string Username { get; set; } = "";
        public List<UserPermission> Permissions { get; set; } = new List<UserPermission>();
    }

    /// <summary>
    /// 
    /// </summary>
    public delegate void PermissionUpdatedEventHandler(User user);

    /// <summary>
    /// 
    /// </summary>
    public delegate void UsersUpdatedEventHandler();

    /// <summary>
    /// 
    /// </summary>
    public class UserManager
    {
        /// <summary>
        /// 
        /// </summary>
        public event PermissionUpdatedEventHandler PermissionsUpdated;

        /// <summary>
        /// 
        /// </summary>
        public event UsersUpdatedEventHandler UsersUpdated;

        /// <summary>
        /// 
        /// </summary>
        public List<User> Users
        {
            get;
            private set;
        } = new List<User>();

        /// <summary>
        /// 
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
        /// 
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
        /// 
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
        /// 
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
        /// 
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public void DeleteUser(User user)
        {
            Users.Remove(user);

            Logger.Log(LogLevel.Info, LogCategory.Users, "Deleted user '{0}'", user.Username);

            UsersUpdated?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public void GrantPermission(User User, UserPermission Permission)
        {
            if (User.Permissions.Contains(Permission))
            {
                return;
            }
            User.Permissions.Add(Permission);

            Logger.Log(LogLevel.Info, LogCategory.Users, "Granted user '{0}' permission '{1}'", User.Username, Permission.ToString());

            UsersUpdated?.Invoke();
            PermissionsUpdated?.Invoke(User);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public void RevokePermission(User User, UserPermission Permission)
        {
            if (!User.Permissions.Contains(Permission))
            {
                return;
            }
            User.Permissions.Remove(Permission);

            Logger.Log(LogLevel.Info, LogCategory.Users, "Revoked user '{0}' permission '{1}'", User.Username, Permission.ToString());

            UsersUpdated?.Invoke();
            PermissionsUpdated?.Invoke(User);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public bool CheckPermission(User User, UserPermission Permission)
        {
            return User.Permissions.Contains(Permission);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public bool CheckPermission(string Username, UserPermission Permission)
        {
            User user = FindUser(Username);
            if (user == null)
            {
                return false;
            }

            return CheckPermission(user, Permission);
        }
    }
}
