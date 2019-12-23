using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using BuildSync.Core.Manifests;
using BuildSync.Core.Networking;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Downloads
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ManifestBlockListState
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id;

        /// <summary>
        /// 
        /// </summary>
        public SparseStateArray BlockState;

        /// <summary>
        /// 
        /// </summary>
        public bool IsActive;
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BlockListState
    {
        /// <summary>
        /// 
        /// </summary>
        public ManifestBlockListState[] States = new ManifestBlockListState[0];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Base"></param>
        /// <returns></returns>
        public BlockListState GetDelta(BlockListState Base)
        {
            // Todo: something ...
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Base"></param>
        /// <returns></returns>
        public BlockListState ApplyDelta(BlockListState Patch)
        {
            // Todo: something ...
            return Patch;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Base"></param>
        /// <returns></returns>
        public void Union(BlockListState Other)
        {
            foreach (ManifestBlockListState OtherState in Other.States)
            {
                ManifestBlockListState OwnState = GetStateById(OtherState.Id);
                if (OwnState == null)
                {
                    OwnState = new ManifestBlockListState();
                    OwnState.Id = OtherState.Id;
                    OwnState.IsActive = true;
                    OwnState.BlockState = OtherState.BlockState.Clone();

                    Array.Resize(ref States, States.Length + 1);
                    States[States.Length - 1] = OwnState;
                }
                else
                {
                    OwnState.BlockState.Union(OtherState.BlockState);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        private ManifestBlockListState GetStateById(Guid Id)
        {
            foreach (ManifestBlockListState State in States)
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
        /// <param name="NeededBlocks"></param>
        /// <returns></returns>
        public bool HasAnyBlocksNeeded(BlockListState NeededBlocks)
        {
            foreach (ManifestBlockListState NeededState in NeededBlocks.States)
            {
                if (NeededState.IsActive)
                {
                    ManifestBlockListState OurState = GetStateById(NeededState.Id);
                    if (OurState != null)
                    {
                        foreach (SparseStateArray.Range NeededRange in NeededState.BlockState.Ranges)
                        {
                            if (!NeededRange.State)
                            {
                                if (OurState.BlockState.IsAnyInRangeSet(NeededRange.Start, NeededRange.End))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        public void Serialize(NetMessageSerializer serializer)
        {
            int StateCount = States.Length;
            serializer.Serialize(ref StateCount);

            Array.Resize(ref States, StateCount);

            for (int i = 0; i < StateCount; i++)
            {
                if (serializer.IsLoading)
                {
                    States[i] = new ManifestBlockListState();
                }

                ManifestBlockListState State = States[i];
                serializer.Serialize(ref State.Id);
                serializer.Serialize(ref State.IsActive);

                if (State.BlockState.Ranges == null)
                {
                    State.BlockState.Ranges = new List<SparseStateArray.Range>();
                }

                int RangeCount = State.BlockState.Ranges.Count;
                serializer.Serialize(ref RangeCount);
                serializer.Serialize(ref State.BlockState.Size);

                for (int j = 0; j < RangeCount; j++)
                {
                    if (serializer.IsLoading)
                    {
                        State.BlockState.Ranges.Add(new SparseStateArray.Range());
                    }
                    SparseStateArray.Range Range = State.BlockState.Ranges[j];
                    serializer.Serialize(ref Range.Start);
                    serializer.Serialize(ref Range.End);
                    serializer.Serialize(ref Range.State);
                }
            }
        }
    }
}