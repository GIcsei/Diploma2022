using PSI_Checker_2p0.Protocol;
using System;

namespace PSI_Checker_2p0.Sensor
{
    public class SMA8 : BaseSensor
    {
        public SMA8() : base("SMA8")
        {
            protocolList.Add(new PSI5Protocol());
            protocolList.Add(new SPIProtocol());
        }

        public override long Encode(IData data)
        {
            throw new NotImplementedException();
        }

        public override IData Decode(long data)
        {
            throw new NotImplementedException();
        }
    }
}
