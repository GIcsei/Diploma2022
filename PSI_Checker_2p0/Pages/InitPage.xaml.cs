using PSI_Checker_2p0.ViewModel.ViewModels;
using System;
using System.Windows;

namespace PSI_Checker_2p0.Pages
{
    /// <summary>
    /// Interaction logic for InitPage.xaml
    /// </summary>
    public partial class InitPage : BasePage<InitVM>
    {
        public InitPage()
        {
            InitializeComponent();
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            ErrorBlock.Visibility = Visibility.Collapsed;
        }
    }
}
