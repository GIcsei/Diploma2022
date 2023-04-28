using PSI_Checker_2p0.Enums;
using System;
using System.Collections.Generic;

namespace PSI_Checker_2p0.Devices
{
    public sealed class ChannelSetting
    {
        public readonly int ChannelNum;
        public string Name
        {
            get => $"Channel {ChannelNum}";
        }

        public bool IsEnabled { get; set; } = false;
        public bool Modifiable { get; }

        public CouplingEnum Coupling { get; set; } = CouplingEnum.DC;
        public uint Attenuation { get; set; } = 1;
        public ImpedanceEnum Impedance { get; set; } = ImpedanceEnum.Impedance_1MOhm;
        public double Range { get; set; } = 20.0;

        public ChannelSetting(int i, bool modifiable = true)
        {
            ChannelNum = i;
            Modifiable = modifiable;
            if (!Modifiable)
            {
                IsEnabled = true;
            }
        }

        public static List<ChannelSetting> InitChannelSetting(int numberOfChannels = 4)
        {
            if (numberOfChannels < 2)
                throw new ArgumentException();
            var UsedChannels = new List<ChannelSetting>();
            for (int i = 0; i < numberOfChannels; i++)
            {
                bool modifiable = i >= 2;
                UsedChannels.Add(new ChannelSetting(i, modifiable));
            }
            return UsedChannels;
        }
    }
}