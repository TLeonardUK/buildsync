using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using CommandLine;
using BuildSync.Server.Tasks;
using BuildSync.Core.Utils;

namespace BuildSync.Server.Commands
{
    [Verb("configure", HelpText = "Sets the value of a setting configuring the behaviour of this tool.")]
    public class CommandLineConfigureOptions
    {
        [Value(0, MetaName = "Name", Required = true, HelpText = "Name of setting to modify.")]
        public string Name { get; set; }

        [Value(1, MetaName = "Value", Required = true, HelpText = "New value of setting to modify.")]
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal void Run(CommandIPC IpcClient)
        {
            PropertyInfo Property = null;
            foreach (PropertyInfo Potential in Program.Settings.GetType().GetProperties())
            {
                if (Potential.Name.ToLower() == Name.ToLower())
                {
                    Property = Potential;
                    break;
                }
            }

            if (Property == null)
            {
                Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Setting '{0}' does not exist.", Name);
                return;
            }

            // Calcualte correct value to set property to.
            object ValueToSet = null;
            if (Property.PropertyType == typeof(string))
            {
                ValueToSet = Value;
            }
            else if (Property.PropertyType == typeof(int))
            {
                int Result = 0;
                if (!int.TryParse(Value, out Result))
                {
                    Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Value '{0}' is not a valid int.", Value);
                    return;
                }
                ValueToSet = Result;
            }
            else if (Property.PropertyType == typeof(float))
            {
                float Result = 0;
                if (!float.TryParse(Value, out Result))
                {
                    Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Value '{0}' is not a valid float.", Value);
                    return;
                }
                ValueToSet = Result;
            }
            else if (Property.PropertyType == typeof(bool))
            {
                bool Result = false;
                if (Value == "1")
                {
                    Result = true;
                }
                else if (Value == "0")
                {
                    Result = false;
                }
                else
                {
                    if (!bool.TryParse(Value, out Result))
                    {
                        Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Value '{0}' is not a valid bool.", Value);
                        return;
                    }
                }
                ValueToSet = Result;
            }
            else if (Property.PropertyType == typeof(bool))
            {
                bool Result = false;
                if (Value == "1")
                {
                    Result = true;
                }
                else if (Value == "0")
                {
                    Result = false;
                }
                else
                {
                    if (!bool.TryParse(Value, out Result))
                    {
                        Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Value '{0}' is not a valid bool.", Value);
                        return;
                    }
                }
                ValueToSet = Result;
            }
            else
            {
                Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Setting '{0}' cannot be set externally.", Name);
                return;
            }

            // Check if we don't need to modify the setting.
            if (Property.GetValue(Program.Settings).ToString() == ValueToSet.ToString())
            {
                Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Setting '{0}' already set to given value.", Name);
                return;
            }

            // Special handling for storage path changing, we need to run some code to clean things up.
            if (Name.ToLower() == "storagepath")
            {
                MoveStorageTask MoveTask = new MoveStorageTask();
                MoveTask.Start(Program.Settings.StoragePath, Value);

                MoveStorageState OldState = MoveStorageState.Unknown;
                string OldFile = "";
                while (MoveTask.State != MoveStorageState.Success)
                {
                    if (OldState != MoveTask.State || OldFile != MoveTask.CurrentFile)
                    {
                        switch (MoveTask.State)
                        {
                            case MoveStorageState.CopyingFiles:
                                {
                                    Logger.Log(LogLevel.Display, LogCategory.Main, "Copying: {0}", MoveTask.CurrentFile);
                                    break;
                                }
                            case MoveStorageState.CleaningUpOldDirectory:
                                {
                                    Logger.Log(LogLevel.Display, LogCategory.Main, "Cleaning up old directory.");
                                    break;
                                }
                            case MoveStorageState.Success:
                                {
                                    break;
                                }
                            case MoveStorageState.FailedDiskError:
                            case MoveStorageState.Failed:
                            default:
                                {
                                    Logger.Log(LogLevel.Display, LogCategory.Main, "FAILED: Failed to change storage directory due to disk error.");
                                    return;
                                }
                        }

                        OldFile = MoveTask.CurrentFile;
                        OldState = MoveTask.State;
                    }

                    Thread.Sleep(10);
                }
            }

            // Update the value.
            Property.SetValue(Program.Settings, ValueToSet);
            Program.SaveSettings();
            Logger.Log(LogLevel.Display, LogCategory.Main, "SUCCESS: Setting '{0}' was set to '{1}'.", Name, Value);
        }
    }
}
