using PSI_Checker_2p0.Protocol;
using PSI_Checker_2p0.PSI5_Utils;
using System.Threading.Tasks;

namespace PSI_Checker_2p0.Checker
{
    /// <summary>
    /// The base class of all analyzers. Define the needed functions for analyzation.
    /// </summary>
    public abstract class BaseAnalyzer : IAnalyzer
    {
        public IResultData<BaseDataContainer> ResultData => resultData;
        protected readonly IResultData<BaseDataContainer> resultData;
        protected readonly BaseDataContainer data;
        protected readonly IProtocol Protocol;

        protected BaseAnalyzer(IResultData<BaseDataContainer> resultData,
            BaseDataContainer baseDataContainer,
            IProtocol protocol)
        {
            this.resultData = resultData;
            this.data = baseDataContainer;
            this.Protocol = protocol;
        }

        public abstract Task Analyze(double[] AnalogCurrentData, double[] AnalogVoltageData);

        protected abstract Task AnalyzeCurrent();
        protected abstract Task AnalyzeVoltage();
    }
}
