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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Utils;
using BuildSync.Core.Users;
using CommandLine;

namespace BuildSync.Server.Commands
{
    [Verb("revokepermission", HelpText = "Removes the given permission type from a user.")]
    public class CommandLineRevokePermissionOptions
    {
        [Value(0, MetaName = "Username", Required = true, HelpText = "Username of user to remove permission from, if on a domain the domain should be included in format Domain\\Username.")]
        public string Username { get; set; } = "";

        [Value(1, MetaName = "Permission", Required = true, HelpText = "Type of permission that should be revoked.")]
        public string Permission { get; set; } = "";

        [Value(2, MetaName = "Path", Required = false, HelpText = "Virtual path that permission is valid on. All decendents also have permission.")]
        public string Path { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        internal void Run(CommandIPC IpcClient)
        {
            UserPermissionType PermissionType;
            if (!Enum.TryParse<UserPermissionType>(Permission, out PermissionType))
            {
                IpcClient.Respond(string.Format("FAILED: Permission '{0}' is not valid.", Permission));
                return;
            }

            User user = Program.UserManager.FindUser(Username);
            if (user == null)
            {
                IpcClient.Respond(string.Format("FAILED: Username '{0}' does not exist.", Username));
            }
            else
            {
                if (!Program.UserManager.CheckPermission(Username, PermissionType, Path, true))
                {
                    IpcClient.Respond(string.Format("FAILED: User does not have the permission."));
                    return;
                }

                Program.UserManager.RevokePermission(user, PermissionType, Path);
                IpcClient.Respond(string.Format("SUCCESS: Revoked user '{0}' permission '{1}' on '{2}'.", Username, Permission, Path));
            }
        }
    }
}
