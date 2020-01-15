﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Scm.Git
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Repo"></param>
    public delegate void GitCommandHandler();

    /// <summary>
    /// 
    /// </summary>
    public class GitScmProvider : IScmProvider
    {
        /// <summary>
        /// 
        /// </summary>
        private BlockingCollection<GitCommandHandler> CommandQueue = new BlockingCollection<GitCommandHandler>();

        /// <summary>
        /// 
        /// </summary>
        public string Server { get; private set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string Username { get; private set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; private set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string Root { get; private set; } = "";

        /// <summary>
        /// 
        /// </summary>
        private Thread CommandThread = null;

        /// <summary>
        /// 
        /// </summary>
        private ulong LastRequestedSyncTime = 0;

        /// <summary>
        /// 
        /// </summary>
        private const int SYNC_TIME_RECHECK_INTERVAL = 2 * 60 * 1000;

        /// <summary>
        /// 
        /// </summary>
        private DateTime SyncTime = DateTime.MinValue;

        /// <summary>
        /// 
        /// </summary>
        private bool SyncTimeRequesting = false;

        /// <summary>
        /// 
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
            CommandThread.Name = "Perforce Commands";
            CommandThread.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Poll()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Callback"></param>
        public void QueueAndAwaitCommand(GitCommandHandler Callback)
        {
            ManualResetEvent Event = new ManualResetEvent(false);

            QueueCommand(() =>
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
            });

            Event.WaitOne();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Callback"></param>
        public void QueueCommand(GitCommandHandler Callback)
        {
            CommandQueue.Add(Callback);
        }

        /// <summary>
        /// 
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

        /// <summary>
        /// 
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

            process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e)
            {
                Logger.Log(LogLevel.Verbose, LogCategory.Scm, "{0}", e.Data);
                Output += e.Data + "\n";
            };

            process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
            {
                Logger.Log(LogLevel.Info, LogCategory.Scm, "{0}", e.Data);
            };

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.WaitForExit();

            return process.ExitCode == 0 ? Output : "";
        }

        /// <summary>
        /// 
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
                    QueueAndAwaitCommand(() =>
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
                    });
                }

                LastRequestedSyncTime = Current;
            }

            return SyncTime;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Terminate()
        {
            CommandQueue.CompleteAdding();
            CommandThread.Join();
            CommandThread = null;
        }
    }
}