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
    ///     CLI command for granting an access permission to the given group on the server.
    /// </summary>
    [Verb("grantpermission", HelpText = "Adds the given permission type to a group.")]
    public class CommandLineGrantPermissionOptions
    {
        /// <summary>
        ///     Virtual path that permission is valid on. All decendents also have permission.
        /// </summary>
        [Value(2, MetaName = "Path", Required = false, HelpText = "Virtual path that permission is valid on. All decendents also have permission.")]
        public string Path { get; set; } = "";

        /// <summary>
        ///     Type of permission that should be granted.
        /// </summary>
        [Value(1, MetaName = "Permission", Required = true, HelpText = "Type of permission that should be granted.")]
        public string Permission { get; set; } = "";

        /// <summary>
        ///     Name of group to give permission to.
        /// </summary>
        [Value(0, MetaName = "Name", Required = true, HelpText = "Name of group to give permission to.")]
        public string Name { get; set; } = "";

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

            UserGroup group = Program.UserManager.FindGroup(Name);
            if (group == null)
            {
                IpcClient.Respond(string.Format("FAILED: Group '{0}' does not exist.", Name));
            }
            else
            {
                Program.UserManager.GrantPermission(group, PermissionType, Path);
                IpcClient.Respond(string.Format("SUCCESS: Granted group '{0}' permission '{1}' on '{2}'.", Name, Permission, Path));
            }
        }
    }
}