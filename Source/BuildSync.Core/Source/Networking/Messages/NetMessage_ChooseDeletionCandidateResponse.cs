using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    ///     Server->Client
    ///
    ///     Sent by the server in response to a <see cref="NetMessage_ChooseDeletionCandidate" /> 
    ///     confirming which manifest the server thinks is the best choice for the client to delete.
    /// </summary>
    public class NetMessage_ChooseDeletionCandidateResponse : NetMessage
    {
        /// <summary>
        ///     Id of manifest to be removed.
        /// </summary>
        public Guid ManifestId;

        /// <summary>
        ///     Serializes the payload of this message to a memory buffer.
        /// </summary>
        /// <param name="serializer">Serializer to read/write payload to.</param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.Serialize(ref ManifestId);
        }
    }
}
