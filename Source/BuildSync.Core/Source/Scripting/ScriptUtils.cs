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
using System.Reflection;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildSync.Core.Utils;
using SharpCompress;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace BuildSync.Core.Scripting
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="State"></param>
    /// <param name="Progress"></param>
    public delegate void ScriptBuildOutputCallbackDelegate(string Line);

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
        public static int RunAndWait(string ExePath, string WorkingDirectory, string Arguments, ScriptBuildOutputCallbackDelegate OutputCallback = null)
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
                /*process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e) 
                {
                    OutputLineBuilder.Add(e.Data);
                    while (true)
                    {
                        string Line = OutputLineBuilder.Read();
                        if (Line == null)
                        {
                            break;
                        }
                        Logger.Log(LogLevel.Info, LogCategory.Main, "{0}", Line);
                    }

                    OutputCallback?.Invoke(e.Data);
                };
                process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
                {
                    while (true)
                    {
                        string Line = ErrorLineBuilder.Read();
                        if (Line == null)
                        {
                            break;
                        }
                        Logger.Log(LogLevel.Info, LogCategory.Main, "{0}", Line);
                    }

                    OutputCallback?.Invoke(e.Data);
                };

                //process.BeginErrorReadLine();
                //process.BeginOutputReadLine();
                */

                Func<StreamReader, ConcurrentQueue<string>, bool> QueueBuilder = (StreamReader Reader, ConcurrentQueue<string> Queue) =>
                {
                    int Char = Reader.Read();
                    if (Char < 0)
                    {
                        return true;
                    }

                    string Value = new string((char)Char, 1);
                    Queue.Enqueue(Value);

                    return false;
                };

                Func<ConcurrentQueue<string>, LineBuilder, bool> Parser = (ConcurrentQueue<string> Queue, LineBuilder Builder) =>
                {
                    while (Queue.Count > 0)
                    {
                        string Value = "";
                        if (Queue.TryDequeue(out Value))
                        {
                            Builder.Add(Value);
                            while (true)
                            {
                                string Line = Builder.Read();
                                if (Line == null)
                                {
                                    break;
                                }
                                Logger.Log(LogLevel.Info, LogCategory.Main, "{0}", Line);
                            }

                            OutputCallback?.Invoke(Value);
                        }
                    }

                    return true;
                };

                Func<LineBuilder, bool> DrainBuilder = (LineBuilder Builder) =>
                {
                    Builder.End();
                    while (true)
                    {
                        string Line = Builder.Read();
                        if (Line == null)
                        {
                            break;
                        }
                        Logger.Log(LogLevel.Info, LogCategory.Main, "{0}", Line);
                    }

                    return true;
                };

                LineBuilder OutputLineBuilder = new LineBuilder();
                LineBuilder ErrorLineBuilder = new LineBuilder();

                // This is retarded, the blocking api for all of this in C# is shite.
                ConcurrentQueue<string> OutputData = new ConcurrentQueue<string>();
                ConcurrentQueue<string> ErrorData = new ConcurrentQueue<string>();

                Task OutputTask = Task.Run(() => { while (!QueueBuilder(process.StandardOutput, OutputData)) ; });
                Task ErrorTask = Task.Run(() => { while (!QueueBuilder(process.StandardError, ErrorData)) ; });

                // Wait for process to exit.
                while (!OutputTask.IsCompleted || !ErrorTask.IsCompleted || OutputData.Count > 0 || ErrorData.Count > 0)
                {
                    Parser(OutputData, OutputLineBuilder);
                    Parser(ErrorData, ErrorLineBuilder);
                    Thread.Sleep(1);
                }

                process.WaitForExit();

                // Drain any output.
                DrainBuilder(OutputLineBuilder);
                DrainBuilder(ErrorLineBuilder);

                Logger.Log(LogLevel.Info, LogCategory.Script, "Finished running with exit code {0}.", process.ExitCode);
                return process.ExitCode;
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Error, LogCategory.Script, "Failed to run program with error: {0}", Ex.Message.ToString());
                return -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class UnpackStatus
        {
            public long TotalSize;
            public IArchiveEntry CurrentEntry;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Archive"></param>
        /// <param name="OutputFolder"></param>
        /// <returns></returns>
        public static bool UnpackArchive(string ArchivePath, string OutputFolder, ScriptBuildProgressDelegate ProgressCallback = null)
        {
            string installLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string sevenZipPath = Path.Combine(Path.Combine(Path.Combine(installLocation, "Libraries"), "7zip"), "7za.exe");

            ProgressParser Parser = new ProgressParser();
            Parser.ParsePartialLines = false;
            Parser.LineSeperator = "\r";
            Parser.AddPattern(@"^\s+(\d+)\% \- (.+)$", 
                new ProgressMatch(ProgressMatchType.Progress, ProgressMatchFormat.Float, 100),
                new ProgressMatch(ProgressMatchType.CurrentFileName, ProgressMatchFormat.String));

            ScriptBuildOutputCallbackDelegate OutputCallback = (string Line) =>
            {
                Parser.Parse(Line);
                ProgressCallback?.Invoke("Decompressing "+Parser.CurrentFileName+"...", Parser.Progress);
            };

            int exitCode = RunAndWait(sevenZipPath, installLocation, "x \"" + ArchivePath  + "\" -o\"" + OutputFolder + "\" -r -y -bsp1", OutputCallback);
            return (exitCode == 0);

            /*
            
            // SharpCompress is waaaaaaaaaaaaaaaaaaaaaaaaaaaaaay to slow :|
            try
            {
                using (IArchive Archive = ArchiveFactory.Open(ArchivePath))
                {
                    int BufferLength = 1 * 1024 * 1024;
                    byte[] Buffer = new byte[BufferLength];

                    long TotalUncompressed = Archive.TotalUncompressSize;
                    long Uncompressed = 0;

                    foreach (IArchiveEntry Entry in Archive.Entries.Where(entry => !entry.IsDirectory))
                    {
                        using (Stream ArchiveStream = Entry.OpenEntryStream())
                        {
                            using (FileStream FileStream = new FileStream(OutputFolder + @"\" + Entry.Key, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                            {
                                int BytesRead = 0;
                                while ((BytesRead = ArchiveStream.Read(Buffer, 0, Buffer.Length)) > 0)
                                {
                                    FileStream.Write(Buffer, 0, BytesRead);
                                    Uncompressed += BytesRead;

                                    float Progress = (float)Uncompressed / (float)TotalUncompressed;
                                    ProgressCallback?.Invoke("Unpacking " + Entry.Key + "...", Progress);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Ex) // Should be more specific, but sharpcompress has so little documentation and looking through the code there are so many potential exceptions...
            {
                Logger.Log(LogLevel.Error, LogCategory.Script, "Failed to extract archive with error: {0}", Ex.Message.ToString());
                return false;
            }

            return true;
            */
        }
    }
}
