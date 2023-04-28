using PSI_Checker_2p0.PSI5_Utils;
using System.Threading.Tasks;

namespace PSI_Checker_2p0.Checker
{
    /// <summary>
    /// Analyze the data according to the given voltage and current data.
    /// The analyzation can be performed for only one independent pin.
    /// </summary>
    public interface IAnalyzer
    {
        IResultData<BaseDataContainer> ResultData { get; } // Object where the results of the analyzer can be saved to

        Task Analyze(double[] AnalogCurrentData, double[] AnalogVoltageData);
    }
}
