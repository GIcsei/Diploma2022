using PSI_Checker_2p0.Enums;
using System;
using System.Collections;

namespace PSI_Checker_2p0.Decoder
{
    /// <summary>
    /// Encapsulates the state change and it's index in a class.
    /// This class can be used for Manchester-decoding.
    /// </summary>
    public class TimestampData : IEnumerable
    {
        private int[] _indexStamp; // Contains the indexes of state change.
        private States_Enum[] _state;

        private readonly uint maxSize;
        private uint currentStateSize;
        private uint currentIndexStampSize;

        public uint Length => currentStateSize;
        public States_Enum this[int key]
        {
            get => _state[key];
        }

        public int GetTimeStampAt(int index)
        {
            return _indexStamp[index];
        }

        /// <summary>
        /// Generates the collection.
        /// </summary>
        /// <param name="size">Sets the maximum size for the arrays. This is the expected bit number in the decoded data.</param>
        public TimestampData(uint size)
        {
            _indexStamp = new int[size];
            _state = new States_Enum[size];
            currentIndexStampSize = 0;
            currentStateSize = 0;
            maxSize = size;
        }

        public void AddState(States_Enum new_state, int index)
        {
            _state[++currentStateSize] = new_state;
            AddIndexStamp(index);
            if (currentStateSize == maxSize)
            {
                throw new InvalidOperationException();
            }
        }

        private void AddIndexStamp(int new_index)
        {
            _indexStamp[++currentIndexStampSize] = new_index;
            if (currentIndexStampSize == maxSize)
            {
                throw new IndexOutOfRangeException();
            }
        }

        public IEnumerator GetEnumerator() => new TimeStampEnumerator(_state, currentStateSize);

        private class TimeStampEnumerator : IEnumerator
        {
            private States_Enum[] _state;
            private readonly uint maxSize;
            private int position = -1;
            private int stepSize = 1;

            public TimeStampEnumerator(States_Enum[] state, uint length)
            {
                _state = state;
                this.maxSize = length;
            }

            public object Current
            {
                get
                {
                    try
                    {
                        return _state[position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            private void SetStepSize()
            {
                try
                {
                    stepSize = _state[position].Equals(States_Enum.MC_BRAKE0) && _state[position + 1].Equals(States_Enum.MC_SHORT1) ? 2 : 1;
                }
                catch (IndexOutOfRangeException)
                {
                    stepSize = 1;
                }
            }

            public bool MoveNext()
            {
                SetStepSize();
                position += stepSize;
                return (position < maxSize);
            }

            public void Reset()
            {
                position = -1;
                stepSize = 1;
            }
        }
    }
}