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

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Scripting
{
    /// <summary>
    ///     Contains some general macro functions typically used by scripts.
    /// </summary>
    public static class ScriptUtils
    {
        /// <summary>
        ///     Makes a junction point that links the target path to the target directory.
        /// </summary>
        /// <param name="SourceDirectory">Source directory to target.</param>
        /// <param name="TargetDirectory">Target directory which should link to source.</param>
        /// <returns>True on success.</returns>
        public static bool CreateJunction(string SourceDirectory, string TargetDirectory)
        {
            Logger.Log(LogLevel.Info, LogCategory.Script, "Creating symlink from '{0}' to '{1}'.", SourceDirectory, TargetDirectory);

            try
            {
                JunctionPoint.Create(TargetDirectory, SourceDirectory, true);
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Error, LogCategory.Script, "Failed to create symlink with error: {0}", Ex.Message.ToString());
                return false;
            }

            return true;
        }
        
        /// <summary>
        ///     Runs a process (executed via shell) with the given arguments.
        /// </summary>
        /// <param name="ExePath">Path to exe or command to run</param>
        /// <param name="WorkingDirectory">Directory to execute command within.</param>
        /// <param name="Arguments">Arguments to pass to command.</param>
        /// <returns>True if process has started.</returns>
        public static bool Run(string ExePath, string WorkingDirectory, string Arguments)
        {
            Logger.Log(LogLevel.Info, LogCategory.Script, "Running '{0}' in '{1}' with arguments '{2}'.", ExePath, WorkingDirectory, Arguments);

            try
            {
                ProcessStartInfo StartInfo = new ProcessStartInfo();
                StartInfo.FileName = ExePath;
                StartInfo.WorkingDirectory = WorkingDirectory;
                StartInfo.Arguments = Arguments;
                StartInfo.UseShellExecute = false;
                StartInfo.CreateNoWindow = true;

                Process.Start(StartInfo);
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Error, LogCategory.Script, "Failed to run program with error: {0}", Ex.Message.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Runs a process (executed via shell) with the given arguments and waits for it to finish.
        /// </summary>
        /// <param name="ExePath">Path to exe or command to run</param>
        /// <param name="WorkingDirectory">Directory to execute command within.</param>
        /// <param name="Arguments">Arguments to pass to command.</param>
        /// <returns>Exit code returned by the process. If process fails to start -1 is returned.</returns>
        public static int RunAndWait(string ExePath, string WorkingDirectory, string Arguments)
        {
            Logger.Log(LogLevel.Info, LogCategory.Script, "Running (and waiting for result) '{0}' in '{1}' with arguments '{2}'.", ExePath, WorkingDirectory, Arguments);

            try
            {
                ProcessStartInfo StartInfo = new ProcessStartInfo();
                StartInfo.FileName = ExePath;
                StartInfo.WorkingDirectory = WorkingDirectory;
                StartInfo.Arguments = Arguments;
                StartInfo.RedirectStandardOutput = true;
                StartInfo.RedirectStandardError = true;
                StartInfo.UseShellExecute = false;
                StartInfo.CreateNoWindow = true;

                Process process = Process.Start(StartInfo);
                process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e) { Logger.Log(LogLevel.Info, LogCategory.Main, "{0}", e.Data); };
                process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e) { Logger.Log(LogLevel.Info, LogCategory.Main, "{0}", e.Data); };

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                process.WaitForExit();

                return process.ExitCode;
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Error, LogCategory.Script, "Failed to run program with error: {0}", Ex.Message.ToString());
                return -1;
            }
        }
    }
}
