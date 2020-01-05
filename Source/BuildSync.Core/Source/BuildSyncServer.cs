using System;
using System.Net;
using System.Collections.Generic;
using BuildSync.Core.Utils;
using BuildSync.Core.Networking;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Manifests;
using BuildSync.Core.Downloads;
using BuildSync.Core.Utils;
using BuildSync.Core.Users;

namespace BuildSync.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class BuildSyncServer
    {
        /// <summary>
        /// 
        /// </summary>
        public class ClientState
        {
            public string Username = "";

            public BlockListState BlockState = null;
            public List<IPEndPoint> RelevantPeerAddresses = new List<IPEndPoint>();
            public bool RelevantPeerAddressesNeedUpdate = false;

            public bool PermissionsNeedUpdate = false;

            public IPEndPoint PeerConnectionAddress = null;
        }

        /// <summary>
        /// 
        /// </summary>
        private NetConnection ListenConnection = new NetConnection();

        /// <summary>
        /// 
        /// </summary>
        private int ServerPort = 0;

        /// <summary>
        /// 
        /// </summary>
        private ulong LastListenAttempt = 0;

        /// <summary>
        /// 
        /// </summary>
        private const int ListenAttemptInterval = 2 * 1000;

        /// <summary>
        /// 
        /// </summary>
        private bool Started = false;

        /// <summary>
        /// 
        /// </summary>
        private BuildManifestRegistry ManifestRegistry = null;

        /// <summary>
        /// 
        /// </summary>
        private UserManager UserManager = null;

        /// <summary>
        /// 
        /// </summary>
        public BuildSyncServer()
        {
            ListenConnection.OnClientMessageRecieved += HandleMessage;
            ListenConnection.OnClientConnect += ClientConnected;

            MemoryPool.PreallocateBuffers((int)BuildManifest.BlockSize, 5);
            MemoryPool.PreallocateBuffers(4 * 1024 * 1024, 8); // CRC'ing buffers.
            NetConnection.PreallocateBuffers(48);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            Started = false;
            ListenConnection.Disconnect();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Hostname"></param>
        /// <param name="Port"></param>
        public void Start(int Port, BuildManifestRegistry BuildManifest, UserManager InUserManager)
        {
            ServerPort = Port;
            Started = true;
            ManifestRegistry = BuildManifest;

            UserManager = InUserManager;
            UserManager.PermissionsUpdated += (User user) =>
            {
                List<NetConnection> Clients = ListenConnection.AllClients;
                foreach (NetConnection Connection in Clients)
                {
                    if (Connection.Metadata != null)
                    {
                        ClientState State = Connection.Metadata as ClientState;
                        if (State.Username == user.Username)
                        {
                            State.PermissionsNeedUpdate = true;
                        }
                    }
                }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void Poll()
        {
            if (Started)
            {
                // Reattempt to create listen server?
                if (!ListenConnection.IsListening)
                {
                    ulong ElapsedTime = TimeUtils.Ticks - LastListenAttempt;
                    if (ElapsedTime > ListenAttemptInterval)
                    {
                        ListenForClients();
                    }
                }
            }

            // Go through each client, send them a list of peers that has blocks they are interested in.
            List<NetConnection> Clients = ListenConnection.AllClients;
            foreach (NetConnection Connection in Clients)
            {
                if (Connection.Metadata != null)
                {
                    ClientState State = Connection.Metadata as ClientState;

                    // Send user an update of the permissions they have.
                    if (State.PermissionsNeedUpdate)
                    {
                        User user = UserManager.GetOrCreateUser(State.Username);
                        
                        NetMessage_PermissionUpdate Msg = new NetMessage_PermissionUpdate();
                        Msg.Permissions = user.Permissions;
                        Connection.Send(Msg);

                        State.PermissionsNeedUpdate = false;
                    }

                    // Send user update of all peers that may have data they are after.
                    if (State.RelevantPeerAddressesNeedUpdate)
                    {
                        List<IPEndPoint> NewPeers = GetRelevantPeerAddressesForBlockState(Clients, Connection);

                        // If the new list is different than the old one send it.
                        bool IsDifferent = (NewPeers.Count != State.RelevantPeerAddresses.Count);
                        if (!IsDifferent)
                        {
                            foreach (IPEndPoint Address in NewPeers)
                            {
                                bool Exists = false;

                                foreach (IPEndPoint PrevAddress in State.RelevantPeerAddresses)
                                {
                                    if (PrevAddress.Equals(Address))
                                    {
                                        Exists = true;
                                        break;
                                    }
                                }

                                if (!Exists)
                                {
                                    IsDifferent = true;
                                    break;
                                }
                            }
                        }

                        Logger.Log(LogLevel.Info, LogCategory.Peers, "----- Peer Relevant Addresses -----");
                        Logger.Log(LogLevel.Info, LogCategory.Peers, "Peer: {0}", State.PeerConnectionAddress == null ? "Unknown" : State.PeerConnectionAddress.ToString());
                        if (NewPeers.Count > 0)
                        {
                            for (int i = 0; i < NewPeers.Count; i++)
                            {
                                Logger.Log(LogLevel.Info, LogCategory.Peers, "[{0}] {1}", i, NewPeers[i].ToString());
                            }
                        }
                        else
                        {
                            Logger.Log(LogLevel.Info, LogCategory.Peers, "\tNo Relevant Peers");
                        }

                        // Send it!
                        if (IsDifferent)
                        {
                            NetMessage_RelevantPeerListUpdate Msg = new NetMessage_RelevantPeerListUpdate();
                            Msg.PeerAddresses = NewPeers;
                            Connection.Send(Msg);
                        }

                        State.RelevantPeerAddresses = NewPeers;
                        State.RelevantPeerAddressesNeedUpdate = false;
                    }
                }
            }

            ListenConnection.Poll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Clients"></param>
        /// <param name="ForClient"></param>
        /// <returns></returns>
        private List<IPEndPoint> GetRelevantPeerAddressesForBlockState(List<NetConnection> Clients, NetConnection ForClient)
        {
            List<IPEndPoint> Result = new List<IPEndPoint>();

            ClientState ForClientState = ForClient.Metadata as ClientState;
            if (ForClientState.BlockState == null)
            {
                return Result;
            }

            foreach (NetConnection Connection in Clients)
            {
                if (Connection == ForClient)
                {
                    continue;
                }

                if (Connection.Metadata == null)
                {
                    continue;
                }

                ClientState ClientState = Connection.Metadata as ClientState;
                if (ClientState.BlockState == null)
                {
                    continue;
                }
                if (ClientState.PeerConnectionAddress == null)
                {
                    continue;
                }

                if (ClientState.BlockState.HasAnyBlocksNeeded(ForClientState.BlockState))
                {
                    Result.Add(ClientState.PeerConnectionAddress);
                }
            }

            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ListenForClients()
        {
            ListenConnection.BeginListen(ServerPort);
            LastListenAttempt = TimeUtils.Ticks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="ClientConnection"></param>
        private void ClientConnected(NetConnection Connection, NetConnection ClientConnection)
        {
            ClientConnection.Metadata = new ClientState();
        }

        /// <summary>
        /// 
        /// </summary>
        private void PrintClientBlockStates()
        {
            List<NetConnection> Clients = ListenConnection.AllClients;

            Logger.Log(LogLevel.Info, LogCategory.Peers, "=======================");
            foreach (NetConnection Connection in Clients)
            {
                if (Connection.Metadata == null)
                {
                    continue;
                }

                ClientState ClientState = Connection.Metadata as ClientState;
                if (ClientState.BlockState == null)
                {
                    continue;
                }

                Logger.Log(LogLevel.Info, LogCategory.Peers, "Peer[{0}]", ClientState.PeerConnectionAddress == null ? "Unknown" : ClientState.PeerConnectionAddress.ToString());
                for (int i = 0; i < ClientState.BlockState.States.Length; i++)
                {
                    ManifestBlockListState State = ClientState.BlockState.States[i];
                    Logger.Log(LogLevel.Info, LogCategory.Peers, "\tManifest[{0}] Id={1} Active={2}", i, State.Id.ToString(), State.IsActive);
                    if (State.BlockState.Ranges != null)
                    {
                        for (int j = 0; j < State.BlockState.Ranges.Count; j++)
                        {
                            Logger.Log(LogLevel.Info, LogCategory.Peers, "\t\tRegion[{0}] Start={1} End={2} Active={3}", j, State.BlockState.Ranges[j].Start, State.BlockState.Ranges[j].End, State.BlockState.Ranges[j].State);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Root"></param>
        private void SendBuildsUpdate(NetConnection Connection, string RootPath)
        {
            ClientState State = Connection.Metadata as ClientState;

            List<string> Children = ManifestRegistry.GetVirtualPathChildren(RootPath);

            NetMessage_GetBuildsResponse ResponseMsg = new NetMessage_GetBuildsResponse();
            ResponseMsg.RootPath = RootPath;
            List<NetMessage_GetBuildsResponse.BuildInfo> Result = new List<NetMessage_GetBuildsResponse.BuildInfo>();

            // Folders first.
            int Index = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                BuildManifest Manifest = ManifestRegistry.GetManifestByPath(Children[i]);
                if (Manifest == null)
                {
                    if (UserManager.CheckPermission(State.Username, UserPermissionType.Access, Children[i], false, true))
                    {
                        Result.Add(new NetMessage_GetBuildsResponse.BuildInfo()
                        {
                            Guid = Guid.Empty,
                            CreateTime = DateTime.UtcNow,
                            VirtualPath = Children[i]
                        });
                    }
                }
            }

            // Builds second.
            if (UserManager.CheckPermission(State.Username, UserPermissionType.Access, RootPath))
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    BuildManifest Manifest = ManifestRegistry.GetManifestByPath(Children[i]);
                    if (Manifest != null)
                    {
                        Result.Add(new NetMessage_GetBuildsResponse.BuildInfo()
                        {
                            Guid = Manifest.Guid,
                            CreateTime = Manifest.CreateTime,
                            VirtualPath = Children[i]
                        });

                        Index++;
                    }
                }
            }

            ResponseMsg.Builds = Result.ToArray();

            Connection.Send(ResponseMsg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Message"></param>
        private void HandleMessage(NetConnection Connection, NetMessage BaseMessage)
        {
            // ------------------------------------------------------------------------------
            // Remote client requests all builds in a given path.
            // ------------------------------------------------------------------------------
            if (BaseMessage is NetMessage_GetBuilds)
            {
                NetMessage_GetBuilds Msg = BaseMessage as NetMessage_GetBuilds;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request for builds in folder: {0}", Msg.RootPath);

                SendBuildsUpdate(Connection, Msg.RootPath);
            }

            // ------------------------------------------------------------------------------
            // Remove client wants to publish a manifest.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_PublishManifest)
            {
                NetMessage_PublishManifest Msg = BaseMessage as NetMessage_PublishManifest;
                ClientState State = Connection.Metadata as ClientState;

                NetMessage_PublishManifestResponse ResponseMsg = new NetMessage_PublishManifestResponse();
                ResponseMsg.Result = PublishManifestResult.Failed;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request to publish manifest.");
                BuildManifest Manifest = null;
                try
                {
                    Manifest = BuildManifest.FromByteArray(Msg.Data);
                    ResponseMsg.ManifestId = Msg.ManifestId;

                    if (!UserManager.CheckPermission(State.Username, UserPermissionType.ManageBuilds, Manifest.VirtualPath))
                    {
                        ResponseMsg.Result = PublishManifestResult.PermissionDenied;
                    }
                    // Check something doesn't already exist at the virtual path.
                    else if (ManifestRegistry.GetManifestByPath(Manifest.VirtualPath) != null)
                    {
                        ResponseMsg.Result = PublishManifestResult.VirtualPathAlreadyExists;
                    }
                    // Check guid doesn't exist.
                    else if (ManifestRegistry.GetManifestById(Manifest.Guid) != null)
                    {
                        ResponseMsg.Result = PublishManifestResult.GuidAlreadyExists;
                    }
                    // Good to go.
                    else
                    {
                        ManifestRegistry.RegisterManifest(Manifest);
                        ResponseMsg.Result = PublishManifestResult.Success;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to process publish request due to error: {0}", ex.Message);
                }

                Connection.Send(ResponseMsg);

                if (Manifest != null)
                {
                    SendBuildsUpdate(Connection, VirtualFileSystem.GetParentPath(Manifest.VirtualPath));
                }
            }

            // ------------------------------------------------------------------------------
            // Block list update from peer.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_BlockListUpdate)
            {
                NetMessage_BlockListUpdate Msg = BaseMessage as NetMessage_BlockListUpdate;

                ClientState State = Connection.Metadata as ClientState;

                if (State.BlockState != null)
                {
                    Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved block list update from client, applying as patch ({0} states).", Msg.BlockState.States.Length);
                    State.BlockState = Msg.BlockState;// State.BlockState.ApplyDelta(Msg.BlockState);
                }
                else
                {
                    Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved block list update from client, applying as new state ({0} states).", Msg.BlockState.States.Length);
                    State.BlockState = Msg.BlockState;
                }

                // Dirty all peer states to force an address update (todo: this is shit we should only update relevant peers).
                List<NetConnection> Clients = ListenConnection.AllClients;
                foreach (NetConnection ClientConnection in Clients)
                {
                    if (ClientConnection.Metadata != null)
                    {
                        ClientState SubState = ClientConnection.Metadata as ClientState;
                        SubState.RelevantPeerAddressesNeedUpdate = true;
                    }
                }

                PrintClientBlockStates();
            }

            // ------------------------------------------------------------------------------
            // Client connection info update.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_ConnectionInfo)
            {
                NetMessage_ConnectionInfo Msg = BaseMessage as NetMessage_ConnectionInfo;

                ClientState State = Connection.Metadata as ClientState;

                State.PeerConnectionAddress = Msg.PeerConnectionAddress;

                if (State.Username != Msg.Username)
                {
                    State.Username = Msg.Username;
                    State.PermissionsNeedUpdate = true;
                }
                // Dirty all peer states to force an address update (todo: this is shit we should only update relevant peers).
                List<NetConnection> Clients = ListenConnection.AllClients;
                foreach (NetConnection ClientConnection in Clients)
                {
                    if (ClientConnection.Metadata != null)
                    {
                        ClientState SubState = ClientConnection.Metadata as ClientState;
                        SubState.RelevantPeerAddressesNeedUpdate = true;
                    }
                }

                Logger.Log(LogLevel.Info, LogCategory.Main, "Clients username was updated to: " + State.Username);
                Logger.Log(LogLevel.Info, LogCategory.Main, "Clients connection address was updated to: " + State.PeerConnectionAddress.ToString());
            }

            // ------------------------------------------------------------------------------
            // Client requested a manifest for a download.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_GetManifest)
            {
                NetMessage_GetManifest Msg = BaseMessage as NetMessage_GetManifest;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request for manifest: {0}", Msg.ManifestId.ToString());

                try
                {
                    BuildManifest Manifest = ManifestRegistry.GetManifestById(Msg.ManifestId);
                    if (Manifest != null)
                    {
                        NetMessage_GetManifestResponse ResponseMsg = new NetMessage_GetManifestResponse();
                        ResponseMsg.ManifestId = Msg.ManifestId;
                        ResponseMsg.Data = Manifest.ToByteArray();
                        Connection.Send(ResponseMsg);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to process manifest request due to error: {0}", ex.Message);
                }
            }

            // ------------------------------------------------------------------------------
            // Client requested a manifest is deleted.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_DeleteManifest)
            {
                NetMessage_DeleteManifest Msg = BaseMessage as NetMessage_DeleteManifest;

                ClientState State = Connection.Metadata as ClientState;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request for deleting manifest: {0}", Msg.ManifestId.ToString());

                try
                {
                    BuildManifest Manifest = ManifestRegistry.GetManifestById(Msg.ManifestId);
                    if (Manifest != null)
                    {
                        string Path = Manifest.VirtualPath;

                        if (!UserManager.CheckPermission(State.Username, UserPermissionType.ManageBuilds, Path))
                        {
                            Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to delete build without permission.", State.Username);
                            return;
                        }

                        ManifestRegistry.UnregisterManifest(Msg.ManifestId);

                        NetMessage_DeleteManifestResponse ResponseMsg = new NetMessage_DeleteManifestResponse();
                        ResponseMsg.ManifestId = Msg.ManifestId;
                        Connection.Send(ResponseMsg);

                        SendBuildsUpdate(Connection, VirtualFileSystem.GetParentPath(Path));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, LogCategory.Main, "Failed to process manifest delete request due to error: {0}", ex.Message);
                }
            }

            // ------------------------------------------------------------------------------
            // Client requested list of users
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_GetUsers)
            {
                NetMessage_GetUsers Msg = BaseMessage as NetMessage_GetUsers;

                ClientState State = Connection.Metadata as ClientState;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ManageUsers, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to get usernames without permission.", State.Username);
                    return;
                }

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request for user list.");

                NetMessage_GetUsersResponse ResponseMsg = new NetMessage_GetUsersResponse();
                ResponseMsg.Users = UserManager.Users;
                Connection.Send(ResponseMsg);
            }

            // ------------------------------------------------------------------------------
            // Client requested to set a users permissions
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_SetUserPermissions)
            {
                NetMessage_SetUserPermissions Msg = BaseMessage as NetMessage_SetUserPermissions;

                ClientState State = Connection.Metadata as ClientState;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ManageUsers, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to set user permissions without permission.", State.Username);
                    return;
                }

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request for setting permissions of '{0}'", Msg.Username);

                UserManager.SetPermissions(Msg.Username, Msg.Permissions);
            }

            // ------------------------------------------------------------------------------
            // Client requested to delete a user
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_DeleteUser)
            {
                NetMessage_DeleteUser Msg = BaseMessage as NetMessage_DeleteUser;

                ClientState State = Connection.Metadata as ClientState;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ManageUsers, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to delete user without permission.", State.Username);
                    return;
                }

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request to delete user '{0}'", Msg.Username);

                UserManager.DeleteUser(Msg.Username);
            }
        }
    }
}
