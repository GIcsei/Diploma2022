namespace PSI_Checker_2p0.Utils
{
    public class ImportantInfos : NotifyBase
    {
        public ImportantInfos()
        {
        }

        /// <summary>
        /// Progress of the currently running AnalogTask.
        /// Maximum value is 100!
        /// </summary>
        private int progress = 0;
        public int Progress
        {
            get => progress;
            set
            {
                if (value > 100) return;
                SetProperty(ref progress, value, nameof(Progress));
            }
        }

        private string protocolName = "No protocol selected";
        public string ProtocolName
        {
            get => protocolName;
            set => SetProperty(ref protocolName, value, nameof(ProtocolName));
        }

        private string currentPatternName = "No pattern selected";
        public string CurrentPatternName
        {
            get => currentPatternName;
            set => SetProperty(ref currentPatternName, value, nameof(CurrentPatternName));
        }
    }
}
