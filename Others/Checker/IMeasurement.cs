using PSI_Checker_2p0.Acquistion;
using PSI_Checker_2p0.Sensor;
using System.Threading.Tasks;

namespace PSI_Checker_2p0.Checker
{
    public interface IMeasurement<T>
    {
        /// <summary>
        /// Performs an overall measurement on the <see cref="Sensor"/>.
        /// To be successfull, a reference for an input and output device thread is needed.
        /// </summary>
        /// <returns></returns>
        Task<T> Measure(IScopeThread scope, IControllerThread controller);

        /// <summary>
        /// The sensor which on the measurement will be performed.
        /// </summary>
        ISensor Sensor { get; }
    }
}
