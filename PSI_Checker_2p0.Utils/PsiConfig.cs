using PSI_Checker_2p0.FileHandler.FileManager.FileManagers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PSI_Checker_2p0.Utils
{
    [Serializable]
    public sealed class PsiConfig
    {
        private int min;
        public int Min
        {
            get { return min; }
            set { min = value; }
        }

        private int nominal;
        public int Nominal
        {
            get { return nominal; }
            set { nominal = value; }
        }

        private int max;
        public int Max
        {
            get { return max; }
            set { max = value; }
        }

        private string unit;
        public string Unit
        {
            get { return unit; }
            set { unit = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }

    public sealed class PsiConfigHandler
    {
        private ObservableCollection<PsiConfig> configs = new ObservableCollection<PsiConfig>();
        public ObservableCollection<PsiConfig> Configs
        {
            get => configs;
            private set => configs = value;
        }

        public async Task<ObservableCollection<PsiConfig>> LoadConfigs(string path)
        {
            try
            {
                Configs = await Serializer.LoadJson<ObservableCollection<PsiConfig>>(path);
            }
            catch
            {
                Configs = new ObservableCollection<PsiConfig> {
                    new PsiConfig{ Name="PSI Current", Unit="[mA]"},
                    new PsiConfig {Name="Reset Trigger", Unit="[mA]"},
                    new PsiConfig {Name="Reset Time", Unit="[us]"},
                    new PsiConfig {Name="Bittime - Vl", Unit="[us]"},
                    new PsiConfig {Name="Bittime - L", Unit="[us]"},
                    new PsiConfig {Name="Bittime - H", Unit="[us]"},
                    new PsiConfig {Name="Sync pulse trigger", Unit="[V]"},
                    new PsiConfig {Name="Sync pulse distance", Unit="[us]"},
                };
                await SaveConfigs(path);
            }
            return Configs;
        }
        public async Task SaveConfigs(string name, string path = null) => await Serializer.SaveJson(Configs, name, path);

        public PsiConfig this[string name]
        {
            get => Configs.AsParallel().Where(x => x.Name == name).FirstOrDefault();
        }
    }
}
