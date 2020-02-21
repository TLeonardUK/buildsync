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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Scm.Git
{
    /// <summary>
    /// </summary>
    /// <param name="Repo"></param>
    public delegate void GitCommandHandler();

    /// <summary>
    /// </summary>
    public class GitScmProvider : IScmProvider
    {
        /// <summary>
        /// </summary>
        private const int SYNC_TIME_RECHECK_INTERVAL = 2 * 60 * 1000;

        /// <summary>
        /// </summary>
        private readonly BlockingCollection<GitCommandHandler> CommandQueue = new BlockingCollection<GitCommandHandler>();

        /// <summary>
        /// </summary>
        private Thread CommandThread;

        /// <summary>
        /// </summary>
        private ulong LastRequestedSyncTime;

        /// <summary>
        /// </summary>
        private DateTime SyncTime = DateTime.MinValue;

        /// <summary>
        /// </summary>
        private bool SyncTimeRequesting;

        /// <summary>
        /// </summary>
        /// <param name="InServerName"></param>
        /// <param name="InUsername"></param>
        /// <param name="InPassword"></param>
        public GitScmProvider(string InServerName, string InUsername, string InPassword, string InRoot)
        {
            Server = InServerName;
            Username = InUsername;
            Password = InPassword;
            Root = FileUtils.NormalizePath(InRoot);

            CommandThread = new Thread(CommandThreadEntry);
            CommandThread.IsBackground = true;
            CommandThread.Name = "Perforce Commands";
            CommandThread.Start();
        }

        /// <summary>
        /// </summary>
        public string Server { get; } = "";

        /// <summary>
        /// </summary>
        public string Username { get; } = "";

        /// <summary>
        /// </summary>
        public string Password { get; } = "";

        /// <summary>
        /// </summary>
        public string Root { get; } = "";

        /// <summary>
        /// </summary>
        public void Poll()
        {
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public DateTime GetSyncTime()
        {
            if (SyncTimeRequesting == false)
            {
                SyncTimeRequesting = true;

                ulong Current = TimeUtils.Ticks;
                ulong Elapsed = Current - LastRequestedSyncTime;
                if (Elapsed > SYNC_TIME_RECHECK_INTERVAL)
                {
                    QueueAndAwaitCommand(
                        () =>
                        {
                            try
                            {
                                Logger.Log(LogLevel.Info, LogCategory.Scm, "Requesting sync time of git repo: {0}", Root);

                                string Output = ExecuteGitCommand("show");
                                string StartTag = "Date:";
                                int StartDateOffset = Output.IndexOf(StartTag);
                                if (StartDateOffset >= 0)
                                {
                                    StartDateOffset += StartTag.Length;

                                    int EndDateOffset = Output.IndexOf("\n", StartDateOffset);
                                    if (EndDateOffset >= 0)
                                    {
                                        string Date = Output.Substring(StartDateOffset, EndDateOffset - StartDateOffset).Trim();

                                        DateTimeOffset DateOffset;
                                        if (!DateTimeOffset.TryParseExact(Date, "ddd MMM d HH:mm:ss yyyy K", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOffset))
                                        {
                                            Logger.Log(LogLevel.Error, LogCategory.Scm, "Failed to parse time returned by git command: {0}", Date);
                                        }
                                        else
                                        {
                                            SyncTime = DateOffset.DateTime;
                                            Logger.Log(LogLevel.Info, LogCategory.Scm, "Workspace '{0}' last have changeset has timestamp {1}", Root, SyncTime.ToString());
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                SyncTimeRequesting = false;
                            }
                        }
                    );
                }

                LastRequestedSyncTime = Current;
            }

            return SyncTime;
        }

        /// <summary>
        /// </summary>
        public void Terminate()
        {
            CommandQueue.CompleteAdding();
            CommandThread.Join();
            CommandThread = null;
        }

        /// <summary>
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public string ExecuteGitCommand(params string[] arguments)
        {
            ProcessStartInfo StartInfo = new ProcessStartInfo();
            StartInfo.FileName = "git";
            StartInfo.WorkingDirectory = Root;
            StartInfo.Arguments = string.Join(" ", arguments);
            StartInfo.RedirectStandardOutput = true;
            StartInfo.RedirectStandardError = true;
            StartInfo.UseShellExecute = false;
            StartInfo.CreateNoWindow = true;

            Process process = Process.Start(StartInfo);

            string Output = "";

            process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Scm, "{0}", e.Data);
                Output += e.Data + "\n";
            };

            process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) { Logger.Log(LogLevel.Info, LogCategory.Scm, "{0}", e.Data); };

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.WaitForExit();

            return process.ExitCode == 0 ? Output : "";
        }

        /// <summary>
        /// </summary>
        /// <param name="Callback"></param>
        public void QueueAndAwaitCommand(GitCommandHandler Callback)
        {
            ManualResetEvent Event = new ManualResetEvent(false);

            QueueCommand(
                () =>
                {
                    try
                    {
                        Callback();
                    }
                    catch (Exception Ex)
                    {
                        Logger.Log(LogLevel.Error, LogCategory.Scm, "Encountered exception while running git command: {0}", Ex.Message);
                    }
                    finally
                    {
                        Event.Set();
                    }
                }
            );

            Event.WaitOne();
        }

        /// <summary>
        /// </summary>
        /// <param name="Callback"></param>
        public void QueueCommand(GitCommandHandler Callback)
        {
            CommandQueue.Add(Callback);
        }

        /// <summary>
        /// </summary>
        private void CommandThreadEntry()
        {
            while (!CommandQueue.IsCompleted)
            {
                // Server name valid?
                if (Server == string.Empty)
                {
                    Thread.Sleep(1 * 1000);
                    continue;
                }

                // Execute next command.
                GitCommandHandler Handler = CommandQueue.Take();
                try
                {
                    Handler();
                }
                catch (Exception Ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.Scm, "Encountered exception while running git command: {0}", Ex.Message);
                }
            }
        }
    }
}