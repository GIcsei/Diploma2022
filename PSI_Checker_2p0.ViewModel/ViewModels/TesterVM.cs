using Microsoft.WindowsAPICodePack.Dialogs;
using PSI_Checker_2p0.Acquistion;
using PSI_Checker_2p0.Checker;
using PSI_Checker_2p0.Enums;
using PSI_Checker_2p0.FileHandler.FileSaver;
using PSI_Checker_2p0.Logger;
using PSI_Checker_2p0.Protocol;
using PSI_Checker_2p0.Sensor;
using PSI_Checker_2p0.Utils;
using ScottPlot;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PSI_Checker_2p0.ViewModel.ViewModels
{
    public class TesterVM : PlotterVM
    {
        // TODO
        /*
         * ATT => egyelőre nem implementált (külön felugró ablak)
         * CONFORMITY => is felugró ablak
         * File Select => Beállítások (random generátor) konfigurációja
         * Sensor select => Sensor interfészen keresztül
         */
        private IScopeThread scope = DIService.Instance.Get<ScopeDeviceThread>();
        private IControllerThread device = DIService.Instance.Get<ControllerDeviceThread>();

        #region Properties
        private ISensor selectedSensor;
        public ISensor SelectedSensor
        {
            get => selectedSensor;
            set => SetProperty(ref selectedSensor, value, nameof(SelectedSensor));
        }

        private List<ISensor> sensors;
        public List<ISensor> Sensors
        {
            get => sensors;
            set => SetProperty(ref sensors, value, nameof(Sensors));
        }

        private PSI5_Mode selectedMode = PSI5_Mode.Mode_16;
        public PSI5_Mode SelectedMode
        {
            get => selectedMode;
            set
            {
                protocol.Bit_Len = (int)value;
                SetProperty(ref selectedMode, value, nameof(SelectedMode));
            }
        }
        private PSI5_CheckSum selectedCheckSum = PSI5_CheckSum.CRC;
        public PSI5_CheckSum SelectedCheckSum
        {
            get => selectedCheckSum;
            set
            {
                protocol.CheckSum = (int)value;
                SetProperty(ref selectedCheckSum, value, nameof(SelectedCheckSum));
            }
        }
        private PSI5_Sync selectedSync = PSI5_Sync.Sync;
        public PSI5_Sync SelectedSync
        {
            get => selectedSync;
            set
            {
                protocol.Sync = value != 0;
                SetProperty(ref selectedSync, value, nameof(SelectedSync));
            }
        }
        private PSI5_SyncDist selectedSyncDist = PSI5_SyncDist.Dist_500;
        public PSI5_SyncDist SelectedSyncDist
        {
            get => selectedSyncDist;
            set
            {
                protocol.Sync_Dist = (int)value;
                SetProperty(ref selectedSyncDist, value, nameof(SelectedSyncDist));
            }
        }

        private PSI5_DataRate selectedDataRate = PSI5_DataRate.High;
        public PSI5_DataRate SelectedDataRate
        {
            get => selectedDataRate;
            set
            {
                protocol.BitRate = (int)value;
                SetProperty(ref selectedDataRate, value, nameof(SelectedDataRate));
            }
        }

        private bool isPowered = false;
        public bool IsPowered
        {
            get => isPowered;
            set => SetProperty(ref isPowered, value, nameof(IsPowered));
        }

        private readonly PSI5Protocol protocol = new PSI5Protocol();


        #region BoardSettings
        private bool[] boardSettings = new bool[8];
        public bool Signal
        {
            get => boardSettings[6];
            set => SetProperty(ref boardSettings[6], value, nameof(Signal));
        }

        public bool ExtSrc
        {
            get => boardSettings[0];
            set => SetProperty(ref boardSettings[0], value, nameof(ExtSrc));
        }

        public bool Output
        {
            get => boardSettings[5];
            set => SetProperty(ref boardSettings[5], value, nameof(Output));
        }

        public bool Load
        {
            get => boardSettings[4];
            set => SetProperty(ref boardSettings[4], value, nameof(Load));
        }

        public bool Bypass
        {
            get => boardSettings[1];
            set => SetProperty(ref boardSettings[1], value, nameof(Bypass));
        }

        public bool Gain
        {
            get => boardSettings[3];
            set => SetProperty(ref boardSettings[3], value, nameof(Gain));
        }
        #endregion

        /// <summary>
        /// Debug purposes only! TODO
        /// </summary>
        private string logMessages;
        public string LogMessages
        {
            get => logMessages;
            set => SetProperty(ref logMessages, value, nameof(LogMessages));
        }

        /// <summary>
        /// The selected file path for the pattern.
        /// </summary>
        private string patternFilePath = "\\\\bp01fs02\\work_aeesi2$\\72_Charakterisierung\\72_Charakterisierungen_Projekte\\CH0318_SMA8\\20_Results\\XA820_FPGA\\5_PSI_Checker\\SMAEIGHT_2592_PSI_ATT\\07_Patterns\\Init_synch.txt";
        public string PatternFilePath
        {
            get => patternFilePath;
            set => SetProperty(ref patternFilePath, value, nameof(PatternFilePath));
        }

        /// <summary>
        /// Collection of the loaded pattern names.
        /// </summary>
        private List<Pattern> patterns = new List<Pattern>();
        public List<Pattern> Patterns
        {
            get => patterns;
            protected set => SetProperty(ref patterns, value, nameof(Patterns));
        }

        private Pattern selectedPattern;
        public Pattern SelectedPattern
        {
            get => selectedPattern;
            set => SetProperty(ref selectedPattern, value, nameof(SelectedPattern));
        }
        #endregion

        #region Commands
        public ICommand LoadData { get; }
        private async Task ExecuteLoadData()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = "C:\\Users",
                IsFolderPicker = true,

            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                await ShowAll(scope.FileSaver.LoadChannelsFromFolder(dialog.FileName));
            }
        }

        public ICommand Analyze { get; }
        private async Task ExecuteAnalyze()
        {
            var Measurement = new CheckerMeasurement<TDMSHandler>(new SMA8(), SelectedPattern);
            var result = await Measurement.Measure(scope, device);
        }

        public ICommand MeasureAll { get; }
        private async Task ExecuteMeasureAll()
        {
            SelectedPattern.Name = Patterns.FirstOrDefault().Name;
            await ExecuteMeasureFromSelected();
        }

        public ICommand MeasureFromSelected { get; }
        private async Task ExecuteMeasureFromSelected()
        {
            var res = Patterns.SkipWhile(x => x.Name != SelectedPattern.Name);
            int i = 0;
            double normalizer = 100.0 / res.Count();
            DIService.Instance.Get<ImportantInfos>().Progress = 0;
            foreach (var pattern in res)
            {
                await ExecuteMeasureSelected(pattern);
                DIService.Instance.Get<ImportantInfos>().Progress = (int)(++i * normalizer);
            }
        }
        public ICommand MeasureSelected { get; }
        private async Task ExecuteMeasureSelected(Pattern selected = null)
        {
            var currentPattern = selected;
            if (currentPattern is null)
                currentPattern = SelectedPattern;
            var Measurement = new CheckerMeasurement<TDMSHandler>(new SMA8(), SelectedPattern);
            await Task.Delay(1000);
            await Measurement.Measure(scope, device);
        }

        public ICommand SelectSaveLocation { get; }
        private void ExecuteSelectSaveLocation()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = "C:\\Users",
                IsFolderPicker = true,

            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                scope.FileSaver = ScopeFileSaverBuilder.CreateFileSaver(
                    FileSavePolicy.FillDisk, protocol.ToString(), dialog.FileName);
            }
        }
        #endregion
        public TesterVM()
        {
            PropertyChanged += new PropertyChangedEventHandler(EnumParamChanged);
            DIService.Logger.NewLog += Logger_NewLog;
            Analyze = new RelayCommand(async () => await ExecuteAnalyze());
            MeasureAll = new RelayCommand(async () => await ExecuteMeasureAll());
            MeasureFromSelected = new RelayCommand(async () => await ExecuteMeasureFromSelected());
            MeasureSelected = new RelayCommand(async () => await ExecuteMeasureSelected());
            LoadData = new RelayCommand(async () => await ExecuteLoadData());
            SelectSaveLocation = new RelayCommand(ExecuteSelectSaveLocation);
            Sensors = new List<ISensor>
            {
                new SMA8(),
            };
        }

        private void Logger_NewLog((string Message, LogLevel Level) obj)
        {
            LogMessages += $"[{obj.Level}]: " + obj.Message + "\n";
        }

        private void EnumParamChanged(object sender, PropertyChangedEventArgs e)
        {
            //Update protocol name
            DIService.Instance.Get<ImportantInfos>().ProtocolName = protocol.ToString();
            if (e.PropertyName != nameof(LogMessages))
            {
                DIService.Logger.Log("Property changed: " + e.PropertyName, Logger.LogLevel.Informative);
            }
        }

    }
}
