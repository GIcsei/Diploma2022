using PSI_Checker_2p0.Enums;
using System;
using System.Collections.Generic;

namespace PSI_Checker_2p0.Decoder
{
    internal class Psi5_Analog2DigitalConverter : Analog2DigitalConverter
    {
        public Psi5_Analog2DigitalConverter(double CompareLevel, uint SamplingRate, int Bitrate, uint Datalength) : base(CompareLevel, SamplingRate, Bitrate)
        {
            this.DataLength = Datalength;
        }

        public override bool CanBeDecoded()
        {
            return DigitalData.Length >= SamplePerBitTime * DataLength;
        }

        public override int[] Decode()
        {
            if (!CanBeDecoded())
            {
                throw new InvalidOperationException();
            }
            PopulateTimeStampData();
            if (!DecodingDone)
            {
                throw new Exception();
            }
            return DecodeTimeStamp();
        }

        private int[] DecodeTimeStamp()
        {
            List<int> result = new List<int>();
            int data = 0, bitpos = (int)DataLength;
            foreach (States_Enum state in timestampData)
            {
                if (state.Equals(States_Enum.MC_EMPTY))
                {
                    continue;
                }

                if (state.Equals(States_Enum.MC_BRAKE0))
                {
                    data = 0;
                    bitpos = (int)DataLength;
                }
                else
                {
                    data |= (int)state << bitpos;
                    bitpos--;
                }

                if (bitpos < 0) // Check if all bit position has been checked in the frame
                {
                    result.Add(data);
                }
            }
            return result.ToArray();
        }

        private void PopulateTimeStampData()
        {
            timestampData = new TimestampData((uint)(DataRate / SamplingRate * DigitalData.Length));

            int actualSample, lastSample = DigitalData[0];
            int stateLength = 0;
            int minimumFrameGapSample = 0; // Minimum frame gap in samples
            int minimumBitTimeSample = 0;
            States_Enum currentState;
            bool Duplicate = false;

            for (var index = 0; index < DigitalData.Length; index++)
            {
                actualSample = DigitalData[index];
                if (actualSample != lastSample)
                {
                    if (stateLength >= minimumFrameGapSample) // Check if we are between two frames
                    {
                        currentState = lastSample == 0 ? States_Enum.MC_BRAKE0 : States_Enum.MC_BRAKE1;
                    }
                    else
                    {
                        if (stateLength >= minimumBitTimeSample)
                        {
                            currentState = lastSample == 0 ? States_Enum.MC_LONG0 : States_Enum.MC_LONG1;
                            Duplicate = true;
                        }
                        else
                        {
                            currentState = lastSample == 0 ? States_Enum.MC_SHORT0 : States_Enum.MC_SHORT1;
                        }
                    }
                    timestampData.AddState(currentState, index);
                    if (Duplicate)
                    {
                        Duplicate = false;
                        timestampData.AddState(currentState, index);
                    }
                    stateLength = 0;
                }
                stateLength++;
                lastSample = actualSample;
            }

            timestampData.AddState(States_Enum.MC_EMPTY, DigitalData.Length - 1);
            DecodingDone = true;
        }

        public override void Digitalize(double[] data)
        {
            data.CopyTo(AnalogData, 0);
            for (var index = 0; index < data.Length; index++)
            {
                DigitalData[index] = (byte)(data[index] >= ComparisonLevel ? 1 : 0);
            }
        }
    }
}
