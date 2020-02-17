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

using BuildSync.Core.Licensing;
using BuildSync.Core.Utils;
using CommandLine;

namespace BuildSync.Server.Commands
{
    /// <summary>
    ///     CLI command for applying a license file to the server.
    /// </summary>
    [Verb("applylicense", HelpText = "Applys a local license file to the server.")]
    public class CommandLineApplyLicenseOptions
    {
        /// <summary>
        ///     Path on the local machine of the license file to apply.
        /// </summary>
        [Value(0, MetaName = "FilePath", Required = true, HelpText = "Path on the local machine of the license file to apply.")]
        public string FilePath { get; set; } = "";

        /// <summary>
        ///     Called when the CLI invokes this command.
        /// </summary>
        /// <param name="IpcClient">Interprocess communication pipe to the application that invoked this command.</param>
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
                IpcClient.Respond("FAILED: Unable to apply license, either invalid or expired.");
            }

            Program.SaveSettings();
        }
    }
}