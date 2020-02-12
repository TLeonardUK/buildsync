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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BuildSync.Core.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace BuildSync.Core.Manifests
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BuildLaunchVariableDataType
    {
        String,
        Int,
        Float,
        Bool
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BuildLaunchVariable
    {
        /// <summary>
        /// 
        /// </summary>
        public bool ConditionResult;

        /// <summary>
        /// 
        /// </summary>
        public bool Internal;

        /// <summary>
        /// 
        /// </summary>
        public string Condition { get; set; } = "true";

        /// <summary>
        /// 
        /// </summary>
        public BuildLaunchVariableDataType DataType { get; set; } = BuildLaunchVariableDataType.String;

        /// <summary>
        /// 
        /// </summary>
        public string FriendlyCategory { get; set; } = "Launch Settings";

        /// <summary>
        /// 
        /// </summary>
        public string FriendlyDescription { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string FriendlyName { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public float MaxValue { get; set; } = 1000;

        /// <summary>
        /// 
        /// </summary>
        public float MinValue { get; set; } = -1000;

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public List<string> Options { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; } = "";

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public BuildLaunchVariable ShallowClone()
        {
            return (BuildLaunchVariable) MemberwiseClone();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BuildLaunchArgument
    {
        /// <summary>
        /// 
        /// </summary>
        public string Condition { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; } = "";

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public BuildLaunchArgument ShallowClone()
        {
            return (BuildLaunchArgument) MemberwiseClone();
        }
    }

    [Serializable]
    public class BuildInstallStep
    {
        /// <summary>
        /// 
        /// </summary>
        public string Arguments { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string Executable { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public bool IgnoreErrors { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public string WorkingDirectory { get; set; } = "";

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public BuildInstallStep ShallowClone()
        {
            return (BuildInstallStep) MemberwiseClone();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BuildLaunchMode
    {
        /// <summary>
        /// 
        /// </summary>
        public List<BuildLaunchArgument> Arguments { get; set; } = new List<BuildLaunchArgument>();

        /// <summary>
        /// 
        /// </summary>
        public string Condition { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string Executable { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public List<BuildInstallStep> InstallSteps { get; set; } = new List<BuildInstallStep>();

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public List<BuildLaunchVariable> Variables { get; set; } = new List<BuildLaunchVariable>();

        /// <summary>
        /// 
        /// </summary>
        public string WorkingDirectory { get; set; } = "";

        /// <summary>
        /// </summary>
        public void AddStringVariable(string Name, string Value)
        {
            BuildLaunchVariable Variable = new BuildLaunchVariable();
            Variable.Condition = "true";
            Variable.ConditionResult = true;
            Variable.DataType = BuildLaunchVariableDataType.String;
            Variable.Name = Name;
            Variable.Value = Value;
            Variable.Internal = true;
            Variables.Add(Variable);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public string CompileArguments()
        {
            string Result = "";
            foreach (BuildLaunchArgument Argument in Arguments)
            {
                if (BuildSettings.Evaluate(Argument.Condition, Variables))
                {
                    string Escape = BuildSettings.ExpandArguments(Argument.Value, Variables);
                    if (Result.Length > 0)
                    {
                        Result += " ";
                    }

                    Result += Escape;
                }
            }

            return Result;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public BuildLaunchMode DeepClone()
        {
            BuildLaunchMode other = (BuildLaunchMode) MemberwiseClone();
            other.Variables = new List<BuildLaunchVariable>();
            foreach (BuildLaunchVariable variable in Variables)
            {
                other.Variables.Add(variable.ShallowClone());
            }

            other.Arguments = new List<BuildLaunchArgument>();
            foreach (BuildLaunchArgument argument in Arguments)
            {
                other.Arguments.Add(argument.ShallowClone());
            }

            other.InstallSteps = new List<BuildInstallStep>();
            foreach (BuildInstallStep argument in InstallSteps)
            {
                other.InstallSteps.Add(argument.ShallowClone());
            }

            return other;
        }

        /// <summary>
        /// </summary>
        public bool Install(string LocalFolder, ref string ResultMessage)
        {
            if (InstallSteps.Count == 0)
            {
                return true;
            }

            foreach (BuildInstallStep Step in InstallSteps)
            {
                if (!InstallStep(LocalFolder, ref ResultMessage, Step))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// </summary>
        public bool Launch(string LocalFolder, ref string ResultMessage)
        {
            // This is now done in launch dialog or manifest downloader.
            /*if (InstallSteps.Count != 0)
            {
                if (!Install(LocalFolder, ref ResultMessage))
                {
                    return false;
                }
            }*/

            string ExePath = BuildSettings.ExpandArguments(Executable, Variables);
            if (!Path.IsPathRooted(ExePath))
            {
                ExePath = Path.Combine(LocalFolder, ExePath);
            }

            string WorkingDir = BuildSettings.ExpandArguments(WorkingDirectory, Variables);
            if (WorkingDir.Length == 0)
            {
                WorkingDir = Path.GetDirectoryName(ExePath);
            }
            else
            {
                if (!Path.IsPathRooted(WorkingDir))
                {
                    WorkingDir = Path.Combine(LocalFolder, WorkingDir);
                }
            }

#if SHIPPING
            if (!File.Exists(ExePath))
            {
                ResultMessage = "Could not find executable, expected to be located at: " + ExePath;
                return false;
            }

            if (!Directory.Exists(WorkingDir))
            {
                ResultMessage = "Could not find working directory, expected at: " + WorkingDir;
                return false;
            }
#endif

            string CompiledArguments = "";
            try
            {
                CompiledArguments = CompileArguments();
            }
            catch (InvalidOperationException Ex)
            {
                ResultMessage = "Error encountered while evaluating launch settings:\n\n" + Ex.Message;
                return false;
            }

            Logger.Log(LogLevel.Info, LogCategory.Main, "Executing: {0} {1}", ExePath, CompiledArguments);

            try
            {
                ProcessStartInfo StartInfo = new ProcessStartInfo();
                StartInfo.FileName = ExePath;
                StartInfo.WorkingDirectory = WorkingDir;
                StartInfo.Arguments = CompiledArguments;
                //StartInfo.RedirectStandardOutput = true;
                //StartInfo.RedirectStandardError = true;
                StartInfo.UseShellExecute = false;
                StartInfo.CreateNoWindow = true;

                Process process = Process.Start(StartInfo);
                //process.WaitForExit();

                /*process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e)
                {
                    Logger.Log(LogLevel.Info, LogCategory.Main, "{0}", e.Data);
                };

                process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
                {
                    Logger.Log(LogLevel.Info, LogCategory.Main, "{0}", e.Data);
                };

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();*/
            }
            catch (Exception Ex)
            {
                ResultMessage = "Failed to start executable with error:\n\n" + Ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// </summary>
        private bool InstallStep(string LocalFolder, ref string ResultMessage, BuildInstallStep Step)
        {
            string ExePath = BuildSettings.ExpandArguments(Step.Executable, Variables);
            if (!Path.IsPathRooted(ExePath))
            {
                ExePath = Path.Combine(LocalFolder, ExePath);
            }

            string WorkingDir = BuildSettings.ExpandArguments(Step.WorkingDirectory, Variables);
            if (WorkingDir.Length == 0)
            {
                WorkingDir = Path.GetDirectoryName(ExePath);
            }
            else
            {
                if (!Path.IsPathRooted(WorkingDir))
                {
                    WorkingDir = Path.Combine(LocalFolder, WorkingDir);
                }
            }

#if SHIPPING
            if (!File.Exists(ExePath))
            {
                ResultMessage = "Could not find executable, expected to be located at: " + ExePath;
                return false;
            }

            if (!Directory.Exists(WorkingDir))
            {
                ResultMessage = "Could not find working directory, expected at: " + WorkingDir;
                return false;
            }
#endif

            string Arguments = BuildSettings.ExpandArguments(Step.Arguments, Variables);

            Logger.Log(LogLevel.Info, LogCategory.Main, "Executing: {0} {1}", ExePath, Arguments);

            try
            {
                ProcessStartInfo StartInfo = new ProcessStartInfo();
                StartInfo.FileName = ExePath;
                StartInfo.WorkingDirectory = WorkingDir;
                StartInfo.Arguments = Arguments;
                StartInfo.RedirectStandardOutput = true;
                StartInfo.RedirectStandardError = true;
                StartInfo.UseShellExecute = false;
                StartInfo.CreateNoWindow = true;

                Process process = Process.Start(StartInfo);

                process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) { Logger.Log(LogLevel.Info, LogCategory.Main, "{0}", e.Data); };

                process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) { Logger.Log(LogLevel.Info, LogCategory.Main, "{0}", e.Data); };

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                process.WaitForExit();

                if (process.ExitCode != 0 && !Step.IgnoreErrors)
                {
                    ResultMessage = "Install process exited with error code " + process.ExitCode;
                    return false;
                }
            }
            catch (Exception Ex)
            {
                ResultMessage = "Install process exited with error: " + Ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessOutputRecieved(object sender, DataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class BuildSettings : SettingsBase
    {
        public List<BuildLaunchMode> Modes { get; set; } = new List<BuildLaunchMode>();

        /// <summary>
        ///     Evaluates all conditions and returns a list of launch modes that are valid for execution.
        /// </summary>
        /// <returns>List of launch modes valid for execution.</returns>
        public List<BuildLaunchMode> Compile()
        {
            List<BuildLaunchMode> Result = new List<BuildLaunchMode>();

            foreach (BuildLaunchMode Mode in Modes)
            {
                if (Evaluate(Mode.Condition, null))
                {
                    BuildLaunchMode NewMode = Mode.DeepClone();

                    foreach (BuildLaunchVariable Var in NewMode.Variables)
                    {
                        Var.ConditionResult = Evaluate(Var.Condition, null);
                    }

                    Result.Add(NewMode);
                }
            }

            return Result;
        }

        /// <summary>
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Variables"></param>
        /// <returns></returns>
        public static string ExpandArguments(string Value, List<BuildLaunchVariable> Variables)
        {
            Dictionary<string, string> Replacements = new Dictionary<string, string>();

            if (Variables != null)
            {
                foreach (BuildLaunchVariable Var in Variables)
                {
                    Replacements.Add(Var.Name, Var.Value);
                }
            }

            foreach (DictionaryEntry Var in Environment.GetEnvironmentVariables())
            {
                Replacements.Add((string) Var.Key, (string) Var.Value);
            }

            string Result = Value;

            int ReplacementsPerformed = 0;
            do
            {
                ReplacementsPerformed = 0;

                int StartIndex = 0;
                while (true)
                {
                    StartIndex = Result.IndexOf('%', StartIndex);
                    if (StartIndex < 0)
                    {
                        break;
                    }

                    int EndIndex = Result.IndexOf('%', StartIndex + 1);
                    if (EndIndex < 0)
                    {
                        break;
                    }

                    string Name = Result.Substring(StartIndex + 1, EndIndex - StartIndex - 1);
                    if (Replacements.ContainsKey(Name))
                    {
                        Result = Result.Substring(0, StartIndex) + Replacements[Name] + Result.Substring(EndIndex + 1);
                        ReplacementsPerformed++;
                    }
                    else
                    {
                        StartIndex = EndIndex + 1;
                    }
                }
            } while (ReplacementsPerformed > 0);

            return Result;
        }

        /// <summary>
        /// </summary>
        public static void Init()
        {
            //BuildSettings.WriteDummy(@"F:\buildsync\Docs\Example Launch Configs\buildsync.json");

            // This hack preps the script compiler so we don't have to pay any JIT
            // costs when we come to evaluate actual conditions.
            Task.Run(() => { Evaluate("false", null); });
        }

        /// <summary>
        /// </summary>
        /// <param name="Path"></param>
        public static void WriteDummy(string Path)
        {
            BuildSettings Settings = new BuildSettings();

            BuildLaunchMode Mode = new BuildLaunchMode();
            Mode.Name = "Launch Game";
            Mode.Executable = @"Data\game.exe";
            Mode.WorkingDirectory = @"Data";
            Mode.Condition = "Environment.GetEnvironmentVariable(\"PATH\") != null && Directory.Exists(\"C:\\\\\")";
            Mode.Variables.Add(new BuildLaunchVariable {Name = "HIDDEN_INT_TEST", FriendlyName = "Hidden Int Test", FriendlyDescription = "Should not show, condition should evaluate to false.", DataType = BuildLaunchVariableDataType.Int, Condition = "false", Value = "0", MinValue = 0, MaxValue = 100});
            Mode.Variables.Add(new BuildLaunchVariable {Name = "INT_TEST", FriendlyName = "Int Test", FriendlyDescription = "Test of integer variables.", DataType = BuildLaunchVariableDataType.Int, Condition = "true", Value = "0", MinValue = 0, MaxValue = 100});
            Mode.Variables.Add(new BuildLaunchVariable {Name = "BOOL_TEST", FriendlyName = "Bool Test", FriendlyDescription = "Test of bool variables.", DataType = BuildLaunchVariableDataType.Bool, Condition = "true", Value = "true"});
            Mode.Variables.Add(new BuildLaunchVariable {Name = "STRING_OPTIONS_TEST", FriendlyName = "String Options Test", FriendlyDescription = "Test of string options variables.", DataType = BuildLaunchVariableDataType.String, Condition = "true", Value = "main", Options = new List<string> {"main", "debug", "something"}});
            Mode.Variables.Add(new BuildLaunchVariable {Name = "STRING_TEST", FriendlyName = "String Test", FriendlyDescription = "Test of string variables.", DataType = BuildLaunchVariableDataType.String, Condition = "true", Value = "main"});
            Mode.Variables.Add(new BuildLaunchVariable {Name = "FLOAT_TEST", FriendlyName = "Float Test", FriendlyDescription = "Test of float variables.", DataType = BuildLaunchVariableDataType.Float, Condition = "true", Value = "0.1", MinValue = 0.1f, MaxValue = 1.0f});
            Mode.Arguments.Add(new BuildLaunchArgument {Value = "-bool_test", Condition = "%BOOL_TEST%"});
            Mode.Arguments.Add(new BuildLaunchArgument {Value = "-int_test=%INT_TEST%", Condition = "%INT_TEST% > 0"});
            Mode.Arguments.Add(new BuildLaunchArgument {Value = "-float_test=%FLOAT_TEST%", Condition = "%FLOAT_TEST% > 0.1"});
            Mode.Arguments.Add(new BuildLaunchArgument {Value = "-string_test=\"%STRING_TEST%\"", Condition = "\"%STRING_TEST%\" != \"\""});
            Mode.Arguments.Add(new BuildLaunchArgument {Value = "-string_options_test=%STRING_OPTIONS_TEST%", Condition = "\"%STRING_OPTIONS_TEST%\" != \"\""});

            BuildInstallStep Step = new BuildInstallStep();
            Step.Executable = "exepath";
            Step.Arguments = "args";
            Step.WorkingDirectory = "workingdir";
            Mode.InstallSteps.Add(Step);

            Settings.Modes.Add(Mode);

            Stopwatch watch = new Stopwatch();
            watch.Start();

            List<BuildLaunchMode> CompiledModes = Settings.Compile();

            watch.Stop();
            Logger.Log(LogLevel.Info, LogCategory.Main, "Took {0}ms to compile", watch.ElapsedMilliseconds);

            foreach (BuildLaunchMode CompiledMode in CompiledModes)
            {
                watch.Restart();

                string Args = CompiledMode.CompileArguments();
                Logger.Log(LogLevel.Info, LogCategory.Main, "Arguments: {0}", Args);

                watch.Stop();
                Logger.Log(LogLevel.Info, LogCategory.Main, "Took {0}ms to compile arguments", watch.ElapsedMilliseconds);
            }

            Settings.Save(Path);
        }

        /// <summary>
        /// </summary>
        /// <param name="Condition"></param>
        /// <param name="Variables"></param>
        /// <returns></returns>
        internal static bool Evaluate(string Condition, List<BuildLaunchVariable> Variables)
        {
            string EscapedCondition = ExpandArguments(Condition, Variables);

            try
            {
                ScriptOptions Options = ScriptOptions.Default
                    .WithImports("System", "System.IO", "System.Math")
                    .WithAllowUnsafe(false)
                    .WithOptimizationLevel(OptimizationLevel.Debug);

                Task<object> EvaluateTask = CSharpScript.EvaluateAsync(EscapedCondition, Options);
                EvaluateTask.Wait();

                if (EvaluateTask.Result is bool)
                {
                    return (bool) EvaluateTask.Result;
                }

                throw new InvalidOperationException("Result of condition was not boolean '" + EscapedCondition + "'");
            }
            catch (CompilationErrorException e)
            {
                string Errors = string.Join(Environment.NewLine, e.Diagnostics);

                Console.WriteLine("Failed to evaluate build setting expession: " + EscapedCondition);
                Console.WriteLine(Errors);

                throw new InvalidOperationException("Failed to run condition '" + EscapedCondition + "', with errors:\n\n" + Errors);
            }
        }
    }
}