using NationalInstruments;
using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.ModularInstruments.SystemServices.DeviceServices;
using NationalInstruments.Restricted;
using PSI_Checker_2p0.Enums;
using PSI_Checker_2p0.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PSI_Checker_2p0.Devices
{
    public class NIScopeHandler : NotifyBase, IScope
    {
        private NIScope Scope;
        private ChannelSetting[] usedChannels;
        private AnalogWaveformCollection<double> scopeWaveforms = new AnalogWaveformCollection<double>();
        private readonly PrecisionTimeSpan precisionTime = PrecisionTimeSpan.FromTimeSpan(new TimeSpan(-1));
        private int recordLength = 7000000;
        private string EnabledChannels = "";

        public int RecordLength
        {
            get => recordLength;
            set => recordLength = value;
        }

        public int MaxChannels
        {
            get => maxChannelNum;
        }

        private bool Errored = true;

        #region Const Values
        private readonly double InputFreqMax = 0;
        private const double MaxSampleRateMin = 90_000_000;
        private const double referencePosition = 0.0;
        private const int numberOfRecords = 1;
        private const bool enforceRealtime = true;
        private double sampleRateMin = 9_000_000;
        private readonly double VerticalOffset = 0;
        private int maxChannelNum = 4;
        #endregion

        public int ChannelNum { get => usedChannels.Count(x => x.IsEnabled); }

        public double SampleRateMin
        {
            get => sampleRateMin;
            set
            {
                if (MaxSampleRateMin < value) throw new ArgumentException();
                SetProperty(ref sampleRateMin, value, nameof(SampleRateMin));
            }
        }

        public string ScopeName { get; set; }

        public double SampleRate => MaxSampleRateMin;

        public NIScopeHandler(string selectedScopeName)
        {
            ScopeName = selectedScopeName;
            maxChannelNum = 4;
            usedChannels = new ChannelSetting[maxChannelNum];
        }

        public void SetUsedChannel(ChannelSetting channel)
        {
            if (channel.ChannelNum > MaxChannels)
                throw new ArgumentException();
            usedChannels[channel.ChannelNum] = channel;
        }

        public void Open(string deviceID)
        {
            //logger.LogInformation($"Trying to open connection to the Scope with device ID: {deviceID}!");
            try
            {
                Scope = new NIScope(deviceID, false, false);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private void SetUtils()
        {
            EnabledChannels = "";
            foreach (var element in usedChannels.Where(x => x.IsEnabled))
            {
                Scope.Channels[element.ChannelNum.ToString()].Configure
                    (element.Range, VerticalOffset, ConvertCoupling(element), element.Attenuation, true);
                Scope.Channels[element.ChannelNum.ToString()].ConfigureCharacteristics
                    ((double)element.Impedance, InputFreqMax);
                EnabledChannels += $"{element.ChannelNum},";
            }
            Scope.Trigger.ConfigureTriggerSoftware(PrecisionTimeSpan.Zero, PrecisionTimeSpan.Zero);

            Scope.Measurement.FetchRelativeTo = ScopeFetchRelativeTo.ReadPointer;

            // Configure the horizontal parameters.
            Scope.Timing.ConfigureTiming(sampleRateMin, recordLength, referencePosition, numberOfRecords, enforceRealtime);
            Scope.Measurement.Commit();
        }

        private ScopeVerticalCoupling ConvertCoupling(ChannelSetting element)
        {
            switch ((element.Coupling)
)
            {
                case CouplingEnum.AC:
                    return ScopeVerticalCoupling.AC;
                case CouplingEnum.DC:
                    return ScopeVerticalCoupling.DC;
                default:
                    return ScopeVerticalCoupling.DC;
            }
        }

        public void Init()
        {
            Errored = false;
            SetUtils();
            Scope.Measurement.Initiate();
        }

        public async System.Threading.Tasks.Task Close()
        {
            await System.Threading.Tasks.Task.Delay(5000);
            Scope.Close();
            Scope.Dispose();
            scopeWaveforms = new AnalogWaveformCollection<double>();
        }

        public static IEnumerable<string> ListScopes()
        {
            using (ModularInstrumentsSystem scopeDevices = new ModularInstrumentsSystem("NI-Scope"))
            {
                foreach (DeviceInfo device in scopeDevices.DeviceCollection)
                {
                    yield return device.Name;
                }
            }
        }

        public IEnumerable<string> ListDevices()
        {
            return ListScopes();
        }

        public double[] ReadValues()
        {
            try
            {
                scopeWaveforms = Scope.Channels[$"{usedChannels.First().ChannelNum}"].Measurement.FetchDouble(precisionTime, recordLength, scopeWaveforms);
            }
            catch
            {
                Errored = true;
                return Array.Empty<double>();
            }
            return scopeWaveforms[0].GetRawData();
        }

        public IEnumerable<double[]> ReadMultipleValues()
        {
            try
            {
                scopeWaveforms = Scope.Channels[EnabledChannels].Measurement.FetchDouble(precisionTime, recordLength, scopeWaveforms);
            }
            catch
            {
                Errored = true;
                yield break;
            }
            foreach (AnalogWaveform<double> wf in scopeWaveforms.Cast<AnalogWaveform<double>>())
            {
                yield return wf.GetRawData();
            }
        }

        public void Open()
        {
            if ((ScopeName is null) || (ScopeName.IsEmpty()))
                throw new Exception();
            this.Open(ScopeName);
        }
    }
}