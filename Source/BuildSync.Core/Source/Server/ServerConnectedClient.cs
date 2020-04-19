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
    public class ServerConnectedClient
    {
        /// <summary>
        /// 
        /// </summary>
        public long BandwidthLimit;

        /// <summary>
        /// 
        /// </summary>
        public BlockListState BlockState;

        /// <summary>
        /// 
        /// </summary>
        public int ConnectedPeerCount;

        /// <summary>
        /// 
        /// </summary>
        public long DiskUsage;

        /// <summary>
        /// 
        /// </summary>
        public long DownloadRate;

        /// <summary>
        /// 
        /// </summary>
        public IPEndPoint PeerConnectionAddress;

        /// <summary>
        /// 
        /// </summary>
        public bool PermissionsNeedUpdate;

        /// <summary>
        /// 
        /// </summary>
        public List<NetConnection> RelevantPeers = new List<NetConnection>();

        /// <summary>
        /// 
        /// </summary>
        public bool RelevantPeerAddressesNeedUpdate;

        /// <summary>
        /// 
        /// </summary>
        public long TotalDownloaded;

        /// <summary>
        /// 
        /// </summary>
        public long TotalUploaded;

        /// <summary>
        /// 
        /// </summary>
        public long UploadRate;

        /// <summary>
        /// 
        /// </summary>
        public string Username = "";

        /// <summary>
        /// 
        /// </summary>
        public string Version = "";

        /// <summary>
        /// 
        /// </summary>
        public int VersionNumeric = 0;
    }

}