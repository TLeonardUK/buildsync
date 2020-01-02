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
    [Verb("listusers", HelpText = "Lists all the users that can perform admin actions on the server.")]
    public class CommandLineListUsersOptions
    {
        /// <summary>
        /// 
        /// </summary>
        internal void Run(CommandIPC IpcClient)
        {
            string Format = "{0,-30} | {1,-30}";

            IpcClient.Respond(string.Format(Format, "Username", "Permissions"));
            foreach (User user in Program.UserManager.Users)
            {
                string Permissions = "";
                foreach (UserPermission Permission in user.Permissions.Permissions)
                {
                    if (Permissions.Length != 0)
                    {
                        Permissions += ",";
                    }
                    Permissions += Permission.Type.ToString() + "@\"" + Permission.VirtualPath + "\"";
                }

                IpcClient.Respond(string.Format(Format, user.Username, Permissions));

            }
            IpcClient.Respond("");

            Program.SaveSettings();
        }
    }
}
