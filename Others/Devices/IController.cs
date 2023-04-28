using PSI_Checker_2p0.Protocol;

namespace PSI_Checker_2p0.Devices
{
    public interface IController : IDevice
    {
        void SendAnalog(double[] waveform);

        void SendDigital(byte data);

        bool SetProtocol(IProtocol protocol);
    }
}
