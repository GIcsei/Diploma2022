using PSI_Checker_2p0.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace PSI_Checker_2p0.Protocol
{
    public class PSI5Protocol : BaseProtocol
    {
        private int syncDistance;
        private int bitLength;
        private bool syncType;
        private int[] startTimesLogical;
        private double[] startTimes;
        private int dataRegionA;
        private int dataRegionB;
        private PSI5Protocol BiDir;
        public int DataRegionA { get; set; }
        public int DataRegionB { get; private set; } = 0;

        private readonly double FREQ = 18; // MHz

        public PSI5Protocol(bool withBidir = true)
        {
            syncDistance = PSI5_DEFAULT.SYNC_DIST;
            bitLength = PSI5_DEFAULT.BIT_LEN;
            BitRate = PSI5_DEFAULT.BITRATE;
            CheckSum = PSI5_DEFAULT.CHECK_TYPE;
            syncType = PSI5_DEFAULT.SYNC;
            startTimesLogical = PSI5_DEFAULT.START_TIMES;
            BitTime = 1000 / (double)BitRate;
            Name = "PSI5";
            if (withBidir) // To prevent stack overflow
                BiDir = new PSI5Protocol(false);
        }

        /// <summary>
        /// Creates protocol setting from string description.
        /// </summary>
        /// <param name="_type"></param>
        /// <exception cref="ArgumentException"></exception>
        public PSI5Protocol(string _type)
        {
            _type = _type.ToUpper();
            string[] splitted = _type.Split('_');
            if (splitted.Length != 3)
            {
                throw new ArgumentException("Invalid input format, please check tooltip!");
            }
            switch (splitted[0][0])
            {
                case 'A':
                    syncType = false;
                    break;
                case 'P':
                    syncType = true;
                    break;
                default:
                    throw new ArgumentException("No match for sync argument!");
            }
            Match bitlen = Regex.Match(splitted[0], @"\d+");
            bitLength = Int32.Parse(bitlen.Value);
            switch (splitted[0].Substring(bitlen.Length + 1))
            {
                case "CRC":
                    CheckSum = CHECKSUM.CRC;
                    break;
                case "P":
                    CheckSum = CHECKSUM.PARITY;
                    break;
                default:
                    throw new ArgumentException("No match for check Type argument!");
            }
            syncDistance = Int32.Parse(splitted[1]);
            switch (splitted[2][1])
            {
                case 'H':
                    BitRate = Constans.HIGH;
                    break;
                case 'L':
                    BitRate = Constans.LOW;
                    break;
                default:
                    throw new ArgumentException("No match for bitrate argument!");
            }
            BitTime = 1000 / (double)BitRate;
            CreateStartTimes(Int32.Parse(splitted[2][0].ToString()));
            ConsistencyCheck();
        }

        public PSI5Protocol(string _type, int[] startTimes)
        {
            _type = _type.ToUpper();
            string[] splitted = _type.Split('_');
            if (splitted.Length != 3)
            {
                throw new ArgumentException("Invalid input format, please check tooltip!");
            }
            switch (splitted[0][0])
            {
                case 'A':
                    syncType = false;
                    break;
                case 'P':
                    syncType = true;
                    break;
                default:
                    throw new ArgumentException("No match for sync argument!");
            }
            Match bitlen = Regex.Match(splitted[0], @"\d+");
            bitLength = Int32.Parse(bitlen.Value);
            switch (splitted[0].Substring(bitlen.Length + 1))
            {
                case "CRC":
                    CheckSum = CHECKSUM.CRC;
                    break;
                case "P":
                    CheckSum = CHECKSUM.PARITY;
                    break;
                default:
                    throw new ArgumentException("No match for check Type argument!");
            }
            syncDistance = Int32.Parse(splitted[1]);
            switch (splitted[2][1])
            {
                case 'H':
                    BitRate = Constans.HIGH;
                    break;
                case 'L':
                    BitRate = Constans.LOW;
                    break;
                default:
                    throw new ArgumentException("No match for bitrate argument!");
            }
            BitTime = 1000 / (double)BitRate;
            int numberOfStartTimes = Int32.Parse(splitted[2][0].ToString());
            int realNumberOfStartTimes = startTimes.Count((x) => x > 0);
            startTimesLogical = new int[numberOfStartTimes > realNumberOfStartTimes ? realNumberOfStartTimes : numberOfStartTimes];
            int realIndex = 0;
            for (int index = 0; index < numberOfStartTimes; index++)
            {
                if (startTimes[index] != 0)
                {
                    startTimesLogical[realIndex] = (int)((startTimes[index] - Constans.DELTA_T) * FREQ);
                    realIndex++;
                }
            }
            ConsistencyCheck();
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

        public int[] Start_Times
        {
            get => startTimesLogical;
            set
            {
                if (value.Min() < 0)
                {
                    throw new ArgumentException("One of the arguments is negative!");
                }
                if (value.Length > Constans.MAX_START_TIMES)
                    throw new ArgumentException("Start time is out of frame!");
                List<int> _start_times = new List<int>();
                foreach (int start in value)
                {
                    if (start != 0)
                    {
                        _start_times.Add(start);
                    }
                }
                startTimesLogical = _start_times.ToArray();
            }
        }

        public int GetStartTimeAt(uint index) => startTimesLogical[index];

        private void CreateStartTimes(int NumOfStartTimes)
        {
            startTimesLogical = new int[NumOfStartTimes];
            int dataLength = 2 + bitLength;
            if (CheckSum == CHECKSUM.PARITY)
                dataLength += 1;
            else
                dataLength += 3;
            double dataTime = (dataLength + 1) * BitTime;
            double startTime = Constans.T_START;
            for (int index = 0; index < NumOfStartTimes; index++)
            {
                startTimesLogical[index] = (int)((startTime - Constans.DELTA_T) * FREQ);
                startTime += dataTime + 4 * BitTime;
            }
        }

        /// <summary>
        /// Checks if the given protocol settings can be realised.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public bool ConsistencyCheck()
        {
            //TODO !
            // Consistency check
            startTimes = new double[startTimesLogical.Length];
            int dataLength = 2 + bitLength;
            if (CheckSum == CHECKSUM.PARITY)
                dataLength += 1;
            else
                dataLength += 3;
            double dataTime = (dataLength + 1) * BitTime;
            double _start, _prevStart;
            for (int index = 0; index < startTimesLogical.Length - 1; index++)
            {
                _start = (int)(startTimesLogical[index + 1] / FREQ) + Constans.DELTA_T;
                _prevStart = (int)(startTimesLogical[index] / FREQ) + Constans.DELTA_T;
                if (_start > syncDistance - BitTime)
                {
                    return false;
                }
                if (_start + 0.2 < _prevStart + dataTime)
                {
                    return false;
                }
                startTimes[index] = _prevStart;
            }

            if (!syncType)
            {
                startTimesLogical = new int[] { startTimesLogical[1] };
                bitLength = 10;
                CheckSum = CHECKSUM.PARITY;
                syncDistance = 250;
            }

            this.dataLength = dataLength;
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
            try
            {
                if (syncType)
                    coding += "P";
                else
                    coding += "A";
                coding += bitLength.ToString();
                if (CheckSum != CHECKSUM.PARITY)
                    coding += "CRC_";
                else
                    coding += "P_";
                coding += syncDistance.ToString() + "_" + startTimesLogical.Length.ToString();
                if (BitRate == Constans.HIGH)
                    coding += "H";
                else
                    coding += "L";
            }
            catch (Exception)
            {
                throw new Exception("Coding cannot be created because some value does not exist!\n");
            }
            return coding;
        }

        /// <summary>
        /// Check if the check bit(s) are correct in the decoded data.
        /// </summary>
        /// <param name="word">The decoded PSI5Protocol data</param>
        /// <returns>True, if the checksum is OK, otherwise false</returns>
        public override bool CheckData(long word)
        {
            if (this.CheckSum.Equals(CHECKSUM.CRC))
            {
                int crc1 = 0x01;
                int crc2 = 0x01;
                int crc3 = 0x01;
                int hbit;
                int value;

                for (var bit = 0; bit < this.dataLength; bit++)
                {
                    value = (int)((word >> bit) & 0x01);
                    hbit = crc3;
                    crc3 = crc2;
                    crc2 = hbit ^ crc1;
                    crc1 = hbit ^ value;
                }

                return (crc1 | crc2 | crc3) == 1;
            }
            else
            {
                long parity = 0;
                for (var bit = 0; bit < this.dataLength; bit++)
                {
                    parity ^= 0x01 & (word >> bit);
                }
                return (parity & 0x1) == 1;
            }
        }

        /// <summary>
        /// Calculates the expected checksum for the decoded data.
        /// </summary>
        /// <param name="word">The decoded PSI5Protocol data</param>
        /// <returns>The expected checksum.</returns>
        /// <exception cref="ArgumentException"></exception>
        public int Calculate_CheckData(long word)
        {
            long result = word >> this.dataLength;
            // Check if the check bit(s) are correct in the decoded data
            if (this.CheckSum.Equals(CHECKSUM.CRC))
            {
                switch (result)
                {
                    case 1: return 4;
                    case 4: return 1;
                    case 6: return 3;
                    case 3: return 6;
                    default: throw new ArgumentException();
                }
            }
            return (int)result;
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