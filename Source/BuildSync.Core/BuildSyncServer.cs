using System;
using System.Net;
using System.Collections.Generic;
using BuildSync.Core.Networking;
using BuildSync.Core.Networking.Messages;
using BuildSync.Core.Manifests;
using BuildSync.Core.Downloads;

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
            public BlockListState BlockState = null;
            public List<IPEndPoint> RelevantPeerAddresses = new List<IPEndPoint>();
            public bool RelevantPeerAddressesNeedUpdate = false;

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
        private int LastListenAttempt = 0;

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
        public BuildSyncServer()
        {
            ListenConnection.OnClientMessageRecieved += HandleMessage;
            ListenConnection.OnClientConnect += ClientConnected;
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
        public void Start(int Port, BuildManifestRegistry BuildManifest)
        {
            ServerPort = Port;
            Started = true;
            ManifestRegistry = BuildManifest;
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
                    int ElapsedTime = Environment.TickCount - LastListenAttempt;
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

                        // Send it!
                        if (IsDifferent)
                        {
                            Console.WriteLine("----- Peer Relevant Addresses -----");
                            Console.WriteLine("Peer: {0}", Connection.ListenAddress == null ? "Unknown" : Connection.ListenAddress.ToString());

                            for (int i = 0; i < NewPeers.Count; i++)
                            {
                                Console.WriteLine("[{0}] {1}", i, NewPeers[i].ToString());
                            }

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
            LastListenAttempt = Environment.TickCount;
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

            Console.WriteLine("=======================");
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

                Console.WriteLine("Peer[{0}]", ClientState.PeerConnectionAddress == null ? "Unknown" : ClientState.PeerConnectionAddress.ToString());
                for (int i = 0; i < ClientState.BlockState.States.Length; i++)
                {
                    ManifestBlockListState State = ClientState.BlockState.States[i];
                    Console.WriteLine("\tManifest[{0}] Id={1} Active={2}", i, State.Id.ToString(), State.IsActive);
                    if (State.BlockState.Ranges != null)
                    {
                        for (int j = 0; j < State.BlockState.Ranges.Count; j++)
                        {
                            Console.WriteLine("\t\tRegion[{0}] Start={1} End={2} Active={3}", j, State.BlockState.Ranges[j].Start, State.BlockState.Ranges[j].End, State.BlockState.Ranges[j].State);
                        }
                    }
                }
            }
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

                Console.WriteLine("Recieved request for builds in folder: {0}", Msg.RootPath);

                List<string> Children = ManifestRegistry.GetVirtualPathChildren(Msg.RootPath);

                NetMessage_GetBuildsResponse ResponseMsg = new NetMessage_GetBuildsResponse();
                ResponseMsg.RootPath = Msg.RootPath;
                ResponseMsg.Builds = new NetMessage_GetBuildsResponse.BuildInfo[Children.Count];
                for (int i = 0; i < ResponseMsg.Builds.Length; i++)
                {
                    BuildManifest Manifest = ManifestRegistry.GetManifestByPath(Children[i]);

                    ResponseMsg.Builds[i].Guid = Manifest != null ? Manifest.Guid : Guid.Empty;
                    ResponseMsg.Builds[i].VirtualPath = Children[i];
                }

                Connection.Send(ResponseMsg);
            }

            // ------------------------------------------------------------------------------
            // Remove client wants to publish a manifest.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_PublishManifest)
            {
                NetMessage_PublishManifest Msg = BaseMessage as NetMessage_PublishManifest;

                NetMessage_PublishManifestResponse ResponseMsg = new NetMessage_PublishManifestResponse();
                ResponseMsg.Result = PublishManifestResult.Failed;

                Console.WriteLine("Recieved request to publish manifest.");
                try
                {
                    BuildManifest Manifest = BuildManifest.FromByteArray(Msg.Data);
                    ResponseMsg.ManifestId = Msg.ManifestId;

                    // Check something doesn't already exist at the virtual path.
                    if (ManifestRegistry.GetManifestByPath(Manifest.VirtualPath) != null)
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
                    Console.WriteLine("Failed to process publish request due to error: {0}", ex.Message);
                }

                Connection.Send(ResponseMsg);
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
                    Console.WriteLine("Recieved block list update from client, applying as patch ({0} states).", Msg.BlockState.States.Length);
                    State.BlockState = Msg.BlockState;// State.BlockState.ApplyDelta(Msg.BlockState);
                }
                else
                {
                    Console.WriteLine("Recieved block list update from client, applying as new state ({0} states).", Msg.BlockState.States.Length);
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
            // Peer connection info update.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_ConnectionInfo)
            {
                NetMessage_ConnectionInfo Msg = BaseMessage as NetMessage_ConnectionInfo;

                ClientState State = Connection.Metadata as ClientState;

                State.PeerConnectionAddress = Msg.PeerConnectionAddress;

                Console.WriteLine("Peers connection address was updated to: " + State.PeerConnectionAddress.ToString());
            }

            // ------------------------------------------------------------------------------
            // Client requested a manifest for a download.
            // ------------------------------------------------------------------------------
            else if (BaseMessage is NetMessage_GetManifest)
            {
                NetMessage_GetManifest Msg = BaseMessage as NetMessage_GetManifest;

                Console.WriteLine("Recieved request for manifest: {0}", Msg.ManifestId.ToString());

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
                    Console.WriteLine("Failed to process manifest request due to error: {0}", ex.Message);
                }
            }            
        }
    }
}
