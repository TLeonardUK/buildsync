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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Client;
using BuildSync.Core.Utils;
using BuildSync.Core.Networking;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Networking.Messages.RemoteActions;

namespace BuildSync.Core.Server
{ 
    /// <summary>
    /// 
    /// </summary>
    public class RemoteActionServerState
    {
        public const int UpdateInterval = 10 * 1000;
        public const int MinUpdateInterval = 1 * 1000;
        public const int LastUpdateTimeout = 60 * 1000;
        public const int SolicitTimeout = (24 * 60 * 60 * 1000); // 24 hours?
        public const int SolicitInterval = 10 * 1000;
        public const int MinSolicitWait = 2 * 1000;

        public Guid Id;
        public RemoteActionType Type;
        public Dictionary<string, string> Settings;

        public NetConnection ForClient;
        public NetConnection AllocatedClient;

        public List<NetConnection> SolicitationReplies = new List<NetConnection>();

        public ulong LastUpdateRecieved = TimeUtils.Ticks;
        public ulong LastUpdateSent = TimeUtils.Ticks;

        public ulong RecievedTime = TimeUtils.Ticks;
        public ulong LastSolicitBroadcast = 0;

        public bool Cancelled = false;

        public bool Completed = false;
        public bool Failed = false;
        public string ResultMessage = "";
        public float Progress = 0.0f;
        public string ProgressText = "";

        public bool Dirty = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public class RemoteActionServer
    {
        /// <summary>
        /// 
        /// </summary>
        public Server Server = null;

        /// <summary>
        /// 
        /// </summary>
        public List<RemoteActionServerState> States { get; internal set; } = new List<RemoteActionServerState>();
        private Random SolicitSelectionRandom = new Random(Environment.TickCount);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Client"></param>
        public RemoteActionServer(Server InServer)
        {
            Server = InServer;
            Server.OnRequestRemoteActionRecieved += RequestRecieved;
            Server.OnCancelRemoteActionRecieved += CancelRecieved;
            Server.OnSolicitAcceptRemoteActionRecieved += SolicitAcceptRecieved;
            Server.OnRemoteActionProgressRecieved += (NetMessage_RemoteActionProgress Msg) =>
            {
                RemoteActionServerState Action = GetActionState(Msg.ActionId);
                if (Action != null)
                {
                    if (Action.Completed != Msg.Completed ||
                        Action.Failed != Msg.Failed ||
                        Action.ResultMessage != Msg.ResultMessage ||
                        Action.Progress != Msg.Progress ||
                        Action.ProgressText != Msg.ProgressText)
                    {
                        Action.Completed = Msg.Completed;
                        Action.Failed = Msg.Failed;
                        Action.ResultMessage = Msg.ResultMessage;
                        Action.Progress = Msg.Progress;
                        Action.ProgressText = Msg.ProgressText;
                        Action.Dirty = true;
                    }
                    Action.LastUpdateRecieved = TimeUtils.Ticks;
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="ActionId"></param>
        private void SolicitAcceptRecieved(NetConnection Client, Guid ActionId)
        {
            RemoteActionServerState State = GetActionState(ActionId);
            if (State == null)
            {
                return;
            }

            State.SolicitationReplies.Add(Client);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="ActionId"></param>
        private void CancelRecieved(NetConnection Client, Guid ActionId)
        {
            RemoteActionServerState State = GetActionState(ActionId);
            if (State == null)
            {
                return;
            }

            State.Cancelled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Msg"></param>
        private void RequestRecieved(NetConnection Client, NetMessage_RequestRemoteAction Msg)
        {
            RemoteActionServerState State = AllocateState(Msg.ActionId, Msg.Type);
            State.Settings = Msg.Settings;
            State.ForClient = Client;
            State.Progress = 0.0f;
            State.ProgressText = "Waiting for available client ...";
            State.Dirty = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void Poll()
        {
            for (int i = 0; i < States.Count; i++)
            {
                RemoteActionServerState State = States[i];

                // If this action has been allocated a client, wait for result.
                if (State.AllocatedClient != null)
                {
                    // Client has not updated us in quite a while :(
                    if (TimeUtils.Ticks - State.LastUpdateRecieved > RemoteActionServerState.LastUpdateTimeout)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Main, "Remote action '{0}' failed as no response has been heard from allocated client.", State.Id.ToString());

                        State.ResultMessage = "Remote client failed to send update in time.";
                        State.Completed = true;
                        State.Failed = true;
                        State.Dirty = true;
                    }

                    // Client who was allocated this action has disconnected, so abort.
                    else if (!State.AllocatedClient.IsConnected)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Main, "Remote action '{0}' failed as allocated client disconnected.", State.Id.ToString());

                        State.ResultMessage = "Remote client unexpected disconnected.";
                        State.Completed = true;
                        State.Failed = true;
                        State.Dirty = true;
                    }
                }

                // Otherwise send out a solicitation to perform action.
                else
                {
                    // Client who requested this action has disconnected, so abort.
                    if (!State.ForClient.IsConnected)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Main, "Remote action '{0}' failed as requesting client disconnected.", State.Id.ToString());

                        State.ResultMessage = "Requesting client unexpected disconnected.";
                        State.Completed = true;
                        State.Failed = true;
                        State.Dirty = true;
                    }

                    // Client has cancelled this state.
                    else if (State.Cancelled)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Main, "Remote action '{0}' failed as client cancelled it.", State.Id.ToString());

                        State.ResultMessage = "Cancelled by user.";
                        State.Completed = true;
                        State.Failed = true;
                        State.Dirty = true;

                        // Tell client allocated the job to cancel.
                        if (State.AllocatedClient != null && State.AllocatedClient.IsConnected)
                        {
                            NetMessage_CancelRemoteAction Msg = new NetMessage_CancelRemoteAction();
                            Msg.ActionId = State.Id;
                            State.AllocatedClient.Send(Msg);
                        }
                    }

                    // Send new solicitation.
                    else if (TimeUtils.Ticks - State.LastSolicitBroadcast > RemoteActionServerState.SolicitInterval)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Main, "Remote action '{0}' sending new solicitation request.", State.Id.ToString());

                        List<NetConnection> Clients = Server.ListenConnection.AllClients;
                        foreach (NetConnection ClientConnection in Clients)
                        {
                            if (ClientConnection != State.ForClient && ClientConnection.Metadata != null)
                            {
                                ServerConnectedClient ClientState = ClientConnection.Metadata as ServerConnectedClient;
                                if (ClientState.AllowRemoteActions)
                                {
                                    NetMessage_SolicitRemoteAction Msg = new NetMessage_SolicitRemoteAction();
                                    Msg.ActionId = State.Id;
                                    Msg.Type = State.Type;
                                    Msg.Settings = State.Settings;
                                    ClientConnection.Send(Msg);
                                }
                            }
                        }

                        State.LastSolicitBroadcast = TimeUtils.Ticks;
                    }

