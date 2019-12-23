using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using BuildSync.Core.Manifests;
using BuildSync.Core.Downloads;
using BuildSync.Core.Networking;
using BuildSync.Core.Networking.Messages;

namespace BuildSync.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Connection"></param>
    /// <param name="Message"></param>
    public delegate void BuildsRecievedHandler(string RootPath, NetMessage_GetBuildsResponse.BuildInfo[] Builds);

    /// <summary>
    /// 
    /// </summary>
    public delegate void ConenctedToServerHandler();

    /// <summary>
    /// 
    /// </summary>
    public delegate void ManifestPublishResultRecievedHandler(Guid ManifestId, PublishManifestResult Result);

    /// <summary>
    /// 
    /// </summary>
    public class BuildSyncClient
    {
        /// <summary>
        /// 
        /// </summary>
        public class Peer
        {
            /// <summary>
            /// 
            /// </summary>
            public IPEndPoint Address;

            /// <summary>
            /// 
            /// </summary>
            public NetConnection Connection = new NetConnection();

            /// <summary>
            /// 
            /// </summary>
            public int LastConnectionAttemptTime = 0;

            /// <summary>
            /// 
            /// </summary>
            public bool RemoteInitiated = false;

            /// <summary>
            /// 
            /// </summary>
            public bool WasConnected = false;

            /// <summary>
            /// 
            /// </summary>
            public BlockListState BlockState = new BlockListState();

            /// <summary>
            /// 
            /// </summary>
            public List<ManifestPendingDownloadBlock> ActiveBlockDownloads = new List<ManifestPendingDownloadBlock>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name=""></param>
            /// <returns></returns>
            public bool IsDownloadingBlock(ManifestPendingDownloadBlock Block)
            {
                foreach (ManifestPendingDownloadBlock Download in ActiveBlockDownloads)
                {
                    if (Download.BlockIndex == Block.BlockIndex && Download.ManifestId == Block.ManifestId)
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public bool HasBlock(ManifestPendingDownloadBlock Block)
            {
                foreach (ManifestBlockListState ManifestState in BlockState.States)
                {
                    if (ManifestState.Id == Block.ManifestId)
                    {
                        return ManifestState.BlockState.Get(Block.BlockIndex);
                    }
                }
                return false;
            }
        };

        /// <summary>
        /// 
        /// </summary>
        private NetConnection Connection = new NetConnection();

        /// <summary>
        /// 
        /// </summary>
        private NetConnection ListenConnection = new NetConnection();

        /// <summary>
        /// 
        /// </summary>
        private int LastConnectionAttempt = 0;

        /// <summary>
        /// 
        /// </summary>
        private const int ConnectionAttemptInterval = 10 * 1000;

        /// <summary>
        /// 
        /// </summary>
        public string ServerHostname;

        /// <summary>
        /// 
        /// </summary>
        private const int ConcurrentBlockDownloadLimit = 20;

        /// <summary>
        /// 
        /// </summary>
        public int ServerPort = 0;

        /// <summary>
        /// 
        /// </summary>
        public int PeerListenPortRangeMin = 0;

        /// <summary>
        /// 
        /// </summary>
        public int PeerListenPortRangeMax = 0;

        /// <summary>
        /// 
        /// </summary>
        private int PortIndex = 0;

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
        private const int IdealPeerCount = 20;

        /// <summary>
        /// 
        /// </summary>
        private bool ConnectionInfoUpdateRequired = false;

        /// <summary>
        /// 
        /// </summary>
        private bool Started = false;

        /// <summary>
        /// 
        /// </summary>
        private bool BlockListUpdatePending = false;

        /// <summary>
        /// 
        /// </summary>
        private int LastManifestStateDirtyCounter = 0;

        /// <summary>
        /// 
        /// </summary>
        private int LastBlockListUpdateTime = 0;

        /// <summary>
        /// 
        /// </summary>
        private const int BlockListUpdateInterval = 15 * 1000;

        /// <summary>
        /// 
        /// </summary>
        private int PeerCycleIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        private BlockListState LastBlockListState = null;

        /// <summary>
        /// 
        /// </summary>
        private BuildManifestRegistry ManifestRegistry = null;

        /// <summary>
        /// 
        /// </summary>
        private List<IPEndPoint> RelevantPeerAddresses;

        /// <summary>
        /// 
        /// </summary>
        private List<Peer> Peers = new List<Peer>();

        /// <summary>
        /// 
        /// </summary>
        private ManifestDownloadManager ManifestDownloadManager = null;

        /// <summary>
        /// 
        /// </summary>
        public Queue<Action> DeferredActions = new Queue<Action>();

        /// <summary>
        /// 
        /// </summary>
        public event BuildsRecievedHandler OnBuildsRecieved;

        /// <summary>
        /// 
        /// </summary>
        public event ManifestPublishResultRecievedHandler OnManifestPublishResultRecieved;

        /// <summary>
        /// 
        /// </summary>
        public event ConenctedToServerHandler OnConnectedToServer;

        /// <summary>
        /// 
        /// </summary>
        public bool IsConnected
        {
            get { return Connection != null ? Connection.IsConnected : false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool InternalConnectionsDisabled = false;

        /// <summary>
        /// 
        /// </summary>
        public bool ConnectionsDisabled
        {
            get
            {
                return InternalConnectionsDisabled;
            }
            set
            {
                bool Changed = (InternalConnectionsDisabled != value);
                InternalConnectionsDisabled = value;
                if (InternalConnectionsDisabled && Changed)
                {
                    RestartConnections();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int PeerCount
        {
            get
            {
                lock (Peers)
                {
                    return Peers.Count;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BuildSyncClient()
        {
            Connection.OnMessageRecieved += HandleMessage;
            Connection.OnConnect += (NetConnection Connection) => { OnConnectedToServer?.Invoke(); };

            ListenConnection.OnClientConnect += PeerConnected;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            Started = false;

            RestartConnections();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RestartConnections()
        {   
            lock (Peers)
            {
                foreach (Peer peer in Peers)
                {
                    peer.Connection.Disconnect();
                }

                Connection.Disconnect();
                ListenConnection.Disconnect();

                // Deferred actions contain responses to connection events, so purge them. 
                lock (DeferredActions)
                {
                    DeferredActions.Clear();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Hostname"></param>
        /// <param name="Port"></param>
        public void Start(string Hostname, int Port, int ListenPortRangeMin, int ListenPortRangeMax, BuildManifestRegistry BuildManifest, ManifestDownloadManager DownloadManager)
        {
            ServerHostname = Hostname;
            ServerPort = Port;
            PeerListenPortRangeMin = ListenPortRangeMin;
            PeerListenPortRangeMax = ListenPortRangeMax;
            ManifestRegistry = BuildManifest;
            ManifestDownloadManager = DownloadManager;
            ManifestDownloadManager.OnManifestRequested += (Guid Id) => {
                RequestManifest(Id);
            };
            Started = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Poll()
        {
            if (Started)
            {
                // Reconnect?
                if (!Connection.IsConnected && !Connection.IsConnecting && !InternalConnectionsDisabled)
                {
                    int ElapsedTime = Environment.TickCount - LastConnectionAttempt;
                    if (ElapsedTime > ConnectionAttemptInterval)
                    {
                        ConnectionInfoUpdateRequired = true;
                        ConnectToServer();
                    }
                }

                // Reattempt to create listen server?
                if (!ListenConnection.IsListening && !InternalConnectionsDisabled)
                {
                    int ElapsedTime = Environment.TickCount - LastListenAttempt;
                    if (ElapsedTime > ListenAttemptInterval)
                    {
                        ConnectionInfoUpdateRequired = true;
                        ListenForPeers();
                    }
                }
            }

            // Periodically send our manifest block state to the server.
            if (Connection.IsConnected)
            {
                if (ManifestDownloadManager.StateDirtyCount > LastManifestStateDirtyCounter)
                {
                    BlockListUpdatePending = true;
                    LastManifestStateDirtyCounter = ManifestDownloadManager.StateDirtyCount;
                }

                if (ConnectionInfoUpdateRequired && ListenConnection.IsListening)
                {
                    SendConnectionInfo();
                    ConnectionInfoUpdateRequired = false;
                }

                int ElapsedTime = Environment.TickCount - LastBlockListUpdateTime;
                if (ElapsedTime > BlockListUpdateInterval && BlockListUpdatePending)
                {
                    SendBlockListUpdate();

                    LastBlockListUpdateTime = Environment.TickCount;
                    BlockListUpdatePending = false;
                }
            }
            else
            {
                BlockListUpdatePending = true;
                LastBlockListUpdateTime = 0;
                LastBlockListState = null;
            }

            if (!InternalConnectionsDisabled)
            {
                ConnectToPeers();
            }

            Connection.Poll();
            ListenConnection.Poll();

            // Poll local initiated peer connections.
            lock (Peers)
            {
                foreach (Peer peer in Peers)
                {
                    if (!peer.RemoteInitiated)
                    {
                        peer.Connection.Poll();
                    }
                }
            }

            UpdateBlockDownloads();

            // Execute all deferred actions.
            lock (DeferredActions)
            {
                while (DeferredActions.Count > 0)
                {
                    DeferredActions.Dequeue().Invoke();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ConnectToServer()
        {
            Connection.BeginConnect(ServerHostname, ServerPort);

            LastConnectionAttempt = Environment.TickCount;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ListenForPeers()
        {
            int PortCount = (PeerListenPortRangeMax - PeerListenPortRangeMin) + 1;
            int Port = PeerListenPortRangeMin + (PortIndex++ % PortCount);
            ListenConnection.BeginListen(Port, false);

            LastListenAttempt = Environment.TickCount;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateAvailableBlocks()
        {
            // Inform the download manager what blocks are available from peers.
            BlockListState State = new BlockListState();

            lock (Peers)
            {
                foreach (Peer peer in Peers)
                {
                    if (peer.BlockState != null)
                    {
                        State.Union(peer.BlockState);
                    }
                }
            }

            ManifestDownloadManager.SetAvailableToDownloadBlocks(State);
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateBlockDownloads()
        {
            ManifestDownloadManager.UpdateBlockQueue();

            if (ManifestDownloadManager.DownloadQueue == null)
            {
                return;
            }

            lock (Peers)
            {
                for (int i = 0; i < ManifestDownloadManager.DownloadQueue.Count; i++)
                {
                    ManifestPendingDownloadBlock Item = ManifestDownloadManager.DownloadQueue[i];

                    // Make sure block is not already being downloaded.
                    // TODO: Change to a set lookup, this is slow.
                    bool AlreadyDownloading = false;
                    foreach (Peer peer in Peers)
                    {
                        if (peer.IsDownloadingBlock(Item))
                        {
                            AlreadyDownloading = true;
                            continue;
                        }
                    }
                    if (AlreadyDownloading)
                    {
                        continue;
                    }

                    // Try and find a peer with space in its download queue for this item.
                    // Cycle peers to request from so we don't always hit the first if he has everything.
                    // TODO: If recent request failed, wait.
                    for (int j = 0; j < Peers.Count; j++)
                    {
                        Peer peer = Peers[(j + PeerCycleIndex) % Peers.Count];

                        if (peer.ActiveBlockDownloads.Count < ConcurrentBlockDownloadLimit && peer.HasBlock(Item))
                        {
                            NetMessage_GetBlock Msg = new NetMessage_GetBlock();
                            Msg.ManifestId = Item.ManifestId;
                            Msg.BlockIndex = Item.BlockIndex;
                            peer.Connection.Send(Msg);

                            peer.ActiveBlockDownloads.Add(Item);
                            break;
                        }
                    }

                    if (Peers.Count > 0)
                    {
                        PeerCycleIndex = (++PeerCycleIndex % Peers.Count);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EndPoint"></param>
        /// <returns></returns>
        private Peer GetPeerByAddress(IPEndPoint EndPoint)
        {
            lock (Peers)
            {
                foreach (Peer peer in Peers)
                {
                    if (peer.Address.Equals(EndPoint))
                    {
                        return peer;
                    }
                }
            }           
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EndPoint"></param>
        /// <returns></returns>
        private Peer GetPeerByConnection(NetConnection Connection)
        {
            lock (Peers)
            {
                foreach (Peer peer in Peers)
                {
                    if (peer.Connection == Connection)
                    {
                        return peer;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ConnectToPeers()
        {
            if (RelevantPeerAddresses != null)
            {
                // Connect to relevant peers we are not already connected to.
                foreach (IPEndPoint EndPoint in RelevantPeerAddresses)
                {
                    Peer peer = null;
                    lock (Peers)
                    {
                        peer = GetPeerByAddress(EndPoint);
                        if (peer == null)
                        {
                            peer = new Peer();
                            peer.Address = EndPoint;
                            peer.RemoteInitiated = false;
                            peer.LastConnectionAttemptTime = 0;
                            peer.Connection.OnMessageRecieved += HandleMessage;
                            Peers.Add(peer);
                        }
                    }

                    // Attempt connection if time has elapsed.
                    if (!peer.Connection.IsConnected && !peer.Connection.IsConnecting && !peer.RemoteInitiated)
                    {
                        int Elapsed = Environment.TickCount - peer.LastConnectionAttemptTime;
                        if (peer.LastConnectionAttemptTime == 0 || Elapsed > ConnectionAttemptInterval)
                        {
                            Console.WriteLine("Connecting to peer: {0}", peer.Address.ToString());
                            peer.Connection.BeginConnect(peer.Address.Address.ToString(), peer.Address.Port);

                            peer.LastConnectionAttemptTime = Environment.TickCount;
                        }
                    }
                    else if (peer.Connection.IsConnected)
                    {
                        peer.LastConnectionAttemptTime = Environment.TickCount;

                        if (!peer.WasConnected)
                        {
                            // Exchange block list with remote.
                            BlockListUpdatePending = true;
                        }
                    }

                    peer.WasConnected = peer.Connection.IsConnected;
                }
            }

            // Disconnect from no longer relevant peers if we are above ideal number.
            lock (Peers)
            {
                for (int i = 0; i < Peers.Count; i++)
                {
                    Peer peer = Peers[i];

                    bool ShouldRemove = false;

                    if (RelevantPeerAddresses != null)
                    {
                        bool Exists = false;

                        foreach (IPEndPoint EndPoint in RelevantPeerAddresses)
                        {
                            if (EndPoint.Equals(peer.Address))
                            {
                                Exists = true;
                                break;
                            }
                        }

                        if (!Exists)
                        {
                            ShouldRemove = true;
                        }
                    }

                    // Always remove peers that were remote initiated and are now closed as we won't be
                    // reinitiating their connection anytime soon.
                    if (peer.RemoteInitiated && !peer.Connection.IsConnected)
                    {
                        ShouldRemove = true;
                    }
                    /*else if (Peers.Count < IdealPeerCount)
                    {
                        // Don't bother removing peers if below ideal peer count, they may be useful later.
                        ShouldRemove = false;
                    }*/

                    if (ShouldRemove)
                    {
                        Console.WriteLine("Disconnecting from peer: {0}", peer.Address.ToString());

                        peer.Connection.OnMessageRecieved -= HandleMessage;

                        if (peer.Connection.IsConnected)
                        {
                            peer.Connection.Disconnect();
                        }

                        Peers.RemoveAt(i);
                        i--;

                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SendBlockListUpdate()
        {
            Console.WriteLine("Sending block list update.");

            BlockListState State = ManifestDownloadManager.GetBlockListState();

            NetMessage_BlockListUpdate Msg = new NetMessage_BlockListUpdate();
            Msg.BlockState = State;

            // Send to server and each peer.
            foreach (Peer peer in Peers)
            {
                if (peer.Connection.IsConnected)
                {
                    Console.WriteLine("\tSent to peer: " + peer.Connection.Address.ToString());
                    peer.Connection.Send(Msg);
                }
            }

            Console.WriteLine("\tSent to server.");
            Connection.Send(Msg);

            /*
            BlockListState DeltaEncoded = State;
            if (LastBlockListState != null)
            {
                DeltaEncoded = State.GetDelta(LastBlockListState);
            }

            NetMessage_BlockListUpdate Msg = new NetMessage_BlockListUpdate();
            Msg.BlockState = DeltaEncoded;

            LastBlockListState = DeltaEncoded;

            Connection.Send(Msg);
            */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        public bool RequestBuilds(string Path)
        {
            if (!Connection.IsConnected)
            {
                Console.WriteLine("Failed to request builds, no connection to server?");
                return false;
            }

            NetMessage_GetBuilds Msg = new NetMessage_GetBuilds();
            Msg.RootPath = Path;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RequestManifest(Guid Id)
        {
            if (!Connection.IsConnected)
            {
                Console.WriteLine("Failed to request manifests, no connection to server?");
                return false;
            }

            NetMessage_GetManifest Msg = new NetMessage_GetManifest();
            Msg.ManifestId = Id;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SendConnectionInfo()
        {
            if (!Connection.IsConnected)
            {
                return;
            }

            NetMessage_ConnectionInfo Msg = new NetMessage_ConnectionInfo();
            Msg.PeerConnectionAddress = ListenConnection.ListenAddress;
            Connection.Send(Msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        public bool PublishManifest(BuildManifest Manifest)
        {
            if (!Connection.IsConnected)
            {
                Console.WriteLine("Failed to publish, no connection to server?");
                return false;
            }

            byte[] Data = Manifest.ToByteArray();

            NetMessage_PublishManifest Msg = new NetMessage_PublishManifest();
            Msg.Data = Data;
            Msg.ManifestId = Manifest.Guid;
            Connection.Send(Msg);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="ClientConnection"></param>
        private void PeerConnected(NetConnection Connection, NetConnection ClientConnection)
        {
            Peer peer = null;
            lock (Peers)
            {
                peer = GetPeerByAddress(ClientConnection.Address);
                if (peer == null)
                {
                    Console.WriteLine("Peer connected from {0}.", ClientConnection.Address.ToString());

                    peer = new Peer();
                    peer.Address = ClientConnection.Address;
                    peer.Connection = ClientConnection;
                    peer.Connection.OnMessageRecieved += HandleMessage;
                    peer.RemoteInitiated = true;
                    peer.LastConnectionAttemptTime = 0;
                    Peers.Add(peer);
                }
            }

            // Exchange block list with remote.
            BlockListUpdatePending = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Message"></param>
        private void HandleMessage(NetConnection Connection, NetMessage BaseMessage)
        {
            // Server has sent us back a list of builds from a previous request.
            if (BaseMessage is NetMessage_GetBuildsResponse)
            {
                NetMessage_GetBuildsResponse Msg = BaseMessage as NetMessage_GetBuildsResponse;

                Console.WriteLine("Recieved builds in folder: {0}", Msg.RootPath);

                OnBuildsRecieved?.Invoke(Msg.RootPath, Msg.Builds);
            }

            // Server is giving us response to manifest publishing.
            else if (BaseMessage is NetMessage_PublishManifestResponse)
            {
                NetMessage_PublishManifestResponse Msg = BaseMessage as NetMessage_PublishManifestResponse;

                Console.WriteLine("Recieved response for manifest publish: {0}", Msg.Result.ToString());

                OnManifestPublishResultRecieved?.Invoke(Msg.ManifestId, Msg.Result);
            }

            // Server is sending us a list of peers that are relevant to our active downloads.
            else if (BaseMessage is NetMessage_RelevantPeerListUpdate)
            {
                NetMessage_RelevantPeerListUpdate Msg = BaseMessage as NetMessage_RelevantPeerListUpdate;

                Console.WriteLine("Recieved relevant peer list update.");

                RelevantPeerAddresses = Msg.PeerAddresses;
            }

            // Server is sending us a manifest we previously requested.
            else if (BaseMessage is NetMessage_GetManifestResponse)
            {
                NetMessage_GetManifestResponse Msg = BaseMessage as NetMessage_GetManifestResponse;

                Console.WriteLine("Recieved manifest: {0}", Msg.ManifestId.ToString());
                try
                {
                    BuildManifest Manifest = BuildManifest.FromByteArray(Msg.Data);
                    ManifestRegistry.RegisterManifest(Manifest);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to process manifest response due to error: {0}", ex.Message);
                }
            }

            // Recieved block list update from peer.
            else if (BaseMessage is NetMessage_BlockListUpdate)
            {
                NetMessage_BlockListUpdate Msg = BaseMessage as NetMessage_BlockListUpdate;

                Console.WriteLine("Recieved block list update from: {0}", Connection.Address.ToString());

                Peer peer = GetPeerByConnection(Connection);
                if (peer != null)
                {
                    peer.BlockState = Msg.BlockState;
                }

                UpdateAvailableBlocks();
            }
            
            // Peer requested a block of data from us.
            else if (BaseMessage is NetMessage_GetBlock)
            {
                NetMessage_GetBlock Msg = BaseMessage as NetMessage_GetBlock;

                //Console.WriteLine("Recieved request for block {0} in manifest {1} from {2}", Msg.BlockIndex, Msg.ManifestId.ToString(), Connection.Address.ToString());

                NetMessage_GetBlockResponse Response = new NetMessage_GetBlockResponse();
                Response.ManifestId = Msg.ManifestId;
                Response.BlockIndex = Msg.BlockIndex;

                ManifestDownloadManager.GetBlockData(Msg.ManifestId, Msg.BlockIndex, ref Response.Data, (bool bSuccess) => {

                    lock (DeferredActions)
                    {
                        DeferredActions.Enqueue(() =>
                        {
                            if (!bSuccess)
                            {
                                ManifestDownloadManager.MarkBlockAsUnavailable(Msg.ManifestId, Msg.BlockIndex);
                                Response.Data = null;
                            }

                            if (Connection.IsConnected)
                            {
                                Connection.Send(Response);
                            }
                        });
                    }

                });
            }

            // Peer provided block of data we requested.
            else if (BaseMessage is NetMessage_GetBlockResponse)
            {
                NetMessage_GetBlockResponse Msg = BaseMessage as NetMessage_GetBlockResponse;

                //Console.WriteLine("Recieved block {0} in manifest {1} from {2}", Msg.BlockIndex, Msg.ManifestId.ToString(), Connection.Address.ToString());

                ManifestDownloadManager.SetBlockData(Msg.ManifestId, Msg.BlockIndex, Msg.Data, (bool bSuccess) => {

                    lock (DeferredActions)
                    {
                        DeferredActions.Enqueue(() =>
                        {
                            // Mark block as complete.
                            if (bSuccess)
                            {
                                ManifestDownloadManager.MarkBlockAsComplete(Msg.ManifestId, Msg.BlockIndex);
                            }

                            // Remove active download marker for this block.
                            Peer peer = GetPeerByConnection(Connection);
                            if (peer != null)
                            {
                                for (int i = 0; i < peer.ActiveBlockDownloads.Count; i++)
                                {
                                    if (peer.ActiveBlockDownloads[i].BlockIndex == Msg.BlockIndex &&
                                        peer.ActiveBlockDownloads[i].ManifestId == Msg.ManifestId)
                                    {
                                        peer.ActiveBlockDownloads.RemoveAt(i);
                                        break;
                                    }
                                }
                            }
                        });
                    }

                });
            }
        }
    }
}
