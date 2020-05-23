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
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BuildSync.Core.Utils;
using BuildSync.Core.Scripting;
using BuildSync.Core.Downloads;
using BuildSync.Core.Manifests;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Networking.Messages.RemoteActions;

namespace BuildSync.Core.Client
{
    /// <summary>
    /// 
    /// </summary>
    public enum RemoteActionType
    {
        Install
    }

    /// <summary>
    /// 
    /// </summary>
    public class RemoteActionClientState
    {
        public const int UpdateInterval = 5 * 1000;
        public const int MinUpdateInterval = 1 * 1000;
        public const int LastUpdateTimeout = 60 * 1000;

        public Guid Id;
        public RemoteActionType Type;

        public bool RemoteInitiated = false;
        public Dictionary<string, string> Settings = new Dictionary<string, string>();

        public CancellationTokenSource WorkTokenSource = new CancellationTokenSource();
        public Task Work = null;

        public ulong LastUpdateRecieved = TimeUtils.Ticks;
        public ulong LastUpdateSent = TimeUtils.Ticks;

        public bool Dirty = false;

        public bool Completed = false;
        public bool Failed = false;
        public string ResultMessage = "";
        public float Progress = 0.0f;
        public string ProgressText = "";
    }

    /// <summary>
    /// 
    /// </summary>
    public class RemoteActionClient
    {
        /// <summary>
        /// 
        /// </summary>
        public Client Client = null;

        /// <summary>
        /// 
        /// </summary>
        private List<RemoteActionClientState> States = new List<RemoteActionClientState>();