                    // Nobody responded saying they can perform the action :|
                    else if (TimeUtils.Ticks - State.RecievedTime > RemoteActionServerState.SolicitTimeout)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Main, "Remote action '{0}' failed as nobody responded to solicitation.", State.Id.ToString());

                        State.ResultMessage = "No remote clients accepted request.";
                        State.Completed = true;
                        State.Failed = true;
                        State.Dirty = true;
                    }

                    // We have a client who can do the job!
                    else if (State.SolicitationReplies.Count > 0 && TimeUtils.Ticks - State.LastSolicitBroadcast > RemoteActionServerState.MinSolicitWait)
                    {
                        Logger.Log(LogLevel.Info, LogCategory.Main, "Remote action '{0}' recieved solicitation reply and is starting.", State.Id.ToString());

                        State.AllocatedClient = State.SolicitationReplies[SolicitSelectionRandom.Next(0, State.SolicitationReplies.Count - 1)];
                        State.SolicitationReplies.Clear();
                        State.LastUpdateRecieved = TimeUtils.Ticks;

                        NetMessage_RequestRemoteAction Msg = new NetMessage_RequestRemoteAction();
                        Msg.ActionId = State.Id;
                        Msg.Type = State.Type;
                        Msg.Settings = State.Settings;
                        State.AllocatedClient.Send(Msg);
                    }
                }

                // Send progress update to the client.
                if (TimeUtils.Ticks - State.LastUpdateSent > RemoteActionServerState.UpdateInterval || State.Dirty)
                {
                    if (State.ForClient.IsConnected)
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
                            State.ForClient.Send(Msg);

                            State.LastUpdateSent = TimeUtils.Ticks;
                        }
                    }

                    State.Dirty = false;

                    // If completed, clean up state.
                    if (State.Completed)
                    {
                        RemoveActionState(State.Id);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RemoteActionServerState AllocateState(Guid Id, RemoteActionType Type)
        {
            RemoteActionServerState State = new RemoteActionServerState();
            State.Id = Id;
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
        public RemoteActionServerState GetActionState(Guid Id)
        {
            foreach (RemoteActionServerState State in States)
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
            foreach (RemoteActionServerState State in States)
            {
                if (State.Id == Id)
                {
                    States.Remove(State);
                    return;
                }
            }
        }
    }
}
