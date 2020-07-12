using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Downloads;

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
        ///     Which heuristic to use for selecting deletion candidate.
        /// </summary>
        public ManifestStorageHeuristic Heuristic = ManifestStorageHeuristic.OldestInLargestContainer;

        /// <summary>
        ///     Build tag id's to prioritize keeping.
        /// </summary>
        public List<Guid> PrioritizeKeepingTagIds = new List<Guid>();

        /// <summary>
        ///     Build tag id's to prioritize deleting.
        /// </summary>
        public List<Guid> PrioritizeDeletingTagIds = new List<Guid>();

        /// <summary>
        ///     Serializes the payload of this message to a memory buffer.
        /// </summary>
        /// <param name="serializer">Serializer to read/write payload to.</param>
        protected override void SerializePayload(NetMessageSerializer serializer)
        {
            serializer.SerializeList(ref CandidateManifestIds);

            if (serializer.Version > 100000560)
            {
                serializer.SerializeEnum<ManifestStorageHeuristic>(ref Heuristic);
            }

            if (serializer.Version >= 100000603)
            {
                serializer.SerializeList(ref PrioritizeKeepingTagIds);
                serializer.SerializeList(ref PrioritizeDeletingTagIds);
            }
        }
    }
}
