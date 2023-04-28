using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace PSI_Checker_2p0.Localization
{
    public class TranslationManager : INotifyPropertyChanged
    {
        private static readonly TranslationManager instance = new TranslationManager();
        public static TranslationManager Instance
        {
            get => instance;
        }

        private readonly ResourceManager resManager = Localization.Resource.ResourceManager;
        private CultureInfo culture = null;

        public string this[string key]
        {
            get => resManager.GetString(key, culture);
        }

        public static void SetLanguage(string locale)
        {
            if (string.IsNullOrEmpty(locale)) locale = "en-US";
            Instance.CurrentCulture = new System.Globalization.CultureInfo(locale);
        }

        public CultureInfo CurrentCulture
        {
            get => culture ?? CultureInfo.CurrentCulture;
            set
            {
                if (this.CurrentCulture != value)
                {
                    culture = value;
                    var @event = PropertyChanged;
                    @event?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
