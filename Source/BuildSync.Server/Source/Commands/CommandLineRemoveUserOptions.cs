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

using BuildSync.Core.Users;
using BuildSync.Core.Utils;
using CommandLine;

namespace BuildSync.Server.Commands
{
    [Verb("removeuser", HelpText = "Removes a user that can perform admin actions on the server.")]
    public class CommandLineRemoveUserOptions
    {
        [Value(0, MetaName = "Username", Required = true, HelpText = "Username of user to remove, if on a domain the domain should be included in format Domain\\Username.")]
        public string Username { get; set; } = "";

        /// <summary>
        /// </summary>
        internal void Run(CommandIPC IpcClient)
        {
            User ExistingUser = Program.UserManager.FindUser(Username);
            if (ExistingUser != null)
            {
                Program.UserManager.DeleteUser(ExistingUser);
                IpcClient.Respond(string.Format("SUCCESS: Removed user '{0}'.", Username));
            }
            else
            {
                IpcClient.Respond(string.Format("FAILED: Username '{0}' does not exist.", Username));
            }
        }
    }
}