using PSI_Checker_2p0.Devices;
using PSI_Checker_2p0.Protocol;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSI_Checker_2p0.Acquistion
{
    public class ControllerDeviceThread : BaseDeviceThread, IControllerThread
    {
        private IController device;
        public ControllerDeviceThread() : base("ControllerThread")
        {
        }

        private List<double> waveform = new List<double>();

        public async Task AddController(IController controller)
        {
            await Task.Run(() =>
            {
                device?.Close();
                device = controller;
                device.Init();
            });
        }

        //TODO
        // Change hard coded values!
        public async Task SetPattern(Pattern pattern)
        {
            waveform = new List<double>();
            var SampleRate = 2500000.0;
            var TimeScale = 1_000_000; // The time scale of the pattern file. It is always in [us]
            await Task.Run(() =>
            {
                foreach (var sample in pattern.Values)
                {
                    var elemNum = (int)(SampleRate / TimeScale * sample.Item2);
                    double[] values = new double[elemNum];
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = sample.Item1;
                    }
                    waveform.AddRange(values);
                }
            });
        }

        public async Task SetProtocol(IProtocol protocol)
        {
            await Task.Run(() =>
            {
                device.SetProtocol(protocol);
            });
        }

        /// <summary>
        /// Function called when the AnalogTask is finished.
        /// </summary>
        protected override async Task ClearUp()
        {
            await device?.Close();
        }

        /// <summary>
        /// Called before the thread started. If there is any values which should be 
        /// initialized right before the run, can be performed here.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected override void InitValues()
        {

        }

        /// <summary>
        /// The AnalogTask of the thread. This will run in every cases when the thread starts.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        protected override void ThreadTask()
        {
            device.SendAnalog(waveform.ToArray());
        }

        public async Task SendDigitalImmediate(byte command)
        {
            await Task.Run(() =>
            {
                device?.SendDigital(command);
            });
        }
    }
}
