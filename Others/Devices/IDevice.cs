using System.Collections.Generic;
using System.Threading.Tasks;

namespace PSI_Checker_2p0
{
    public interface IDevice
    {
        void Open(string deviceID);
        Task Close();
        IEnumerable<string> ListDevices();
        void Init();
    }
}
