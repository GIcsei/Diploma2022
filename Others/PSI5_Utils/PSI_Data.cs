using System.Collections.Generic;

namespace PSI_Checker_2p0.PSI5_Utils
{
    internal class PSI_Data : DataContainer
    {
        public const string ContainerName = "PSI_DATA";
        public PSI_Data() : base(ContainerName)
        {
            RestrictedNames = new List<string>
            {
                "Mesg2Mesg",
                "GapTime",
            };
        }

        public override void CalculateRestrictedFields()
        {
            List<double> pulse2pulse = new List<double>();
            List<double> width = new List<double>();
            List<double> starts = this["StartTime"];
            List<double> stops = this["StopTime"];
            int index;
            for (index = 0; index < stops.Count - 1; index++)
            {
                pulse2pulse.Add(starts[index + 1] - stops[index]);
                width.Add(stops[index] - starts[index]);
            }
            index++;
            pulse2pulse.Add(0);
            width.Add(stops[index] - starts[index]);
            this["Pulse2Pulse"] = pulse2pulse;
            this["Width"] = width;
        }
    }
}
