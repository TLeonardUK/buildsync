using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Utils;
using CommandLine;

namespace BuildSync.Server.Commands
{
    [Verb("adduser", HelpText = "Adds a user that can perform admin actions on the server.")]
    public class CommandLineAddUserOptions
    {
        [Value(0, MetaName = "Username", Required = true, HelpText = "Username of user to add, if on a domain the domain should be included in format Domain\\Username.")]        
        public string Username { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        internal void Run(CommandIPC IpcClient)
        {
            if (Program.UserManager.FindUser(Username) != null)
            {
                IpcClient.Respond(string.Format("FAILED: Username '{0}' already exists.", Username));
            }
            else
            {
                Program.UserManager.CreateUser(Username);
                IpcClient.Respond(string.Format("SUCCESS: Added user '{0}'.", Username));
            }

            Program.SaveSettings();
        }
    }
}
