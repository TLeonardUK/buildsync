using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// 
        /// </summary>
        internal void Run(CommandIPC IpcClient)
        {
            User ExistingUser = Program.Settings.GetUser(Username);
            if (ExistingUser != null)
            {
                Program.Settings.Users.Remove(ExistingUser);

                Logger.Log(LogLevel.Display, LogCategory.Main, "SUCCESS: Removed user '{0}'.", Username);
            }
            else
            {
                Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Username '{0}' does not exist.", Username);
            }

            Program.SaveSettings();
        }
    }
}
