using Microsoft.Win32;
using PSI_Checker_2p0.Acquistion;
using PSI_Checker_2p0.Devices;
using PSI_Checker_2p0.Enums;
using PSI_Checker_2p0.FileHandler.FileManager.FileManagers;
using PSI_Checker_2p0.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PSI_Checker_2p0.ViewModel.ViewModels
{
    public class InitVM : BaseVM
    {
        public IEnumerable<string> Scopes { get; } = Devices.NIScopeHandler.ListScopes();
        public IEnumerable<string> Ecus { get; } = Devices.NIGpioHandler.ListEcus();

        private string configFileDir;
        public string ConfigFileDir
        {
            get => configFileDir;
            set => SetProperty(ref configFileDir, value, nameof(ConfigFileDir));
        }

        public string SelectedScopeName
        {
            get => selectedScopeName;
            set
            {
                SetProperty(ref selectedScopeName, value);
                Properties.ScopeSettings.Default.ScopeName = value;
            }
        }

        private string selectedEcuName;
        public string SelectedEcuName
        {
            get => selectedEcuName;
            set
            {
                SetProperty(ref selectedEcuName, value);
                DIService.Instance.Get<ControllerDeviceThread>().AddController(new NIGpioHandler(selectedEcuName));
            }
        }

        public ObservableCollection<PsiConfig> Config { get; set; } = new ObservableCollection<PsiConfig>();

        public ICommand InitDevices { get; }

        private bool failedInit = false;
        public bool FailedInit
        {
            get => failedInit;
            set => SetProperty(ref failedInit, value);
        }

        public bool InitIsRunning { get; set; }

        private bool stepNextPage = false;
        private string selectedScopeName;

        public bool StepNextPage
        {
            get => stepNextPage;
            set => SetProperty(ref stepNextPage, value);
        }

        public InitVM()
        {
            InitDevices = new RelayCommand(Init);
            Config = Task.Run(async () =>
            {
                return await DIService.PsiConfigs.LoadConfigs("PsiSettings.json");
            }).Result;
            SaveConfig = new RelayCommand(async () => await ExecuteSaveConfig());
            LoadConfig = new RelayCommand(async () => await ExecuteLoadConfig());
            SelectedEcuName = Ecus.FirstOrDefault();
            SelectedScopeName = Scopes.FirstOrDefault();
        }

        public void Init()
        {
            try
            {
                var scope = new NIScopeHandler(SelectedScopeName);
                scope.Open();
                scope.Close();
            }
            catch
            {
                FailedInit = true;
                return;
            }
            FailedInit = false;
            /*
            await RunCommand(() => this.InitIsRunning, async () =>
            {
                await Task.Delay(1000);
            });*/
            DIService.Instance.Get<MenuVM>().CurrentPage = ApplicationPage.Checker;
        }

        public ICommand SaveConfig { get; }
        private async Task ExecuteSaveConfig()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Json files (*.json)|*.json"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                await Serializer.SaveJson<ObservableCollection<PsiConfig>>(Config, saveFileDialog.FileName);
                ConfigFileDir = saveFileDialog.FileName;
            }
        }

        public ICommand LoadConfig { get; }
        private async Task ExecuteLoadConfig()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Json files (*.json)|*.json"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                Config = await DIService.PsiConfigs.LoadConfigs(openFileDialog.FileName);
            }
            OnPropertyChanged(nameof(Config));
        }

    }
}
