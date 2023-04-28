using NationalInstruments.DAQmx;
using PSI_Checker_2p0.Protocol;
using System;
using System.Collections.Generic;

namespace PSI_Checker_2p0.Devices
{
    public class NIGpioHandler : IController
    {
        private NationalInstruments.DAQmx.Task AnalogTask;
        private NationalInstruments.DAQmx.Task DigitalTask;
        private string DeviceID;
        public async System.Threading.Tasks.Task Close()
        {
            AnalogTask.Stop();
            DigitalTask.Stop();
            AnalogTask.Dispose();
            DigitalTask.Dispose();
            Reset();
        }

        public IEnumerable<string> ListDevices()
        {
            return ListEcus();
        }

        public NIGpioHandler(string name)
        {
            DeviceID = name;
        }

        public void Open(string deviceID)
        {
            if (deviceID is null) return;
            this.DeviceID = deviceID;
            Init();
        }

        private void Reset()
        {
            var device = DaqSystem.Local.LoadDevice(DeviceID);
            device.Reset();
        }

        public void Init()
        {
            Reset();
            AnalogTask = new NationalInstruments.DAQmx.Task();
            AnalogTask.AOChannels.CreateVoltageChannel(DeviceID + "/ao0", "aoChannel",
                0, 10, AOVoltageUnits.Volts);
            AnalogTask.AOChannels.All.DataTransferMechanism = AODataTransferMechanism.Dma;
            AnalogTask.AOChannels.All.DataTransferRequestCondition = AODataTransferRequestCondition.OnBoardMemoryHalfFullOrLess;
            AnalogTask.Timing.SampleTimingType = SampleTimingType.SampleClock;
            AnalogTask.Control(TaskAction.Commit);
            DigitalTask = new NationalInstruments.DAQmx.Task();
            DigitalTask.DOChannels.CreateChannel(DeviceID + "/port0/line0:7", "doPorts", ChannelLineGrouping.OneChannelForAllLines);
        }

        public void SendAnalog(double[] waveform)
        {
            if (AnalogTask is null) return;
            AnalogTask.Timing.ConfigureSampleClock("", AnalogTask.Timing.SampleClockMaximumRate
                , SampleClockActiveEdge.Rising,
                SampleQuantityMode.FiniteSamples, waveform.Length);
            var writer = new AnalogSingleChannelWriter(AnalogTask.Stream);
            writer.WriteMultiSample(false, waveform);
            AnalogTask.Start();
            AnalogTask.WaitUntilDone();
        }

        /// <summary>
        /// Writes the data to the ports defined in <see cref="DigitalTask"/>.
        /// </summary>
        /// <param name="data">The data to be written with LSB</param>
        public void SendDigital(byte data)
        {
            if (DigitalTask is null) return;
            bool[] DigitalValues = new bool[8];
            for (int i = 0; i < DigitalValues.Length; i++)
            {
                DigitalValues[i] = ((data >> i) & 0x1) == 1;
            }
            var writer = new DigitalSingleChannelWriter(DigitalTask.Stream);
            writer.WriteSingleSampleMultiLine(true, DigitalValues);
        }

        public bool SetProtocol(IProtocol protocol)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<string> ListEcus()
        {
            foreach (var device in DaqSystem.Local.Devices)
            {
                yield return device;
            }
        }
    }
}
