using PSI_Checker_2p0.Acquistion;
using PSI_Checker_2p0.Sensor;
using System.Threading;
using System.Threading.Tasks;

namespace PSI_Checker_2p0.Checker
{
    public abstract class BaseMeasurement<T> : IMeasurement<T>
        where T : BaseResultHandler, new()
    {

        protected static readonly SemaphoreSlim SelfLock = new SemaphoreSlim(1, 1);

        protected readonly T ResultData;
        protected IAnalyzer Analyzer;
        protected IControllerThread Controller;
        protected IScopeThread Scope;

        public ISensor Sensor
        {
            get;
        }

        protected BaseMeasurement(ISensor sensor)
        {
            Sensor = sensor;
            ResultData = new T();
        }

        public abstract Task<T> Measure(IScopeThread scope, IControllerThread controller);
    }
}