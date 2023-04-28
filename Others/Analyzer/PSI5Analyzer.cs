using PSI_Checker_2p0.Checker;
using PSI_Checker_2p0.Protocol;
using PSI_Checker_2p0.PSI5_Utils;
using System;
using System.Threading.Tasks;

namespace PSI_Checker_2p0.Analyzer
{
    public class PSI5Analyzer : BaseAnalyzer
    {
        private double[] AnalogCurrentData;
        private double[] AnalogVoltageData;

        private readonly double SampleRate = 90_000_000;

        public int SyncWidth_Min
        {
            get;
            set;
        } = 25;
        public double SyncTrig_Max
        {
            get;
            set;
        } = 7.6;
        public double SyncTrig
        {
            get;
            set;
        } = 7;

        // Lépések, amit meg kell csinálni:
        // Digitalizálás --> Teljes adatok, ennek működnie kell analyzer nélkül is --> Attól független, vagy szenzor vagy protocol funkció
        // Analóg áram elemzés --> Megadott szempontok alapján, meglévők behúzása
        // Analóg feszültség elemzés --> Same

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentProtocol"></param>
        /// <param name="result"></param>
        public PSI5Analyzer(PSI5Protocol currentProtocol, BaseResultHandler result) :
            base(result, new PSI_Result(), currentProtocol)
        {

        }

        public override async Task Analyze(double[] AnalogCurrentData, double[] AnalogVoltageData)
        {
            this.AnalogCurrentData = AnalogCurrentData;
            this.AnalogVoltageData = AnalogVoltageData;
            await AnalyzeVoltage();
        }

        protected override Task AnalyzeCurrent()
        {
            throw new NotImplementedException();
        }

        protected override Task AnalyzeVoltage()
        {
            int start = 0;
            bool StartFound = false;
            var voltageDataResult = data[PSI_Sync.ContainerName];
            for (int index = 1; index < AnalogVoltageData.Length; index++)
            {
                if (StartFound)
                {
                    if (GetSyncStop(index))
                    {
                        voltageDataResult.AddValue("Polarity", 1);
                        MathFunctions.Statistics.MinMaxMean(MathFunctions.Statistics.GetSubArray(AnalogVoltageData, start, index - start),
                            out double Min_Pulse, out double Max_Pulse, out double Voltage_Mean);
                        voltageDataResult.AddValue("Max_Pulse", Max_Pulse);
                        voltageDataResult.AddValue("Min_Pulse", Min_Pulse);
                        voltageDataResult.AddValue("Voltage", Voltage_Mean);
                    }
                }
                else
                {
                    StartFound = GetSyncStart(index);
                    start = index;
                }
            }
            data.CalculateRemainingFields();
            return Task.CompletedTask;
        }

        private bool GetSyncStop(int index)
        {
            var analogData = AnalogVoltageData; ;
            bool result = false;
            int j = 1;
            var voltageDataResult = data[PSI_Sync.ContainerName];
            if (analogData[index + 1] < SyncTrig_Max && analogData[index] > SyncTrig_Max)
            {
                result = true;
                for (j = 1; j < SyncWidth_Min; j++)
                {
                    result &= analogData[index + j] < SyncTrig_Max;
                }
            }
            if (result)
            {
                try
                {
                    for (j = 1; analogData[index + j] >= SyncTrig; j++) ;
                }
                catch (ArgumentOutOfRangeException) { }
                int syncStop = index - j;
                voltageDataResult.AddValue("StopTime", syncStop / SampleRate);
                voltageDataResult.AddValue("StopSlope", (analogData[index] - analogData[syncStop]) * SampleRate / (syncStop - index));
            }
            return result;
        }

        private bool GetSyncStart(int index)
        {
            var analogData = AnalogVoltageData;
            bool result = false;
            int j;
            var voltageDataResult = data[PSI_Sync.ContainerName];
            if (analogData[index + 1] > SyncTrig_Max && analogData[index] < SyncTrig_Max)
            {
                result = true;
                for (j = 1; j < SyncWidth_Min; j++) // Check if the min with is counted
                {
                    result &= analogData[index + j] > SyncTrig_Max;
                }
            }
            if (result)
            {
                for (j = 1; j < index && analogData[index - j] >= SyncTrig; j++) ; // Search the index where the sync pulse counted as triggered
                int syncStart = index - j;
                voltageDataResult.AddValue("ID", data.Counter);
                voltageDataResult.AddValue("StartTime", syncStart / SampleRate);
                voltageDataResult.AddValue("StartSlope", (analogData[index] - analogData[syncStart]) * SampleRate / j);
            }
            return result;
        }
    }
}
