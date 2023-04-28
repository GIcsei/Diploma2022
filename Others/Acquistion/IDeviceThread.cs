using System.Threading.Tasks;

namespace PSI_Checker_2p0.Acquistion
{
    public interface IDeviceThread
    {
        bool IsRunning { get; }

        Task StartThread();
        Task StopThread();
    }
}