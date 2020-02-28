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
using System.Net;
using BuildSync.Core.Downloads;
using BuildSync.Core.Licensing;
using BuildSync.Core.Manifests;
using BuildSync.Core.Networking;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Users;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Server
{
    /// <summary>
    /// </summary>
    public class Server
    {
        /// <summary>
        /// 
        /// </summary>
        private class ServerClientManifestInfo
        {
            public int PeersWithAllBlocks;
        };

        /// <summary>
        /// </summary>
        private const int ListenAttemptInterval = 2 * 1000;

        /// <summary>
        /// </summary>
        public long BandwidthLimit;

        /// <summary>
        /// </summary>
        public int MaxConnectedClients = 0;

        /// <summary>
        /// </summary>
        private ulong LastListenAttempt;

        /// <summary>
        /// </summary>
        private LicenseManager LicenseManager;

        /// <summary>
        /// </summary>
        private readonly NetConnection ListenConnection = new NetConnection();

        /// <summary>
        /// </summary>
        private BuildManifestRegistry ManifestRegistry;

        /// <summary>
        /// </summary>
        private readonly NetDiscoveryServer NetDiscoveryServer = new NetDiscoveryServer();

        /// <summary>
        /// </summary>
        private int ServerPort;

        /// <summary>
        /// </summary>
        private bool Started;

        /// <summary>
        /// </summary>
        private UserManager UserManager;

        /// <summary>
        /// </summary>
        public int ClientCount => ListenConnection.AllClients.Count;

        /// <summary>
        /// 
        /// </summary>
        Dictionary<Guid, ServerClientManifestInfo> ClientManifestInfo = new Dictionary<Guid, ServerClientManifestInfo>();

        /// <summary>
        /// </summary>
        public Server()
        {
            ListenConnection.OnClientMessageRecieved += HandleMessage;
            ListenConnection.OnClientConnect += ClientConnected;
            ListenConnection.OnClientDisconnect += ClientDisconnected;

            MemoryPool.PreallocateBuffers((int)BuildManifest.BlockSize, 16);
            NetConnection.PreallocateBuffers(NetConnection.MaxRecieveMessageBuffers, NetConnection.MaxSendMessageBuffers, NetConnection.MaxGenericMessageBuffers, NetConnection.MaxSmallMessageBuffers);
        }

        /// <summary>
        /// </summary>
        public void Disconnect()
        {
            Started = false;
            ListenConnection.Disconnect();
        }

        /// <summary>
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
            ListenConnection.MaxConnectedClients = MaxConnectedClients;

            List<NetConnection> ActivelyDownloadingClients = new List<NetConnection>();

            List<NetConnection> Clients = ListenConnection.AllClients;
            foreach (NetConnection Connection in Clients)
            {
                if (Connection.Metadata != null)
                {
                    ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                    // Determine if client is actively download for bandwidth limitation later.
                    if (State.BlockState != null)
                    {
                        foreach (ManifestBlockListState ManifestState in State.BlockState.States)
                        {
                            if (ManifestState.IsActive)
                            {
                                ActivelyDownloadingClients.Add(Connection);
                                break;
                            }
                        }
                    }

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
                        bool IsDifferent = NewPeers.Count != State.RelevantPeerAddresses.Count;
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

                        Logger.Log(LogLevel.Verbose, LogCategory.Peers, "----- Peer Relevant Addresses -----");
                        Logger.Log(LogLevel.Verbose, LogCategory.Peers, "Peer: {0}", State.PeerConnectionAddress == null ? "Unknown" : State.PeerConnectionAddress.ToString());
                        if (NewPeers.Count > 0)
                        {
                            for (int i = 0; i < NewPeers.Count; i++)
                            {
                                Logger.Log(LogLevel.Verbose, LogCategory.Peers, "[{0}] {1}", i, NewPeers[i].ToString());
                            }
                        }
                        else
                        {
                            Logger.Log(LogLevel.Verbose, LogCategory.Peers, "\tNo Relevant Peers");
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

            // Limit bandwidth between clients if required.
            long PerClientBandwidthLimit = ActivelyDownloadingClients.Count > 0 ? BandwidthLimit / ActivelyDownloadingClients.Count : 0;
            foreach (NetConnection Connection in ActivelyDownloadingClients)
            {
                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;
                if (State.BandwidthLimit != PerClientBandwidthLimit)
                {
                    NetMessage_EnforceBandwidthLimit Msg = new NetMessage_EnforceBandwidthLimit();
                    Msg.BandwidthLimit = PerClientBandwidthLimit;
                    Connection.Send(Msg);

                    Logger.Log(LogLevel.Info, LogCategory.Peers, "Limiting peer {0} to {1}", Connection.Address.Address.ToString(), StringUtils.FormatAsTransferRate(PerClientBandwidthLimit));

                    State.BandwidthLimit = PerClientBandwidthLimit;
                }
            }

            ListenConnection.Poll();
        }

        /// <summary>
        /// </summary>
        /// <param name="Hostname"></param>
        /// <param name="Port"></param>
        public void Start(int Port, BuildManifestRegistry BuildManifest, UserManager InUserManager, LicenseManager InLicenseManager)
        {
            ServerPort = Port;
            Started = true;
            ManifestRegistry = BuildManifest;

            LicenseManager = InLicenseManager;

            UserManager = InUserManager;
            UserManager.PermissionsUpdated += user =>
            {
                List<NetConnection> Clients = ListenConnection.AllClients;
                foreach (NetConnection Connection in Clients)
                {
                    if (Connection.Metadata != null)
                    {
                        ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;
                        if (State.Username == user.Username)
                        {
                            State.PermissionsNeedUpdate = true;
                        }
                    }
                }
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="ClientConnection"></param>
        private void ClientConnected(NetConnection Connection, NetConnection ClientConnection)
        {
            ClientConnection.Metadata = new ServerConnectedClient();
        }

        /// <summary>
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="ClientConnection"></param>
        private void ClientDisconnected(NetConnection Connection, NetConnection ClientConnection)
        {
            UpdateGlobalBlockInformation();
        }

        /// <summary>
        /// </summary>
        /// <param name="Clients"></param>
        /// <param name="ForClient"></param>
        /// <returns></returns>
        private List<IPEndPoint> GetRelevantPeerAddressesForBlockState(List<NetConnection> Clients, NetConnection ForClient)
        {
            List<IPEndPoint> Result = new List<IPEndPoint>();

            ServerConnectedClient ForServerConnectedClient = ForClient.Metadata as ServerConnectedClient;
            if (ForServerConnectedClient.BlockState == null)
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

                ServerConnectedClient ServerConnectedClient = Connection.Metadata as ServerConnectedClient;
                if (ServerConnectedClient.BlockState == null)
                {
                    continue;
                }

                if (ServerConnectedClient.PeerConnectionAddress == null)
                {
                    continue;
                }

                if (ServerConnectedClient.BlockState.HasAnyBlocksNeeded(ForServerConnectedClient.BlockState))
                {
                    Result.Add(ServerConnectedClient.PeerConnectionAddress);
                }
            }

            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateGlobalBlockInformation()
        {
            ClientManifestInfo.Clear();

            List<NetConnection> Clients = ListenConnection.AllClients;
            foreach (NetConnection ClientConnection in Clients)
            {
                if (ClientConnection.Metadata != null)
                {
                    ServerConnectedClient Sub = ClientConnection.Metadata as ServerConnectedClient;
                    if (Sub.BlockState != null)
                    {
                        foreach (ManifestBlockListState State in Sub.BlockState.States)
                        {
                            if (!ClientManifestInfo.ContainsKey(State.Id))
                            {
                                ClientManifestInfo.Add(State.Id, new ServerClientManifestInfo());
                            }

                            if (State.BlockState.AreAllSet(true))
                            {
                                ClientManifestInfo[State.Id].PeersWithAllBlocks++;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetPeerCountForManifest(Guid Id)
        {
            if (ClientManifestInfo.ContainsKey(Id))
            {
                return ClientManifestInfo[Id].PeersWithAllBlocks;
            }

            return 0;
        }

        /// <summary>
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

                Logger.Log(LogLevel.Verbose, LogCategory.Main, "Recieved request for builds in folder: {0}", Msg.RootPath);

                SendBuildsUpdate(Connection, Msg.RootPath);
            }

            // ------------------------------------------------------------------------------
            // Remove client wants to publish a manifest.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_PublishManifest)
            {
                NetMessage_PublishManifest Msg = BaseMessage as NetMessage_PublishManifest;
                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

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

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (State.BlockState != null)
                {
                    Logger.Log(LogLevel.Verbose, LogCategory.Main, "Recieved block list update from client, applying as patch ({0} states).", Msg.BlockState.States.Length);
                    State.BlockState = Msg.BlockState; // State.BlockState.ApplyDelta(Msg.BlockState);
                }
                else
                {
                    Logger.Log(LogLevel.Verbose, LogCategory.Main, "Recieved block list update from client, applying as new state ({0} states).", Msg.BlockState.States.Length);
                    State.BlockState = Msg.BlockState;
                }

                if (State.BlockState != null)
                {
                    foreach (ManifestBlockListState ManifestState in State.BlockState.States)
                    {
                        ManifestRegistry.MarkAsSeen(ManifestState.Id);
                    }
                }

                // Dirty all peer states to force an address update (todo: this is shit we should only update relevant peers).
                List<NetConnection> Clients = ListenConnection.AllClients;
                foreach (NetConnection ClientConnection in Clients)
                {
                    if (ClientConnection.Metadata != null)
                    {
                        ServerConnectedClient Sub = ClientConnection.Metadata as ServerConnectedClient;
                        Sub.RelevantPeerAddressesNeedUpdate = true;
                    }
                }

                UpdateGlobalBlockInformation();
                PrintClientBlockStates();
            }

            // ------------------------------------------------------------------------------
            // Client connection info update.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_ConnectionInfo)
            {
                NetMessage_ConnectionInfo Msg = BaseMessage as NetMessage_ConnectionInfo;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

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
                        ServerConnectedClient Sub = ClientConnection.Metadata as ServerConnectedClient;
                        Sub.RelevantPeerAddressesNeedUpdate = true;
                    }
                }

                Logger.Log(LogLevel.Info, LogCategory.Main, "Clients username was updated to: " + State.Username);
                Logger.Log(LogLevel.Info, LogCategory.Main, "Clients connection address was updated to: " + State.PeerConnectionAddress);
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

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

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

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

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

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

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

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ManageUsers, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to delete user without permission.", State.Username);
                    return;
                }

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request to delete user '{0}'", Msg.Username);

                UserManager.DeleteUser(Msg.Username);
            }

            // ------------------------------------------------------------------------------
            // License info requested.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_GetLicenseInfo)
            {
                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request for license info.");

                NetMessage_GetLicenseInfoResponse ResponseMsg = new NetMessage_GetLicenseInfoResponse();
                ResponseMsg.License = LicenseManager.ActiveLicense;
                Connection.Send(ResponseMsg);
            }

            // ------------------------------------------------------------------------------
            // Applying license requested.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_ApplyLicense)
            {
                NetMessage_ApplyLicense Msg = BaseMessage as NetMessage_ApplyLicense;

                Logger.Log(LogLevel.Info, LogCategory.Main, "Recieved request to apply license.");

                LicenseManager.Apply(Msg.License);

                NetMessage_GetLicenseInfoResponse ResponseMsg = new NetMessage_GetLicenseInfoResponse();
                ResponseMsg.License = LicenseManager.ActiveLicense;
                Connection.Send(ResponseMsg);
            }

            // ------------------------------------------------------------------------------
            // Server state requested
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_GetServerState)
            {
                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ManageServer, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to manager server without permission.", State.Username);
                    return;
                }

                NetMessage_GetServerStateResponse ResponseMsg = new NetMessage_GetServerStateResponse();
                ResponseMsg.BandwidthLimit = BandwidthLimit;

                List<NetConnection> Clients = ListenConnection.AllClients;
                foreach (NetConnection ClientConnection in Clients)
                {
                    if (ClientConnection.Metadata != null)
                    {
                        ServerConnectedClient Sub = ClientConnection.Metadata as ServerConnectedClient;
                        if (Sub.PeerConnectionAddress != null)
                        {
                            NetMessage_GetServerStateResponse.ClientState NewState = new NetMessage_GetServerStateResponse.ClientState();
                            NewState.Address = Sub.PeerConnectionAddress.Address.ToString();
                            NewState.DownloadRate = Sub.DownloadRate;
                            NewState.UploadRate = Sub.UploadRate;
                            NewState.TotalDownloaded = Sub.TotalDownloaded;
                            NewState.TotalUploaded = Sub.TotalUploaded;
                            NewState.ConnectedPeerCount = Sub.ConnectedPeerCount;
                            NewState.DiskUsage = Sub.DiskUsage;
                            NewState.Version = Sub.Version;

                            ResponseMsg.ClientStates.Add(NewState);
                        }
                    }
                }

                Connection.Send(ResponseMsg);
            }

            // ------------------------------------------------------------------------------
            // Recieved client state update.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_ClientStateUpdate)
            {
                NetMessage_ClientStateUpdate Msg = BaseMessage as NetMessage_ClientStateUpdate;

                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                State.TotalDownloaded = Msg.TotalDownloaded;
                State.TotalUploaded = Msg.TotalUploaded;
                State.DownloadRate = Msg.DownloadRate;
                State.UploadRate = Msg.UploadRate;
                State.ConnectedPeerCount = Msg.ConnectedPeerCount;
                State.DiskUsage = Msg.DiskUsage;
                State.Version = Msg.Version;
                State.VersionNumeric = StringUtils.ConvertSemanticVerisonNumber(State.Version);
            }

            // ------------------------------------------------------------------------------
            // Changing server max bandwidth
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_SetServerMaxBandwidth)
            {
                ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

                if (!UserManager.CheckPermission(State.Username, UserPermissionType.ManageServer, ""))
                {
                    Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' tried to manager server without permission.", State.Username);
                    return;
                }

                NetMessage_SetServerMaxBandwidth Msg = BaseMessage as NetMessage_SetServerMaxBandwidth;
                BandwidthLimit = Msg.BandwidthLimit;

                Logger.Log(LogLevel.Warning, LogCategory.Main, "User '{0}' changed global bandwidth limit to {1}.", State.Username, StringUtils.FormatAsTransferRate(Msg.BandwidthLimit));
            }
        }

        /// <summary>
        /// </summary>
        private void ListenForClients()
        {
            ListenConnection.BeginListen(ServerPort);
            LastListenAttempt = TimeUtils.Ticks;

            NetDiscoveryServer.ResponseData.Name = Dns.GetHostName();
            NetDiscoveryServer.ResponseData.Address = ListenConnection.ListenAddress.Address.ToString();
            NetDiscoveryServer.ResponseData.Port = ListenConnection.ListenAddress.Port;
        }

        /// <summary>
        /// </summary>
        private void PrintClientBlockStates()
        {
            List<NetConnection> Clients = ListenConnection.AllClients;

            Logger.Log(LogLevel.Verbose, LogCategory.Peers, "=======================");
            foreach (NetConnection Connection in Clients)
            {
                if (Connection.Metadata == null)
                {
                    continue;
                }

                ServerConnectedClient ServerConnectedClient = Connection.Metadata as ServerConnectedClient;
                if (ServerConnectedClient.BlockState == null)
                {
                    continue;
                }

                Logger.Log(LogLevel.Verbose, LogCategory.Peers, "Peer[{0}]", ServerConnectedClient.PeerConnectionAddress == null ? "Unknown" : ServerConnectedClient.PeerConnectionAddress.ToString());
                for (int i = 0; i < ServerConnectedClient.BlockState.States.Length; i++)
                {
                    ManifestBlockListState State = ServerConnectedClient.BlockState.States[i];
                    Logger.Log(LogLevel.Verbose, LogCategory.Peers, "\tManifest[{0}] Id={1} Active={2}", i, State.Id.ToString(), State.IsActive);
                    if (State.BlockState.Ranges != null)
                    {
                        for (int j = 0; j < State.BlockState.Ranges.Count; j++)
                        {
                            Logger.Log(LogLevel.Verbose, LogCategory.Peers, "\t\tRegion[{0}] Start={1} End={2} Active={3}", j, State.BlockState.Ranges[j].Start, State.BlockState.Ranges[j].End, State.BlockState.Ranges[j].State);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="Root"></param>
        private void SendBuildsUpdate(NetConnection Connection, string RootPath)
        {
            ServerConnectedClient State = Connection.Metadata as ServerConnectedClient;

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
                        Result.Add(
                            new NetMessage_GetBuildsResponse.BuildInfo
                            {
                                Guid = Guid.Empty,
                                CreateTime = DateTime.UtcNow,
                                VirtualPath = Children[i],
                                AvailablePeers = 0,
                                LastSeenOnPeer = DateTime.UtcNow,
                                TotalSize = 0
                            }
                        ); ;
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
                        Result.Add(
                            new NetMessage_GetBuildsResponse.BuildInfo
                            {
                                Guid = Manifest.Guid,
                                CreateTime = Manifest.CreateTime,
                                VirtualPath = Children[i],
                                AvailablePeers = (ulong)GetPeerCountForManifest(Manifest.Guid),
                                LastSeenOnPeer = ManifestRegistry.GetLastSeenTime(Manifest.Guid),
                                TotalSize = (ulong)Manifest.GetTotalSize()
                            }
                        );

                        Index++;
                    }
                }
            }

            ResponseMsg.Builds = Result.ToArray();

            // If build is old enough, send simplified response.
            if (State.VersionNumeric <= 100000396)
            {
                ResponseMsg.SendLegacyVersion = true;
            }

            Connection.Send(ResponseMsg);
        }
    }
}