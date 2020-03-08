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
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BuildSync.Core.Networking;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Users
{
    /// <summary>
    /// </summary>
    [Serializable]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserPermissionType
    {
        [Description("Write")]
        Write,

        [Description("Modify Users")]
        ModifyUsers,

        [Description("Read")]
        Read,

        [Description("Push Update")]
        PushUpdate,

        [Description("Modify Server")]
        ModifyServer,

        [Description("Add Users To Group")]
        AddUsersToGroup,

        [Description("Tag Builds")]
        TagBuilds,

        [Description("Modify Tags")]
        ModifyTags,

        [Description("Unknown")]
        Unknown
    }

    /// <summary>
    /// </summary>
    [Serializable]
    public class UserPermission
    {
        private string InternalVirtualPath = "";

        /// <summary>
        /// 
        /// </summary>
        public UserPermissionType Type { get; set; } = UserPermissionType.Unknown;

        /// <summary>
        /// 
        /// </summary>
        public string VirtualPath
        {
            get => InternalVirtualPath;
            set
            {
                InternalVirtualPath = value;
                VirtualPathSplit = value.ToLower().Split('/');
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public string[] VirtualPathSplit { get; private set; } = new string[0];
    }

    /// <summary>
    /// </summary>
    [Serializable]
    public class UserPermissionCollection
    {
        public List<UserPermission> Permissions { get; set; } = new List<UserPermission>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Other"></param>
        public void Merge(UserPermissionCollection Other)
        {
            foreach (UserPermission OtherPermission in Other.Permissions)
            {
                GrantPermission(OtherPermission.Type, OtherPermission.VirtualPath);
            }
        }

        /// <summary>
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
        /// <param name="Type"></param>
        /// <returns></returns>
        public bool HasAnyPermissionOfType(UserPermissionType Type)
        {
            foreach (UserPermission Permission in Permissions)
            {
                if (Permission.Type == Type)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Path"></param>
        /// <returns></returns>
        public bool HasPermission(UserPermissionType Type, string Path, bool IgnoreInheritedPermissions = false, bool AllowIfHavePermissionToSubPath = false)
        {
            // We can always view internal paths (ones that start with $), these are used for updates etc.
            if (Path.StartsWith("$") && Type == UserPermissionType.Read)
            {
                return true;
            }

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
                            // We can early out if we have root permission.
                            if (SplitPath.Length == 0 || (SplitPath.Length == 1 && SplitPath[0] == ""))
                            {
                                return true;
                            }

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

                serializer.SerializeEnum(ref Type);
                serializer.Serialize(ref Path);

                Permissions[i].Type = Type;
                Permissions[i].VirtualPath = Path;
            }
        }
    }
}