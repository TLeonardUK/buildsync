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
    [Verb("revokepermission", HelpText = "Removes the given permission type from a user.")]
    public class CommandLineRevokePermissionOptions
    {
        [Value(0, MetaName = "Username", Required = true, HelpText = "Username of user to remove permission from, if on a domain the domain should be included in format Domain\\Username.")]
        public string Username { get; set; } = "";

        [Value(1, MetaName = "Permission", Required = true, HelpText = "Type of permission that should be revoked.")]
        public string Permission { get; set; } = "";

        [Value(2, MetaName = "Path", Required = false, HelpText = "Virtual path that permission is valid on. All decendents also have permission.")]
        public string Path { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        internal void Run(CommandIPC IpcClient)
        {
            UserPermissionType PermissionType;
            if (!Enum.TryParse<UserPermissionType>(Permission, out PermissionType))
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
                if (!Program.UserManager.CheckPermission(Username, PermissionType, Path, true))
                {
                    IpcClient.Respond(string.Format("FAILED: User does not have the permission."));
                    return;
                }

                Program.UserManager.RevokePermission(user, PermissionType, Path);
                IpcClient.Respond(string.Format("SUCCESS: Revoked user '{0}' permission '{1}' on '{2}'.", Username, Permission, Path));
            }
        }
    }
}
