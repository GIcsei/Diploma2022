using PSI_Checker_2p0.Devices;
using PSI_Checker_2p0.Protocol;
using System.Threading.Tasks;

namespace PSI_Checker_2p0.Acquistion
{
    public interface IControllerThread : IDeviceThread
    {
        Task AddController(IController controller);
        Task SetProtocol(IProtocol protocol);
        Task SetPattern(Pattern pattern);
        Task SendDigitalImmediate(byte command);
    }
}