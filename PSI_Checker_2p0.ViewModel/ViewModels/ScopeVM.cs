using PSI_Checker_2p0.Devices;
using PSI_Checker_2p0.Utils;
using PSI_Checker_2p0.ViewModel.Properties;
using ScottPlot;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PSI_Checker_2p0.ViewModel.ViewModels
{
    public class ScopeVM : PlotterVM
    {
        #region Privates
        private NIScopeHandler Scope;
        #endregion

        #region Properties
        /// <summary>
        /// List of available devices.
        /// </summary>
        private IEnumerable<string> deviceIDs;
        public IEnumerable<string> DeviceIDs
        {
            get => deviceIDs;
            private set => SetProperty(ref deviceIDs, value, nameof(DeviceIDs));
        }

        /// <summary>
        /// The name of the selected device.
        /// By default the device with this name will be initialized.
        /// </summary>
        private string selectedDeviceID;
        public string SelectedDeviceID
        {
            get => selectedDeviceID;
            set
            {
                SetProperty(ref selectedDeviceID, value, nameof(SelectedDeviceID));
                ScopeSettings.Default.ScopeName = value;
                SetupScope();
            }
        }

        private int sampleRate;
        public int SampleRate
        {
            get => sampleRate;
            set
            {
                var MaxValue = Scope.SampleRate / usedChannels.Count(x => x.IsEnabled);
                if (value > MaxValue)
                    value = (int)MaxValue;
                SetProperty(ref sampleRate, value, nameof(SampleRate));
                ScopeSettings.Default.SamplingRateMin = sampleRate;
                Scope.SampleRateMin = sampleRate;
            }
        }

        /// <summary>
        /// Container of the used channels.
        /// Maximum allowed number of channels is 4!
        /// </summary>
        private ObservableCollection<ChannelSetting> usedChannels;
        public ObservableCollection<ChannelSetting> UsedChannels
        {
            get => usedChannels;
            set
            {
                SetProperty(ref usedChannels, value, nameof(UsedChannels));
                SetupScope();
            }
        }
        #endregion

        #region Initializers

        /// <summary>
        /// The default constructor. Initializes everything.
        /// </summary>
        public ScopeVM() : base()
        {
            MeasState = new RelayCommand(async () => await MeasStateChange());
            DeviceIDs = NIScopeHandler.ListScopes();
            usedChannels = new ObservableCollection<ChannelSetting>(ChannelSetting.InitChannelSetting());
            sampleRate = (int)ScopeSettings.Default.SamplingRateMin;
            OngoingMeas = deviceThread.IsRunning;
            SelectedDeviceID = ScopeSettings.Default.ScopeName;
        }

        #endregion

        /// <summary>
        /// Change the state of the measurement.
        /// </summary>
        public RelayCommand MeasState { get; }
        private async Task MeasStateChange()
        {
            await SetupScope();
            OngoingMeas = await ChangeThreadState();
        }

        private async Task SetupScope()
        {
            if (!deviceThread.IsRunning)
            {
                Scope = new NIScopeHandler(SelectedDeviceID);
                foreach (var elem in usedChannels)
                {
                    Scope.SetUsedChannel(elem);
                }
                await deviceThread.AddScope(Scope);
            }
        }
    }
}
