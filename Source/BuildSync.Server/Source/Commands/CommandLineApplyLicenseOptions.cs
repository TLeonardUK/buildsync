using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Utils;
using BuildSync.Core.Licensing;
using CommandLine;

namespace BuildSync.Server.Commands
{
    [Verb("applylicense", HelpText = "Applys a local license file to the server.")]
    public class CommandLineApplyLicenseOptions
    {
        [Value(0, MetaName = "FilePath", Required = true, HelpText = "Path on the local machine of the license file to apply.")]
        public string FilePath { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        internal void Run(CommandIPC IpcClient)
        {
            License license = License.Load(FilePath);
            if (license == null)
            {
                IpcClient.Respond(string.Format("FAILED: Unable to load valid license from '{0}'.", FilePath));
            }

            if (Program.LicenseMgr.Apply(license))
            {
                IpcClient.Respond(string.Format("SUCCESS: Applied license for '{0}' seats which is licensed to '{1}'.", license.MaxSeats == int.MaxValue ? "Unlimited" : license.MaxSeats.ToString(), license.LicensedTo));
            }
            else
            {
                IpcClient.Respond(string.Format("FAILED: Unable to apply license, either invalid or expired."));
            }

            Program.SaveSettings();
        }
    }
}