        /// <summary>
        /// 
        /// </summary>
        private ManifestDownloadManager DownloadManager = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Client"></param>
        public RemoteActionClient(Client InClient, ManifestDownloadManager InDownloadManager)
        {
            DownloadManager = InDownloadManager;
            Client = InClient;
            Client.OnRequestRemoteActionRecieved += RequestRecieved;
            Client.OnCancelRemoteActionRecieved += CancelRecieved;
            Client.OnSolicitRemoteActionRecieved += SolicitRemoteActionRecieved;
            Client.OnRemoteActionProgressRecieved += (NetMessage_RemoteActionProgress Msg) =>
            {
                RemoteActionClientState Action = GetActionState(Msg.ActionId);
                if (Action != null)
                {
                    Action.Completed = Msg.Completed;
                    Action.Failed = Msg.Failed;
                    Action.ResultMessage = Msg.ResultMessage;
                    Action.Progress = Msg.Progress;
                    Action.ProgressText = Msg.ProgressText;
                    Action.LastUpdateRecieved = TimeUtils.Ticks;
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Msg"></param>
        private void RequestRecieved(NetMessage_RequestRemoteAction Msg)
        {
            Logger.Log(LogLevel.Info, LogCategory.Main, "Server is requesting us to start a remote action '{0}'.", Msg.Id.ToString());

            RemoteActionClientState State = AllocateState(Msg.Type);
            State.Id = Msg.ActionId;
            State.RemoteInitiated = true;
            State.Settings = Msg.Settings;
            State.Work = Task.Run(() => { RunAction(State, State.WorkTokenSource.Token); }, State.WorkTokenSource.Token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Action"></param>
        private void RunAction(RemoteActionClientState Action, CancellationToken CancelToken)
        {
            // TODO: we should figure out how to handle cancellation for installs, not sure the safest way to do this though hum.

            switch (Action.Type)
            {
                case RemoteActionType.Install:
                    {
                        Guid ManifestId = Guid.Parse(Action.Settings["ManifestId"]);
                        string DeviceName = Action.Settings["DeviceName"];
                        string InstallLocation = Action.Settings["InstallLocation"];

                        ScriptBuildProgressDelegate Callback = (string InState, float InProgress) =>
                        {
                            if (Action.Progress != InProgress || Action.ProgressText != InState)
                            {
                                Action.Progress = InProgress;
                                Action.ProgressText = InState;
                                Action.Dirty = true;
                            }
                        };

                        DownloadManager.BlockDownload(ManifestId);
                        try
                        {
                            if (DownloadManager.PerformInstallation(ManifestId, DeviceName, InstallLocation, Callback))
                            {
                                Action.Failed = false;
                                Action.Completed = true;
                                Action.Dirty = true;
                            }
                            else
                            {
                                Action.ResultMessage = "Failed with generic error.";
                                Action.Failed = true;
                                Action.Completed = true;
                                Action.Dirty = true;
                            }
                        }
                        catch (Exception Ex)
                        {
                            Logger.Log(LogLevel.Error, LogCategory.Manifest, "Failed to install with error: {0}", Ex.Message);

                            Action.ResultMessage = Ex.Message;
                            Action.Failed = true;
                            Action.Completed = true;
                            Action.Dirty = true;
                        }
                        finally
                        {
                            DownloadManager.UnblockDownload(ManifestId);
                        }

                        break;
                    }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="ActionId"></param>
        private void CancelRecieved(Guid ActionId)
        {
            RemoteActionClientState State = GetActionState(ActionId);
            if (State == null)
            {
                return;
            }

            Logger.Log(LogLevel.Info, LogCategory.Main, "Server requested that we cancel remote action '{0}'.", State.Id.ToString());

            State.WorkTokenSource.Cancel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Msg"></param>
        private void SolicitRemoteActionRecieved(NetMessage_SolicitRemoteAction Msg)
        {
            bool Accepted = false;

            // Don't accept if we are running any other actions.
            if (!IsRunningRemoteActions())
            {
                if (Msg.Type == RemoteActionType.Install)
                {
                    Guid ManifestId = Guid.Parse(Msg.Settings["ManifestId"]);

                    ManifestDownloadState State = DownloadManager.GetDownload(ManifestId);
                    if (State != null && State.State == ManifestDownloadProgressState.Complete)
                    {
                        try
                        {
                            // TODO: We don't support json files here. Should we just remove them? Nobody uses them.

                            string ConfigFilePath = Path.Combine(State.LocalFolder, "buildsync.cs");

                            BuildSettings Settings = new BuildSettings();
                            Settings.ScriptSource = File.ReadAllText(ConfigFilePath);

                            List<BuildLaunchMode> Modes;
                            Modes = Settings.Compile();

                            Accepted = (Modes.Count > 0);
                        }
                        catch (Exception Ex)
                        {
                            // We cannot compile or use this script :(
                        }
                    }
                }
            }

            if (Accepted)
            {
                NetMessage_SolicitAcceptRemoteAction Reply = new NetMessage_SolicitAcceptRemoteAction();
                Reply.ActionId = Msg.ActionId;
                Client.Connection.Send(Reply);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void Poll()
        {
            for (int i = 0; i < States.Count; i++)
            {
                RemoteActionClientState State = States[i];

                // Something we are running locally.
                if (State.RemoteInitiated)
                {
                    // Handled by task runner.
                }

                // Something a remote peer is running.
                else
                {
                    // We lost connection, aborted.
                    if (!Client.IsConnected)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Main, "Removed remote action '{0}' as lost server connection.", State.Id.ToString());

                        State.ResultMessage = "Lost connection to server.";
                        State.Completed = true;
                        State.Failed = true;
                        State.Dirty = true;
                    }

                    // Timed out waiting for progress update from server.
                    else if (TimeUtils.Ticks - State.LastUpdateRecieved > RemoteActionClientState.LastUpdateTimeout)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Main, "Removed remote action '{0}' as timed out.", State.Id.ToString());

                        State.ResultMessage = "Timed out waiting for update from server.";
                        State.Completed = true;
                        State.Failed = true;
                        State.Dirty = true;
                    }
                }

                // Send progress update to the server if we are running it.
                if (TimeUtils.Ticks - State.LastUpdateSent > RemoteActionClientState.UpdateInterval || State.Dirty)
                {
                    // Send update to server if required.
                    if (Client.IsConnected && State.RemoteInitiated)
                    {
                        if (State.Completed || TimeUtils.Ticks - State.LastUpdateSent > RemoteActionClientState.MinUpdateInterval)
                        {
                            NetMessage_RemoteActionProgress Msg = new NetMessage_RemoteActionProgress();
                            Msg.ActionId = State.Id;
                            Msg.Completed = State.Completed;
                            Msg.Failed = State.Failed;
                            Msg.ResultMessage = State.ResultMessage;
                            Msg.Progress = State.Progress;
                            Msg.ProgressText = State.ProgressText;
                            Client.Connection.Send(Msg);

                            State.LastUpdateSent = TimeUtils.Ticks;
                        }
                    }

                    State.Dirty = false;

                    // If completed (and remote initiated), clean up state.
                    // If local initiated, the code starting the action is responsible for cleaning it up.
                    if (State.Completed && State.RemoteInitiated)
                    {
                        States.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RemoteActionClientState AllocateState(RemoteActionType Type)
        {
            RemoteActionClientState State = new RemoteActionClientState();
            State.Id = Guid.NewGuid();
            State.Type = Type;
            States.Add(State);

            Logger.Log(LogLevel.Info, LogCategory.Main, "Allocated new remote action '{0}' of type {1}.", State.Id.ToString(), State.Type.ToString());

            return State;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public RemoteActionClientState GetActionState(Guid Id)
        {
            foreach (RemoteActionClientState State in States)
            {
                if (State.Id == Id)
                {
                    return State;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public void RemoveActionState(Guid Id)
        {
            foreach (RemoteActionClientState State in States)
            {
                if (State.Id == Id)
                {
                    States.Remove(State);
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool IsRunningRemoteActions()
        {
            foreach (RemoteActionClientState State in States)
            {
                if (State.RemoteInitiated)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ManifestId"></param>
        /// <param name="DeviceName"></param>
        /// <param name="DeviceLocation"></param>
        /// <returns></returns>
        public Guid RequestRemoteInstall(Guid ManifestId, string DeviceName, string InstallLocation)
        {
            if (!Client.Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Info, LogCategory.Main, "Failed to start remote install. No connection.");
                return Guid.Empty;
            }

            RemoteActionClientState State = AllocateState(RemoteActionType.Install);

            NetMessage_RequestRemoteAction Msg = new NetMessage_RequestRemoteAction();
            Msg.ActionId = State.Id;
            Msg.Type = RemoteActionType.Install;
            Msg.Settings["ManifestId"] = ManifestId.ToString();
            Msg.Settings["DeviceName"] = DeviceName;
            Msg.Settings["InstallLocation"] = InstallLocation;
            Client.Connection.Send(Msg);

            return State.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ActionId"></param>
        public void CancelRemoteAction(Guid ActionId)
        {
            if (!Client.Connection.IsReadyForData)
            {
                Logger.Log(LogLevel.Info, LogCategory.Main, "Failed to start cancel remote action. No connection.");
                return;
            }

            RemoteActionClientState State = GetActionState(ActionId);
            if (State != null && !State.Completed)
            {
                NetMessage_CancelRemoteAction Msg = new NetMessage_CancelRemoteAction();
                Msg.ActionId = ActionId;
                Client.Connection.Send(Msg);
            }

            RemoveActionState(ActionId);
        }
    }
}
