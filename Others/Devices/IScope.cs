using System.Collections.Generic;

namespace PSI_Checker_2p0.Devices
{
    public interface IScope : IDevice
    {
        int MaxChannels { get; }
        string ScopeName { get; set; }
        int RecordLength { get; set; }
        double SampleRateMin { get; set; }

        double SampleRate { get; }

        int ChannelNum { get; }

        double[] ReadValues();
        IEnumerable<double[]> ReadMultipleValues();
        void Open();
        void SetUsedChannel(ChannelSetting elem);
    }
}
