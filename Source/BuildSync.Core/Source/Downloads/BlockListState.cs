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
using BuildSync.Core.Networking;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Downloads
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class ManifestBlockListState
    {
        /// <summary>
        /// </summary>
        public SparseStateArray BlockState = new SparseStateArray();

        /// <summary>
        /// </summary>
        public Guid Id;

        /// <summary>
        /// </summary>
        public bool IsActive;
    }

    /// <summary>
    /// </summary>
    [Serializable]
    public class BlockListState
    {
        /// <summary>
        /// </summary>
        public ManifestBlockListState[] States = new ManifestBlockListState[0];

        /// <summary>
        /// </summary>
        /// <param name="Base"></param>
        /// <returns></returns>
        public BlockListState ApplyDelta(BlockListState Patch)
        {
            // Todo: something ...
            return Patch;
        }

        /// <summary>
        /// </summary>
        /// <param name="Base"></param>
        /// <returns></returns>
        public BlockListState GetDelta(BlockListState Base)
        {
            // Todo: something ...
            return this;
        }

        /// <summary>
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

                int Size = State.BlockState.Size;
                serializer.Serialize(ref Size);
                State.BlockState.Size = Size;

                for (int j = 0; j < RangeCount; j++)
                {
                    if (serializer.IsLoading)
                    {
                        State.BlockState.Ranges.Add(new SparseStateArray.Range());
                    }

                    SparseStateArray.Range Range = State.BlockState.Ranges[j];

                    int TempStart = Range.Start;
                    serializer.Serialize(ref TempStart);
                    Range.Start = TempStart;

                    int TempEnd = Range.End;
                    serializer.Serialize(ref TempEnd);
                    Range.End = TempEnd;

                    bool TmpState = Range.State;
                    serializer.Serialize(ref TmpState);
                    Range.State = TmpState;

                    State.BlockState.Ranges[j] = Range;
                }
            }
        }

        /// <summary>
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
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ManifestBlockListState GetStateById(Guid Id)
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
    }
}