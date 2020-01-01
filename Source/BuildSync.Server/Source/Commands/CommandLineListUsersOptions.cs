using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Utils;
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
            string Format = "{0,-30}";

            Logger.Log(LogLevel.Display, LogCategory.Main, Format, "Username");
            foreach (User user in Program.Settings.Users)
            {
                Logger.Log(LogLevel.Display, LogCategory.Main, Format, user.Username);
            }

            Program.SaveSettings();
        }
    }
}
