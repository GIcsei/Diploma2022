using PSI_Checker_2p0.Protocol;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PSI_Checker_2p0.Sensor
{
    public abstract class BaseSensor : ISensor
    {
        protected List<IProtocol> protocolList = new List<IProtocol>();
        public IProtocol Protocol
        {
            get => protocolList[selectedProtocol];
            set
            {
                selectedProtocol = protocolList.FindIndex(s => s.Name.Equals(value.Name));
            }
        }

        protected int selectedProtocol = 0;
        private readonly string variant;
        public string Variant => variant;

        public BaseSensor(string variantName)
        {
            variant = variantName;
        }
        public ReadOnlyCollection<IProtocol> ProtocolList => protocolList.AsReadOnly();

        public abstract IData Decode(long data);

        public abstract long Encode(IData data);
    }
}
