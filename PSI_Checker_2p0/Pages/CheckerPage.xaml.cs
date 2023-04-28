using PSI_Checker_2p0.Pages;
using PSI_Checker_2p0.ViewModel.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace PSI_Checker_2p0
{
    /// <summary>
    /// Interaction logic for CheckerPage.xaml
    /// </summary>
    public partial class CheckerPage : BasePage<CheckerPageVM>
    {
        public CheckerPage()
        {
            InitializeComponent();
        }

        private void LanguageSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                Localization.TranslationManager.SetLanguage((sender as ComboBox).SelectedValue as string);
            }
            catch
            {
                Localization.TranslationManager.SetLanguage("");
            }
            finally { e.Handled = true; }
        }
    }
}
