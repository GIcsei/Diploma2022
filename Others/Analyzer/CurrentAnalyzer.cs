using PSI_Checker_2p0.Decoder;

namespace PSI_Checker_2p0.Checker
{
    public class CurrentAnalyzer
    {
        private readonly int DataMask;

        private readonly int[] words;
        private readonly TimestampData timestamps;

        private readonly double SampleRate;

        public CurrentAnalyzer()
        {

        }
        /*
        public CurrentAnalyzer(PSI5Protocol currentProtocol, IResultData<BaseDataContainer> resultData, int[] words, TimestampData timestamps, double SampleRate) :
            base(resultData, new PSI_Data(), currentProtocol)
        {
            this.words = words;
            this.timestamps = timestamps;
            this.SampleRate = SampleRate;
            DataMask = (int)(Math.Pow(2, currentProtocol.DataRegionA) - 1);
        }

        public override IResultData<BaseDataContainer> Analyze(double[] AnalogCurrentData, double[] AnalogVoltageData)
        {
            if(AnalogCurrentData.Length != AnalogVoltageData.Length)
            {
                throw new ArgumentException();
            }
            int current_frame_index = 0, current_word;
            foreach (var index in GetStartOfFrames()) // Index is always the MC_BRAKE0
            {
                current_word =  words[current_frame_index];
                var TBit_max = Calculate_TBitMax(index);
                var TBit_min= Calculate_TBitMin(index);
                var nominalTBit = (TBit_max + TBit_min) / 4;

                CalculateGapTime(index, TBit_min);
                CalculateStatistics(index, AnalogCurrentData, AnalogVoltageData, nominalTBit);
                CalculateWords(current_frame_index);

                data.AddValue("Par_CRC", (Protocol as PSI5Protocol).Calculate_CheckData(current_word));
                data.AddValue("TBit_Min", TBit_min);
                data.AddValue("TBit_Max", TBit_max);
                data.AddValue("StartTime", timestamps.GetTimeStampAt(index + 1) - nominalTBit / 1_000_000);
                data.AddValue("First_FallEdge", timestamps.GetTimeStampAt(index + 2));

                current_frame_index++;
            }

            ResultData.AddData(data);
            return ResultData;
        }

        private void CalculateWords(int current_frame_index)
        {
            var word = words[current_frame_index] & DataMask;
            double word2 = 0;
            double word3 = 0;
            if((Protocol as PSI5Protocol).DataRegionB == 4)
            {
                word3 = (word >> 3) & 0x1;
                word2 = word & 0x7;
                word = (word >> 4) & DataMask;
            }
            if((Protocol as PSI5Protocol).DataRegionB == 10)
            {
                word2 = (word >> 10) & DataMask;
                if(word2 > Math.Pow(2,9)-1)
                    word2 -= (int)Math.Pow(2,10);
            }
            if(word > (int)(Math.Pow(2, (Protocol as PSI5Protocol).DataRegionA-1) - 1))
            {
                word -= DataMask;
            }
            data.AddValue("Word", word);
            data.AddValue("Word2", word2);
            data.AddValue("Word3", word3);
        }

        private void CalculateStatistics(int index, double[] analogCurrentData, double[] analogVoltageData, double nominalTBit)
        {
            int analog_start = (int)(timestamps.GetTimeStampAt(index) - nominalTBit * SampleRate);
            int analog_stop = timestamps.GetTimeStampAt(index + 2* Protocol.DataLength);
            
            var currentData = MathFunctions.Statistics.GetSubArray(analogCurrentData, analog_start, analog_stop - analog_start);
                var voltageData  = MathFunctions.Statistics.GetSubArray(analogVoltageData, analog_start, analog_stop - analog_start);
            var curr_min = currentData.Min();
            var curr_max = currentData.Max();
            data.AddValue("VoltageMean", MathFunctions.Statistics.Mean(voltageData));

            double lev80 = curr_min + (curr_max - curr_min) * 0.8;
            double lev20 = curr_min + (curr_max - curr_min) * 0.2;
            double prev = analogCurrentData[analog_start], curr;
            int[] indexstamps = new int[4];
            while (analog_start<analog_stop)
            {
                curr = analogCurrentData[analog_start++];
                if (indexstamps[0] == 0 && prev < lev20 && curr > lev20)
                    indexstamps[0] = analog_start;
                else if (indexstamps[0] != 0 && indexstamps[1]==0 && prev<lev80 && curr>lev80)
                    indexstamps[1] = analog_start;
                else if (indexstamps[1] != 0 && indexstamps[2]==0 && prev>lev80 && curr<lev80)
                    indexstamps[2] = analog_start;
                else if (indexstamps[2] != 0 && indexstamps[3]==0 && prev>lev20 && curr<lev20)
                {
                    indexstamps[3] = analog_start;
                    break;
                }
                prev = curr;
            }
            data.AddValue("RiseTime", (indexstamps[1] - indexstamps[0])/SampleRate);
            data.AddValue("FallTime", (indexstamps[3] - indexstamps[2])/SampleRate);
            data.AddValue("MarkSpace0", 100 * (indexstamps[2] - indexstamps[0])/SampleRate);
            data.AddValue("MarkSpace1", 100 * (indexstamps[3] - indexstamps[1]) / SampleRate);

            data.AddValue("Curr_Max", curr_max *1000);
            data.AddValue("Curr_Min", curr_min * 1000);
            data.AddValue("Curr_Send", (curr_max - curr_min)*1000);
        }

        private void CalculateGapTime(int index, double tBitMin)
        {
            int EndIndexOfFrame = index + 2 * Protocol.DataLength;
            long value = (int)timestamps[EndIndexOfFrame];
            // Search the last down-signal
            while (timestamps[EndIndexOfFrame] != 0)
            {
                EndIndexOfFrame--;
            }

            data.AddValue("Last_FallEdge", (int)timestamps[EndIndexOfFrame] + (1 + 0.5 * value) * SampleRate * tBitMin);
            if (value == 0)
            {
                data.AddValue("StopTime", ((int)timestamps[EndIndexOfFrame]) / SampleRate);
            }
            else
            {
                data.AddValue("StopTime", ((int)timestamps[EndIndexOfFrame] + 0.5 * SampleRate * tBitMin) / Protocol.BitRate);
            }
        }
            

        private double Calculate_TBitMax(int index)
        {
            int EndIndexOfFrame = index + 2 * Protocol.DataLength;
            long value = (int)timestamps[EndIndexOfFrame-1] << 1 + (int)timestamps[EndIndexOfFrame];
            double TBitMinus = -2.0 / value;
            if (value == 0)
            {
                TBitMinus = -1.0;
            }
            // Search the last up-signal
            while((int)timestamps[EndIndexOfFrame]!= 1)
            {
                EndIndexOfFrame--;
            }
            return (timestamps.GetTimeStampAt(EndIndexOfFrame)-timestamps.GetTimeStampAt(index+1))/
                (Protocol.DataLength+TBitMinus)/SampleRate;
        }

        private double Calculate_TBitMin(int index)
        {
            {
                int EndIndexOfFrame = index + 2 * Protocol.DataLength;
                long value = (int)timestamps[EndIndexOfFrame];
                double TBitMinus = -1.0 + 0.5*value;
                // Search the last down-signal
                while (timestamps[EndIndexOfFrame] != 0)
                {
                    EndIndexOfFrame--;
                }
                return (timestamps.GetTimeStampAt(EndIndexOfFrame) - timestamps.GetTimeStampAt(index + 2)) /
                    (Protocol.DataLength + TBitMinus) / SampleRate;
            }
        }

        private IEnumerable<int> GetStartOfFrames()
        {
            int index = 0;
            while (!timestamps[index].Equals(States_Enum.MC_EMPTY)) { 
                if (timestamps[index] == Enums.States_Enum.MC_BRAKE0 &&
                    timestamps[index + 1] == Enums.States_Enum.MC_SHORT1 &&
                    timestamps[index + 2] == Enums.States_Enum.MC_SHORT0)
                { 
                    yield return index;
                    index += 2*Protocol.DataLength; 
                }
                index++;
            }
        }
        */
    }
}