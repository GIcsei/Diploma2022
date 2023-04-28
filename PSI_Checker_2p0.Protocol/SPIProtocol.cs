using System;

namespace PSI_Checker_2p0.Protocol
{
    public class SPIProtocol : BaseProtocol
    {
        private int syncDistance;
        private int bitLength;
        private bool syncType;
        private int[] startTimesLogical;
        private double[] startTimes;
        public int DataRegionA { get; set; }
        public int DataRegionB { get; private set; } = 0;

        private readonly double FREQ = 18; // MHz

        public SPIProtocol()
        {
            BitTime = 1000 / (double)BitRate;
            Name = "SPI";
        }

        public int Sync_Dist
        {
            get => syncDistance;
            set
            {
                if (value < 0)
                    throw new ArgumentException();
                syncDistance = value;
            }
        }
        public int Bit_Len
        {
            get => bitLength;
            set
            {
                if (value < 0) throw new ArgumentException();
                bitLength = value;
            }
        }
        public bool Sync
        {
            get => syncType;
            set => syncType = value;
        }

        /// <summary>
        /// Checks if the given protocol settings can be realised.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public bool ConsistencyCheck()
        {
            //TODO !
            // Consistency check
            return true;
        }

        /// <summary>
        /// According to the class properties,
        /// creates the string format of the protocol.
        /// </summary>
        /// <returns>The string format of the protocol</returns>
        /// <exception cref="Exception"></exception>
        public override string ToString()
        {
            string coding = String.Empty;
            return coding;
        }

        /// <summary>
        /// Calculates the expected checksum for the decoded data.
        /// </summary>
        /// <param name="word">The decoded PSI5Protocol data</param>
        /// <returns>The expected checksum.</returns>
        /// <exception cref="ArgumentException"></exception>
        public override bool CheckData(long word)
        {
            return true;
        }

        public override byte EncodeCommand(int data)
        {
            throw new NotImplementedException();
        }

        public override int DecodeCommand(byte data)
        {
            throw new NotImplementedException();
        }
    }
}