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
using BuildSync.Core.Users;
using BuildSync.Core.Utils;
using CommandLine;

namespace BuildSync.Server.Commands
{
    /// <summary>
    ///     CLI command for revoking a previously granted access permission for the given user.
    /// </summary>
    [Verb("revokepermission", HelpText = "Removes the given permission type from a user.")]
    public class CommandLineRevokePermissionOptions
    {
        /// <summary>
        ///     Virtual path that permission is valid on. All decendents also have permission.
        /// </summary>
        [Value(2, MetaName = "Path", Required = false, HelpText = "Virtual path that permission is valid on. All decendents also have permission.")]
        public string Path { get; set; } = "";

        /// <summary>
        ///     Type of permission that should be revoked.
        /// </summary>
        [Value(1, MetaName = "Permission", Required = true, HelpText = "Type of permission that should be revoked.")]
        public string Permission { get; set; } = "";

        /// <summary>
        ///     Username of user to remove permission from, if on a domain the domain should be included in format Domain\\Username.
        /// </summary>
        [Value(0, MetaName = "Username", Required = true, HelpText = "Username of user to remove permission from, if on a domain the domain should be included in format Domain\\Username.")]
        public string Username { get; set; } = "";

        /// <summary>
        ///     Called when the CLI invokes this command.
        /// </summary>
        /// <param name="IpcClient">Interprocess communication pipe to the application that invoked this command.</param>
        internal void Run(CommandIPC IpcClient)
        {
            UserPermissionType PermissionType;
            if (!Enum.TryParse(Permission, out PermissionType))
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
                    IpcClient.Respond("FAILED: User does not have the permission.");
                    return;
                }

                Program.UserManager.RevokePermission(user, PermissionType, Path);
                IpcClient.Respond(string.Format("SUCCESS: Revoked user '{0}' permission '{1}' on '{2}'.", Username, Permission, Path));
            }
        }
    }
}