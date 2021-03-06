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

using BuildSync.Core.Users;
using BuildSync.Core.Utils;
using CommandLine;

namespace BuildSync.Server.Commands
{
    /// <summary>
    ///     CLI command for listing all users, and their permissions, that are registered on the server.
    /// </summary>
    [Verb("listusers", HelpText = "Lists all the users that can perform admin actions on the server.")]
    public class CommandLineListUsersOptions
    {
        /// <summary>
        ///     Called when the CLI invokes this command.
        /// </summary>
        /// <param name="IpcClient">Interprocess communication pipe to the application that invoked this command.</param>
        internal void Run(CommandIPC IpcClient)
        {
            string Format = "{0,-30} | {1,-30}";

            IpcClient.Respond(string.Format(Format, "Username", "Groups"));
            foreach (User user in Program.UserManager.Users)
            {
                string Permissions = "";
                foreach (string Group in user.Groups)
                {
                    if (Permissions.Length != 0)
                    {
                        Permissions += ",";
                    }

                    Permissions += Group;
                }

                IpcClient.Respond(string.Format(Format, user.Username, Permissions));
            }

            IpcClient.Respond("");

            Program.SaveSettings();
        }
    }
}