using System.Windows;
using System.Windows.Data;

namespace PSI_Checker_2p0.Localization
{
    public class LocalizationExtension : Binding
    {
        public LocalizationExtension() : base()
        {
            this.Mode = BindingMode.OneWay;
            this.Source = TranslationManager.Instance;
        }
        public LocalizationExtension(string name) : base("[" + name + "]")
        {
            this.Mode = BindingMode.OneWay;
            this.Source = TranslationManager.Instance;
        }

        public new PropertyPath Path
        {
            get => base.Path;
            set
            {
                value.Path = "[" + value.Path + "]";
                base.Path = value;
            }
        }
    }
}
