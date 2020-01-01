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
    [Verb("grantpermission", HelpText = "Adds the given permission type to a user.")]
    public class CommandLineGrantPermissionOptions
    {
        [Value(0, MetaName = "Username", Required = true, HelpText = "Username of user to give permission to, if on a domain the domain should be included in format Domain\\Username.")]
        public string Username { get; set; } = "";

        [Value(1, MetaName = "Permission", Required = true, HelpText = "Type of permission that should be granted.")]
        public string Permission { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        internal void Run(CommandIPC IpcClient)
        {
            UserPermission PermissionType;
            if (!Enum.TryParse<UserPermission>(Permission, out PermissionType))
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
                if (user.Permissions.Contains(PermissionType))
                {
                    IpcClient.Respond(string.Format("FAILED: User already has the permission."));
                    return;
                }

                Program.UserManager.GrantPermission(user, PermissionType);
                IpcClient.Respond(string.Format("SUCCESS: Granted user '{0}' permission '{1}'.", Username, Permission));
            }
        }
    }
}
