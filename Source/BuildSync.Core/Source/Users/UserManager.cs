using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BuildSync.Core.Utils;
using BuildSync.Core.Networking;

namespace BuildSync.Core.Users
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable, JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserPermissionType
    {
        ManageBuilds,
        ManageUsers,
        Access,
        ForceUpdate,
        Unknown
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UserPermission
    {
        public UserPermissionType Type { get; set; } = UserPermissionType.Unknown;

        [JsonIgnore]
        public string[] VirtualPathSplit { get; private set; } = new string[0]; 

        private string InternalVirtualPath = "";
        public string VirtualPath
        {
            get
            {
                return InternalVirtualPath;
            }
            set
            {
                InternalVirtualPath = value;
                VirtualPathSplit = value.ToLower().Split('/');
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class UserPermissionCollection
    {
        public List<UserPermission> Permissions { get; set; } = new List<UserPermission>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Path"></param>
        /// <returns></returns>
        public bool HasPermission(UserPermissionType Type, string Path, bool IgnoreInheritedPermissions = false, bool AllowIfHavePermissionToSubPath = false)
        {
            Path = VirtualFileSystem.Normalize(Path).ToLower();
            string[] SplitPath = Path.Split('/');
            foreach (UserPermission Permission in Permissions)
            {
                if (Permission.Type == Type)
                {
                    if (IgnoreInheritedPermissions)
                    {
                        // Exact path match.
                        if (Permission.VirtualPath.ToLower() == Path)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        // We can early out if we have root permission.
                        if (Permission.VirtualPath.Length == 0)
                        {
                            return true;
                        }

                        // If the permission path is a parent of this path, we are good to go.
                        if (Permission.VirtualPathSplit.Length <= SplitPath.Length)
                        {
                            bool IsValid = true;
                            for (int i = 0; i < Permission.VirtualPathSplit.Length && IsValid; i++)
                            {
                                if (Permission.VirtualPathSplit[i] != SplitPath[i])
                                {
                                    IsValid = false;
                                }
                            }

                            if (IsValid)
                            {
                                return true;
                            }
                        }

                        // If permission path is a sub-path of this path, we are good to go.
                        if (AllowIfHavePermissionToSubPath)
                        {
                            if (Permission.VirtualPathSplit.Length >= SplitPath.Length)
                            {
                                bool IsValid = true;
                                for (int i = 0; i < SplitPath.Length && IsValid; i++)
                                {
                                    if (Permission.VirtualPathSplit[i] != SplitPath[i])
                                    {
                                        IsValid = false;
                                    }
                                }

                                if (IsValid)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Permission"></param>
        /// <param name="Path"></param>
        public void GrantPermission(UserPermissionType Type, string Path)
        {
            Path = VirtualFileSystem.Normalize(Path);

            foreach (UserPermission Permission in Permissions)
            {
                if (Permission.Type == Type && Permission.VirtualPath.ToLower() == Path.ToLower())
                {
                    return;
                }
            }

            UserPermission NewPermission = new UserPermission();
            NewPermission.Type = Type;
            NewPermission.VirtualPath = Path;
            Permissions.Add(NewPermission);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public void RevokePermission(UserPermissionType Type, string Path)
        {
            Path = VirtualFileSystem.Normalize(Path);

            foreach (UserPermission Permission in Permissions)
            {
                if (Permission.Type == Type && Permission.VirtualPath.ToLower() == Path.ToLower())
                {
                    Permissions.Remove(Permission);
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        public void Serialize(NetMessageSerializer serializer)
        {
            int Count = Permissions.Count;
            serializer.Serialize(ref Count);

            for (int i = 0; i < Count; i++)
            {
                if (serializer.IsLoading)
                {
                    Permissions.Add(new UserPermission());
                }

                UserPermissionType Type = Permissions[i].Type;
                string Path = Permissions[i].VirtualPath;

                serializer.SerializeEnum<UserPermissionType>(ref Type);
                serializer.Serialize(ref Path);

                Permissions[i].Type = Type;
                Permissions[i].VirtualPath = Path;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class User
    {
        public string Username { get; set; } = "";
        public UserPermissionCollection Permissions { get; set; } = new UserPermissionCollection();
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

            PermissionsUpdated?.Invoke(user);
            UsersUpdated?.Invoke();
        }

        /// <summary>
        /// 
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
        /// 
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
        /// 
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
        /// 
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public bool CheckPermission(User User, UserPermissionType Permission, string Path, bool IgnoreInheritedPermissions = false, bool AllowIfHavePermissionToSubPath = false)
        {
            return User.Permissions.HasPermission(Permission, Path, IgnoreInheritedPermissions, AllowIfHavePermissionToSubPath);
        }

        /// <summary>
        /// 
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
        /// 
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
