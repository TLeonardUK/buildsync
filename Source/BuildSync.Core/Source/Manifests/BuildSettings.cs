using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BuildSync.Core.Utils;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace BuildSync.Core.Manifests
{
    [Serializable, JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BuildLaunchVariableDataType
    {
        String,
        Int,
        Float,
        Bool
    }

    [Serializable]
    public class BuildLaunchVariable
    {
        public string Name { get; set; } = "";
        public BuildLaunchVariableDataType DataType { get; set; } = BuildLaunchVariableDataType.String;
        public string FriendlyName { get; set; } = "";
        public string FriendlyDescription { get; set; } = "";
        public string FriendlyCategory { get; set; } = "Launch Settings";
        public string Condition { get; set; } = "true";
        public string Value { get; set; } = "";
        public List<string> Options { get; set; } = new List<string>();
        public float MinValue { get; set; } = -1000;
        public float MaxValue { get; set; } =  1000;

        public bool ConditionResult = false;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BuildLaunchVariable ShallowClone()
        {
            return (BuildLaunchVariable)this.MemberwiseClone();
        }
    }

    [Serializable]
    public class BuildLaunchArgument
    {
        public string Value { get; set; } = "";
        public string Condition { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BuildLaunchArgument ShallowClone()
        {
            return (BuildLaunchArgument)this.MemberwiseClone();
        }
    }

    [Serializable]
    public class BuildLaunchMode
    {
        public string Name { get; set; } = "";
        public string Executable { get; set; } = "";
        public string WorkingDirectory { get; set; } = "";
        public string Condition { get; set; } = "";

        public List<BuildLaunchVariable> Variables { get; set; } = new List<BuildLaunchVariable>();
        public List<BuildLaunchArgument> Arguments { get; set; } = new List<BuildLaunchArgument>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BuildLaunchMode DeepClone()
        {
            BuildLaunchMode other = (BuildLaunchMode)this.MemberwiseClone();
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
            return other;
        }

        /// <summary>
        /// 
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
    }

    [Serializable]
    public class BuildSettings : SettingsBase
    {
        public List<BuildLaunchMode> Modes { get; set; } = new List<BuildLaunchMode>();

        /// <summary>
        /// 
        /// </summary>
        public static void Init()
        {
            //BuildSettings.WriteDummy(@"C:\Personal\buildsync\Docs\Example Launch Configs\buildsync.json");

            // This hack preps the script compiler so we don't have to pay any JIT
            // costs when we come to evaluate actual conditions.
            Task.Run(() => { Evaluate("false", null); });
        }

        /// <summary>
        /// 
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
            Mode.Variables.Add(new BuildLaunchVariable { Name ="HIDDEN_INT_TEST", FriendlyName="Hidden Int Test", FriendlyDescription="Should not show, condition should evaluate to false.", DataType=BuildLaunchVariableDataType.Int, Condition="false", Value="0", MinValue=0, MaxValue=100 });
            Mode.Variables.Add(new BuildLaunchVariable { Name ="INT_TEST", FriendlyName="Int Test", FriendlyDescription="Test of integer variables.", DataType=BuildLaunchVariableDataType.Int, Condition="true", Value="0", MinValue=0, MaxValue=100 });
            Mode.Variables.Add(new BuildLaunchVariable { Name = "BOOL_TEST", FriendlyName = "Bool Test", FriendlyDescription = "Test of bool variables.", DataType = BuildLaunchVariableDataType.Bool, Condition = "true", Value = "true" });
            Mode.Variables.Add(new BuildLaunchVariable { Name = "STRING_OPTIONS_TEST", FriendlyName = "String Options Test", FriendlyDescription = "Test of string options variables.", DataType = BuildLaunchVariableDataType.String, Condition ="true", Value="main", Options=new List<string> {"main", "debug", "something"} });
            Mode.Variables.Add(new BuildLaunchVariable { Name = "STRING_TEST", FriendlyName = "String Test", FriendlyDescription = "Test of string variables.", DataType = BuildLaunchVariableDataType.String, Condition = "true", Value = "main" });
            Mode.Variables.Add(new BuildLaunchVariable { Name = "FLOAT_TEST", FriendlyName = "Float Test", FriendlyDescription = "Test of float variables.", DataType = BuildLaunchVariableDataType.Float, Condition = "true", Value = "0.1", MinValue = 0.1f, MaxValue = 1.0f });
            Mode.Arguments.Add(new BuildLaunchArgument { Value= "-bool_test", Condition="%BOOL_TEST%" });
            Mode.Arguments.Add(new BuildLaunchArgument { Value= "-int_test=%INT_TEST%", Condition="%INT_TEST% > 0" });
            Mode.Arguments.Add(new BuildLaunchArgument { Value= "-float_test=%FLOAT_TEST%", Condition="%FLOAT_TEST% > 0.1" });
            Mode.Arguments.Add(new BuildLaunchArgument { Value= "-string_test=\"%STRING_TEST%\"", Condition="\"%STRING_TEST%\" != \"\"" });
            Mode.Arguments.Add(new BuildLaunchArgument { Value= "-string_options_test=%STRING_OPTIONS_TEST%", Condition="\"%STRING_OPTIONS_TEST%\" != \"\"" });
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
        /// 
        /// </summary>
        /// <param name="Condition"></param>
        /// <param name="Variables"></param>
        /// <returns></returns>
        internal static bool Evaluate(string Condition, List<BuildLaunchVariable> Variables)
        {
            string EscapedCondition = BuildSettings.ExpandArguments(Condition, Variables);

            try
            {
                ScriptOptions Options = ScriptOptions.Default
                    .WithImports("System", "System.IO", "System.Math")
                    .WithAllowUnsafe(false)
                    .WithOptimizationLevel(Microsoft.CodeAnalysis.OptimizationLevel.Debug);

                Task<object> EvaluateTask = CSharpScript.EvaluateAsync(EscapedCondition, Options);
                EvaluateTask.Wait();

                if (EvaluateTask.Result is bool)
                {
                    return (bool)EvaluateTask.Result;
                }
                else
                {
                    throw new InvalidOperationException("Result of condition was not boolean '" + EscapedCondition + "'");
                }
            }
            catch (CompilationErrorException e)
            {
                string Errors = string.Join(Environment.NewLine, e.Diagnostics);

                Console.WriteLine("Failed to evaluate build setting expession: " + EscapedCondition);
                Console.WriteLine(Errors);

                throw new InvalidOperationException("Failed to run condition '" + EscapedCondition + "', with errors:\n\n" + Errors);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Variables"></param>
        /// <returns></returns>
        internal static string ExpandArguments(string Value, List<BuildLaunchVariable> Variables)
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
                Replacements.Add((string)Var.Key, (string)Var.Value);
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

                    string Name = Result.Substring(StartIndex + 1, (EndIndex - StartIndex) - 1);
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
            } 
            while (ReplacementsPerformed > 0);

            return Result;
        }

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
    }
}
