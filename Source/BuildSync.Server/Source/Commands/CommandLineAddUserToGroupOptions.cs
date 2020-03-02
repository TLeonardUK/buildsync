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

using BuildSync.Core.Utils;
using BuildSync.Core.Users;
using CommandLine;

namespace BuildSync.Server.Commands
{
    /// <summary>
    ///     CLI command for adding a user to a group on to the server. Bypasses the permissions that are normally required if doing this via the GUI.
    /// </summary>
    [Verb("addusertogroup", HelpText = "Adds a user to a group.")]
    public class CommandLineAddUserToGroupOptions
    {
        /// <summary>
        ///     Name of username to add to group.
        /// </summary>
        [Value(0, MetaName = "Username", Required = true, HelpText = "Username of user to add")]
        public string Username { get; set; } = "";

        /// <summary>
        ///     Name of group to add to.
        /// </summary>
        [Value(1, MetaName = "Name", Required = true, HelpText = "Name of group to add")]
        public string GroupName { get; set; } = "";

        /// <summary>
        ///     Called when the CLI invokes this command.
        /// </summary>
        /// <param name="IpcClient">Interprocess communication pipe to the application that invoked this command.</param>
        internal void Run(CommandIPC IpcClient)
        {
            UserGroup group = Program.UserManager.FindGroup(GroupName);
            if (group == null)
            {
                IpcClient.Respond(string.Format("FAILED: Group '{0}' does not exist.", GroupName));
                return;
            }

            User user = Program.UserManager.FindUser(Username);
            if (user == null)
            {
                IpcClient.Respond(string.Format("FAILED: User '{0}' does not exist.", GroupName));
                return;
            }

            Program.UserManager.AddUserToGroup(user, group);
            IpcClient.Respond(string.Format("SUCCESS: Added user '{0}' to group '{1}'.", Username, GroupName));

            Program.SaveSettings();
        }
    }
}