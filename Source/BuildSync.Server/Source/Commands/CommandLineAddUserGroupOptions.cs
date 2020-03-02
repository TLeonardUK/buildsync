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
using CommandLine;

namespace BuildSync.Server.Commands
{
    /// <summary>
    ///     CLI command for adding a user group to the server. Bypasses the permissions that are normally required if doing this via the GUI.
    /// </summary>
    [Verb("addusergroup", HelpText = "Adds a user group.")]
    public class CommandLineAddUserGroupOptions
    {
        /// <summary>
        ///     Name of group to add.
        /// </summary>
        [Value(0, MetaName = "Name", Required = true, HelpText = "Name of group to add")]
        public string Name { get; set; } = "";

        /// <summary>
        ///     Called when the CLI invokes this command.
        /// </summary>
        /// <param name="IpcClient">Interprocess communication pipe to the application that invoked this command.</param>
        internal void Run(CommandIPC IpcClient)
        {
            if (Program.UserManager.FindGroup(Name) != null)
            {
                IpcClient.Respond(string.Format("FAILED: Group '{0}' already exists.", Name));
            }
            else
            {
                Program.UserManager.CreateGroup(Name);
                IpcClient.Respond(string.Format("SUCCESS: Added group '{0}'.", Name));
            }

            Program.SaveSettings();
        }
    }
}