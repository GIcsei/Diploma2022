using PSI_Checker_2p0.Utils;
using PSI_Checker_2p0.ViewModel.Properties;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace PSI_Checker_2p0.ViewModel.ViewModels
{
    // TODO
    // Digitalizált adat megjelenítés (+pontos értékek)
    public class CheckerPageVM : BaseVM
    {
        private BaseVM _currentChildView;
        private string _caption;
        private string _iconSource;
        public const bool valami = false;

        private string selectedDevice = ScopeSettings.Default.ScopeName;
        public string SelectedDevice
        {
            get => selectedDevice;
            set => SetProperty(ref selectedDevice, value, nameof(SelectedDevice));
        }

        public string Version
        {
            get => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private double sampleRateMin = ScopeSettings.Default.SamplingRateMin;
        public double SampleRateMin
        {
            get => sampleRateMin;
            set
            {
                SetProperty(ref sampleRateMin, value, nameof(SampleRateMin));
            }
        }

        public List<string> Languages
        {
            get; set;
        } = new List<string> { "en-US", "hu-HU" };

        private string selectedLanguage;
        public string SelectedLanguage
        {
            get => selectedLanguage ?? Languages.First();
            set
            {
                selectedLanguage = value;
            }
        }

        public BaseVM CurrentChildView
        {
            get => _currentChildView;
            set => SetProperty(ref _currentChildView, value, nameof(CurrentChildView));
        }

        public string Caption
        {
            get => _caption;
            set => SetProperty(ref _caption, value, nameof(Caption));
        }

        public string IconSource
        {
            get => _iconSource;
            set => SetProperty(ref _iconSource, value, nameof(IconSource));
        }

        public CheckerPageVM()
        {
            ShowScopeViewCommand = new RelayCommand(ExecuteShowScopeViewCommand);
            ShowSettingsViewCommand = new RelayCommand(ExecuteShowSettingsViewCommand);
            ShowSimulyzerViewCommand = new RelayCommand(ExecuteShowSimulyzerViewCommand);
            ShowCheckerViewCommand = new RelayCommand(ExecuteShowCheckerViewCommand);
            ShowBiDirViewCommand = new RelayCommand(ExecuteShowBiDirViewCommand);
            ShowResultsViewCommand = new RelayCommand(ExecuteShowResultsViewCommand);
            ShowTesterViewCommand = new RelayCommand(ExecuteShowTesterViewCommand);
            ExecuteShowScopeViewCommand();

            ScopeSettings.Default.SettingChanging += ScopeSettingChanged;
        }

        private void ScopeSettingChanged(object sender, SettingChangingEventArgs e)
        {
            if (e.SettingName.Equals("SamplingRateMin"))
            {
                double.TryParse(e.NewValue.ToString(), out double result);
                SampleRateMin = result;
            }
            if (e.SettingName.Equals("ScopeName"))
            {
                SelectedDevice = e.NewValue.ToString();
            }
        }

        #region Commands
        public ICommand ShowScopeViewCommand { get; }
        public ICommand ShowSimulyzerViewCommand { get; }
        public ICommand ShowSettingsViewCommand { get; }
        private void ExecuteShowSimulyzerViewCommand()
        {
            CurrentChildView = new SimulyzerVM();
            Caption = "Simulyzer";
            IconSource = "/Images/test-tube.png";
        }

        public ICommand ShowTesterViewCommand { get; }
        private void ExecuteShowTesterViewCommand()
        {
            CurrentChildView = new TesterVM();
            Caption = "Testing";
            IconSource = "/Images/test-tube.png";
        }

        private void ExecuteShowScopeViewCommand()
        {
            CurrentChildView = new ScopeVM();
            Caption = "Scope";
            IconSource = "/Images/device-digital.png";
        }

        private void ExecuteShowSettingsViewCommand()
        {
            CurrentChildView = new SettingsVM();
            Caption = "Settings";
            IconSource = "/Images/settings.png";
        }

        public ICommand ShowCheckerViewCommand
        {
            get;
        }

        private void ExecuteShowCheckerViewCommand()
        {
            CurrentChildView = new CheckerVM();
            Caption = "TDMSHandler";
            IconSource = "/Images/settings.png";
        }

        public ICommand ShowBiDirViewCommand { get; }
        private void ExecuteShowBiDirViewCommand()
        {
            CurrentChildView = DIService.Instance.Get<BiDirVM>();
            Caption = "BiDir";
            IconSource = "/Images/device-digital.png";
        }

        public ICommand ShowResultsViewCommand { get; }
        private void ExecuteShowResultsViewCommand()
        {
            CurrentChildView = new ResultsVM();
            Caption = "Results";
            IconSource = "/Images/device-digital.png";
        }
        #endregion
    }
}
