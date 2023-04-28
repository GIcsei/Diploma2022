using PSI_Checker_2p0.Checker;
using PSI_Checker_2p0.Protocol;
using System.Collections.Generic;

namespace PSI_Checker_2p0.Analyzer
{
    public static class AnalyzerBuilder
    {
        public static List<IAnalyzer> GetAnalyzers(IProtocol protocol, BaseResultHandler resultHandler)
        {
            var analyzers = new List<IAnalyzer>();
            if (protocol is PSI5Protocol psi)
            {
                analyzers.Add(new PSI5Analyzer(psi, resultHandler));
            }
            return analyzers;
        }
    }
}
