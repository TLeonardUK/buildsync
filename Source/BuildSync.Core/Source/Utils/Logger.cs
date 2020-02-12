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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public enum LogLevel
    {
        Verbose,    // Debug information
        Info,       // General logs and useful information
        Display,    // Logs that should always display for the purpose of command line processing.
        Warning,    // Problems which may not be desirable but which will not cause an issue.
        Error       // Problems which will cause a direct issue.
    }

    /// <summary>
    /// 
    /// </summary>
    public enum LogCategory
    {
        Main,
        Manifest,
        Download,
        Transport,
        Peers,
        IO,
        Users,
        Licensing,
        Scm
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class LogSink
    {
        public abstract void Close();
        public abstract void Log(LogLevel Level, LogCategory Category, string Message, string RawMessage);
    }

    /// <summary>
    /// 
    /// </summary>
    public class ConsoleLogSink : LogSink
    {
        public override void Close()
        {
            // Nothing to do
        }

        public override void Log(LogLevel Level, LogCategory Category, string Message, string RawMessage)
        {
            if (Logger.DisplayOutputOnly)
            {
                if (Level == LogLevel.Display)
                {
                    Console.WriteLine(RawMessage);
                    Debug.WriteLine(RawMessage);
                }
            }
            else
            {
                Console.WriteLine(Message);
                //Debug.WriteLine(Message);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class FileLogSink : LogSink
    {
        private FileStream Stream = null;
        private StreamWriter Writer = null;

        public FileLogSink(string FilePath)
        {
            for (int i = 0; i < 10; i++)
            {
                string AttemptPath = FilePath;
                if (i > 0)
                {
                    AttemptPath = FilePath + "." + i;
                }

                try
                {
                    string DirPath = Path.GetDirectoryName(AttemptPath);
                    if (!Directory.Exists(DirPath))
                    {
                        Directory.CreateDirectory(DirPath);
                    }

                    Stream = new FileStream(AttemptPath, FileMode.Append, FileAccess.Write, FileShare.Read);
                    Writer = new StreamWriter(Stream);
                    break;
                }
                catch (IOException Ex)
                {
                    Console.WriteLine("Failed to open file '{0}' with error: {1}", AttemptPath, Ex.Message);
                }
            }
        }

        ~FileLogSink()
        {
            Close();
        }

        public override void Close()
        {
            if (Writer != null)
            {
                Writer.Close();
                Writer = null;
            }
            if (Stream != null)
            {
                Stream.Close();
                Stream = null;
            }

        }

        public override void Log(LogLevel Level, LogCategory Category, string Message, string RawMessage)
        {
            if (Writer != null)
            {
                Writer.WriteLine(Message);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// 
        /// </summary>
        private static List<LogSink> Sinks = new List<LogSink>();

        /// <summary>
        /// 
        /// </summary>
        public static LogLevel MaximumVerbosity = LogLevel.Info;

        /// <summary>
        /// 
        /// </summary>
        public static bool DisplayOutputOnly = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sink"></param>
        public static void RegisterSink(LogSink Sink)
        {
            lock (Sinks)
            {
                Sinks.Add(Sink);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sink"></param>
        public static void UnregisterSink(LogSink Sink)
        {
            lock (Sinks)
            {
                Sinks.Remove(Sink);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="Category"></param>
        /// <param name="Format"></param>
        /// <param name="Arguments"></param>
        public static void Log(LogLevel Level, LogCategory Category, string Format, params object[] Arguments)
        {
            if ((int)Level < (int)MaximumVerbosity)
            {
                return;
            }

            string Msg = string.Format(Format, Arguments);

            string MsgDateTime = DateTime.Now.ToString("G");
            string FinalMsg = string.Format("{0} | {1,-10} | {2,-10} | {3}", MsgDateTime, Category.ToString(), Level.ToString(), Msg);

            lock (Sinks)
            {
                foreach (LogSink Sink in Sinks)
                {
                    Sink.Log(Level, Category, FinalMsg, Msg);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LoggingFolder"></param>
        public static void SetupStandardLogging(string LoggingFolder, bool InCommandLineMode)
        {
            if (InCommandLineMode)
            {
                // Console mode should only be showing display/warning/error messages, logs shouldn't be shown.
                // They also should not have datestamps etc prefixed.
                DisplayOutputOnly = true;
                MaximumVerbosity = LogLevel.Display;
            }

            RegisterSink(new ConsoleLogSink());
            RegisterSink(new FileLogSink(Path.Combine(LoggingFolder, "program.log")));

            AppDomain.CurrentDomain.ProcessExit += (object sender, EventArgs e) =>
            {
                lock (Sinks)
                {
                    foreach (LogSink Sink in Sinks)
                    {
                        Sink.Close();
                    }
                    Sinks.Clear();
                }
            };
        }
    }
}
