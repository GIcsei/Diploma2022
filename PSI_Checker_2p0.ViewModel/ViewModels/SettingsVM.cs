using PSI_Checker_2p0.ViewModel.Properties;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;

namespace PSI_Checker_2p0.ViewModel.ViewModels
{
    public class SettingsVM : BaseVM
    {
        private ObservableCollection<KeyValuePair<string, List<object>>> settings;
        public ObservableCollection<KeyValuePair<string, List<object>>> Settings
        {
            get => settings;
            set => SetProperty(ref settings, value, nameof(Settings));
        }

        public SettingsVM()
        {
            //Settings = StaticFunctions.LoadSettings();
            SettingsList = new List<SettingsProperty>();
            foreach (SettingsProperty elem in ScopeSettings.Default.Properties)
            {
                SettingsList.Add(elem);
            }
        }

        public List<SettingsProperty> SettingsList
        {
            get;
        }
    }
}
