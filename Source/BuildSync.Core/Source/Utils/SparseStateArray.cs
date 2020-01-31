//#define DO_VALIDATION

using System;
using System.Collections.Generic;
using System.Text;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SparseStateArray
    {
        [Serializable]
        public struct Range
        {
            public int Start { get; set; }
            public int End { get; set; }
            public bool State { get; set; }
        };

        public List<Range> Ranges { get; set; }
        public int Size { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public void Validate()
        {
            int LastEnd = 0;
            foreach (Range range in Ranges)
            {
                if (range.Start < LastEnd)
                {
                    throw new InvalidOperationException("Sparse array has failed validation, something got corrupt ...");
                }
                LastEnd = range.End;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool[] ToArray()
        {
            bool[] Result = new bool[Size];
            foreach (Range range in Ranges)
            {
                for (int i = range.Start; i <= range.End; i++)
                {
                    Result[i] = range.State;
                }
            }
            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void ToArray(ref bool[] Result)
        {
            if (Result.Length != Size)
            {
                Array.Resize(ref Result, Size);
            }
            foreach (Range range in Ranges)
            {
                for (int i = range.Start; i <= range.End; i++)
                {
                    Result[i] = range.State;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Array"></param>
        public void FromArray(bool[] Array)
        {
            if (Size != Array.Length)
            {
                Resize(Array.Length);
            }

            Ranges.Clear();

            for (int RangeStart = 0; RangeStart < Array.Length; )
            {
                int RangeEnd = RangeStart;

                for (; RangeEnd < Size; RangeEnd++)
                {
                    if (Array[RangeStart] != Array[RangeEnd])
                    {
                        break;
                    }
                }

                Range Range = new Range
                {
                    Start = RangeStart,
                    End = RangeEnd - 1,
                    State = Array[RangeStart]
                };

                Ranges.Add(Range);

                RangeStart = RangeEnd;
            }

#if DO_VALIDATION
            Validate();
#endif
        }

        /// <summary>
        ///     BE CAREFUL, THIS DOESNT MAINTAIN ORDERING
        /// </summary>
        /// <param name="Array"></param>
        public void AddArray(bool[] Array, int Offset, int Length)
        {
            if (Ranges == null)
            {
                Ranges = new List<Range>();
            }

            for (int RangeStart = Offset; RangeStart < Offset + Length; )
            {
                int RangeEnd = RangeStart;

                for (; RangeEnd < Offset + Length; RangeEnd++)
                {
                    if (Array[RangeStart] != Array[RangeEnd])
                    {
                        break;
                    }
                }

                Range Range = new Range
                {
                    Start = RangeStart,
                    End = RangeEnd - 1,
                    State = Array[RangeStart]
                };

                Ranges.Add(Range);

                RangeStart = RangeEnd;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="State"></param>
        /// <returns></returns>
        public bool AreAllSet(bool State)
        {
            // If all are set we should only have a single range.
            if (Ranges.Count != 1)
            {
                return false;
            }

            return 
                Ranges[0].Start == 0 && 
                Ranges[0].End == Size - 1 && 
                Ranges[0].State == State;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Other"></param>
        public void Union(SparseStateArray Other)
        {
            // Expand to new size.
            if (Size < Other.Size)
            {
                Resize(Other.Size);
            }

            // This is shit, do it in-place.
            bool[] Array = ToArray();
            bool[] OtherArray = Other.ToArray();
            bool[] Union = new bool[Size];
            for (int i = 0; i < Other.Size; i++)
            {
                Union[i] = Array[i] || OtherArray[i];
            }

            FromArray(Union);

#if DO_VALIDATION
            Validate();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SparseStateArray Clone()
        {
            SparseStateArray Result = new SparseStateArray();
            Result.Size = Size;
            Result.Ranges = null;

            if (Ranges != null)
            {
                Result.Ranges = new List<Range>();
                foreach (Range range in Ranges)
                {
                    Result.Ranges.Add(new Range { Start = range.Start, End = range.End, State = range.State });
                }
            }

            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        public void CompactRanges()
        {
            for (int i = 0; i < Ranges.Count - 1; i++)
            {
                Range Range = Ranges[i];
                Range NextRange = Ranges[i + 1];

                if (Range.State == NextRange.State)
                {
                    Range.End = NextRange.End;
                    Ranges[i] = Range;

                    Ranges.RemoveAt(i + 1);
                    i--;
                }
            }

#if DO_VALIDATION
            Validate();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public int Count(bool State)
        {
            int Result = 0;
            foreach (Range range in Ranges)
            {
                if (range.State == State)
                {
                    Result += (range.End - range.Start) + 1;
                }
            }
            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <returns></returns>
        public bool IsAnyInRangeSet(int Start, int End)
        {
            foreach (Range range in Ranges)
            {
                // If the range start is beyond the end of the range we are interested in, we 
                // can early exit, we're done.
                if (End < range.Start)
                {
                    break;
                }

                if (range.State)
                {
                    if (!(range.End < Start || range.Start > End))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Size"></param>
        public void Resize(int NewSize)
        {
            // If new size is less than current, reduce ranges until we are inside size.
            if (NewSize < Size)
            {
                for (int i = 0; i < Ranges.Count; i++)
                {
                    Range range = Ranges[i];

                    // Greater than size, remove.
                    if (range.Start >= NewSize)
                    {
                        Ranges.RemoveAt(i);
                        i--;
                        continue;
                    }

                    // Straddling the new size, trim and we are done.
                    else if (range.End >= NewSize)
                    {
                        range.End = NewSize - 1;
                        Ranges[i] = range;

                        break;
                    }
                }
            }

            // If new size is greater than current, increase last range (if state is false), or add new range.
            else if (NewSize > Size)
            {
                if (Ranges == null)
                {
                    Ranges = new List<Range>();
                }
               
                if (Ranges.Count == 0)
                {
                    Ranges.Add(new Range { Start = 0, End = NewSize - 1, State = false });
                }
                else
                {
                    Range LastRange = Ranges[Ranges.Count - 1];
                    if (LastRange.State == false)
                    {
                        LastRange.End = NewSize - 1;
                        Ranges[Ranges.Count - 1] = LastRange;
                    }
                    else
                    {
                        Ranges.Add(new Range { Start = LastRange.End + 1, End = NewSize - 1, State = false });
                    }
                }
            }

            Size = NewSize;

#if DO_VALIDATION
            Validate();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BlockIndex"></param>
        /// <param name="NewState"></param>
        public void Set(int BlockIndex, bool NewState, bool DoNotCompact = false)
        {
            if (BlockIndex >= Size)
            {
                Resize(BlockIndex + 1);
            }

            for (int i = 0; i < Ranges.Count; i++)
            {
                Range range = Ranges[i];
                if (BlockIndex >= range.Start && BlockIndex <= range.End)
                {
                    if (range.State != NewState)
                    {
                        int EndOffset = 1;

                        // Start block
                        if (range.Start < BlockIndex)
                        {
                            Ranges.Insert(i, new Range { Start = range.Start, End = BlockIndex - 1, State = range.State });
                            EndOffset++;
                        }

                        // End block
                        if (range.End > BlockIndex)
                        {
                            Ranges.Insert(i + EndOffset, new Range { Start = BlockIndex + 1, End = range.End, State = range.State });
                        }

                        // Remap this range as middle.                        
                        range.Start = BlockIndex;
                        range.End = BlockIndex;
                        range.State = NewState;

                        Ranges[i + EndOffset - 1] = range;
                    }

                    break;
                }
            }

            if (!DoNotCompact)
            {
                CompactRanges();
            }

#if DO_VALIDATION
            Validate();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BlockIndex"></param>
        /// <param name="State"></param>
        public bool Get(int BlockIndex)
        {
            foreach (Range range in Ranges)
            {
                if (BlockIndex >= range.Start && BlockIndex <= range.End)
                {
                    return range.State;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="State"></param>
        public void SetAll(bool InState)
        {
            Ranges.Clear();
            Ranges.Add(new Range { Start = 0, End = Size - 1, State = InState });
        }
    }
}
