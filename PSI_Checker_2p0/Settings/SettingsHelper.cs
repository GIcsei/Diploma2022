namespace PSI_Checker_2p0.Settings
{
    /*public static ObservableCollection<KeyValuePair<string, List<SettingsHelper>>> LoadSettings()
    {
        return new ObservableCollection<KeyValuePair<string, List<SettingsHelper>>> {
                new KeyValuePair<string, List<SettingsHelper>>("Scope Settings", PopulateList(ScopeSettings.Default)),
                new KeyValuePair<string, List<SettingsHelper>>("App Settings", PopulateList(AppSettings.Default)),
                new KeyValuePair<string, List<SettingsHelper>>("Measurement Settings", PopulateList(MeasurementSettings.Default))
            };
    }

    public static List<SettingsHelper> PopulateList(ApplicationSettingsBase setting)
    {
        List<SettingsHelper> list = new List<SettingsHelper>();
        foreach (SettingsProperty item in setting.Properties)
        {
            list.Add(new SettingsHelper(setting)
            {
                Name = item.Name,
                Type = item.PropertyType,
                Value = setting[item.Name],
            });
        }
        return list;
    }*/
}
