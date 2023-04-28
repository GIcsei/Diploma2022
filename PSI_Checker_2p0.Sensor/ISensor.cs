using PSI_Checker_2p0.Protocol;
using System.Collections.ObjectModel;

namespace PSI_Checker_2p0.Sensor
{
    public interface ISensor
    {
        /// <summary>
        /// The name of the sensor variant.
        /// </summary>
        string Variant { get; }

        /// <summary>
        /// The protocols which the sensor uses.
        /// </summary>
        ReadOnlyCollection<IProtocol> ProtocolList { get; }
        IProtocol Protocol { get; set; }

        /// <summary>
        /// Creates command according to the sensor type.
        /// </summary>
        /// <returns></returns>
        long Encode(IData data);

        /// <summary>
        /// Decode digital data according to the sensor type.
        /// </summary>
        /// <param name="data"></param>
        IData Decode(long data);
    }
}
