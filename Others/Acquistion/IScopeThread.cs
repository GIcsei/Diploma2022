using PSI_Checker_2p0.Devices;
using PSI_Checker_2p0.FileHandler.FileSaver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSI_Checker_2p0.Acquistion
{
    public interface IScopeThread : IDeviceThread
    {
        BaseScopeFileHandler FileSaver { get; set; }
        List<double[]> ReadValues { get; }
        Task AddScope(IScope scope);
    }
}