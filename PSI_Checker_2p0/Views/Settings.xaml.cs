using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace PSI_Checker_2p0.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void PreviewTextAsNumber(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Double.TryParse(e.Text, out _);
        }

        private void CheckTextAsNumber(object sender, TextChangedEventArgs e)
        {
            e.Handled = !Double.TryParse(e.ToString(), out _);
        }
    }
}
