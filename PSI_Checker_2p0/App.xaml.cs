using PSI_Checker_2p0.ViewModel;
using System.Windows;

namespace PSI_Checker_2p0
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected App()
        {

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DIService.Logger.Log("Application starting...", Logger.LogLevel.Debug);
            Menu app = new Menu();
            app.Show();
            DIService.Logger.Log("Application started succesfully", Logger.LogLevel.Debug);
        }
    }
}
