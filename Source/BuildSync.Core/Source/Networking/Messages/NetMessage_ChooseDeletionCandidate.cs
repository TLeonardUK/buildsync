using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSync.Core.Networking.Messages
{
    /// <summary>
    ///     Client->Server
    ///
    ///     Sent to the server to ask it to select the best candidate to delete to reduce disk space usage.
    /// </summary>
    public class NetMessage_ChooseDeletionCandidate : NetMessage
    {
        /// <summary>
        ///     List of potentital candidates that the server can delete, listed
        ///     in clients priority order.
        /// </summary>
        public List<Guid> CandidateManifestIds = new List<Guid>();

        /// <summary>
        ///     Serializes the payload of this message to a memory buffer.
        /// </summary>
        /// <param name="serializer">Serializer to read/write payload to.</param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            int Count = CandidateManifestIds.Count;
            serializer.Serialize(ref Count);

            for (int i = 0; i < Count; i++)
            {
                if (serializer.IsLoading)
                {
                    CandidateManifestIds.Add(new Guid());
                }

                Guid Id = CandidateManifestIds[i];
                serializer.Serialize(ref Id);
                CandidateManifestIds[i] = Id;
            }
        }
    }
}
