using PSI_Checker_2p0.Enums;
using PSI_Checker_2p0.ViewModel.Properties;
using System.Windows;

namespace PSI_Checker_2p0.ViewModel.ViewModels
{
    //TODO
    // Known bug of memory leak somewhere in the page change
    public class MenuVM : BaseVM
    {
        private ApplicationPage currentPage = ApplicationPage.Init;
        public ApplicationPage CurrentPage
        {
            get => currentPage;
            set => SetProperty(ref currentPage, value);
        }

        private ResizeMode resizable = ResizeMode.CanResize;
        public ResizeMode Resizable
        {
            get => resizable;
            private set => SetProperty(ref resizable, value);
        }

        private WindowStyle windowStyleMode = WindowStyle.SingleBorderWindow;
        public WindowStyle WindowStyleMode
        {
            get => windowStyleMode;
            private set => SetProperty(ref windowStyleMode, value);
        }

        private WindowState actualWindowState = WindowState.Normal;
        public WindowState ActualWindowState
        {
            get => actualWindowState;
            set => SetProperty(ref actualWindowState, value);
        }

        private Visibility visible = Visibility.Visible;
        public Visibility Visible
        {
            get => visible;
            set => SetProperty(ref visible, value);
        }

        private bool fullScreen = AppSettings.Default.FullWindow;
        public bool FullScreen
        {
            get => fullScreen;
            private set
            {
                if (SetProperty(ref fullScreen, value))
                {
                    if (fullScreen)
                    {
                        Visible = Visibility.Collapsed;
                        ActualWindowState = WindowState.Normal;
                        WindowStyleMode = WindowStyle.None;
                        Resizable = ResizeMode.NoResize;
                        ActualWindowState = WindowState.Maximized;
                        Visible = Visibility.Visible;
                    }
                    else
                    {
                        Resizable = ResizeMode.CanResize;
                        WindowStyleMode = WindowStyle.SingleBorderWindow;
                        ActualWindowState = WindowState.Maximized;
                    }
                }
            }
        }

        private double windowFontSize = AppSettings.Default.DefaultFontSize;
        public double WindowFontSize
        {
            get => windowFontSize;
            private set => SetProperty(ref windowFontSize, value);
        }

        public MenuVM()
        {
            AppSettings.Default.PropertyChanged += AppSettingsChanged;
        }

        private void AppSettingsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (nameof(AppSettings.Default.FullWindow) == e.PropertyName)
            {
                FullScreen = AppSettings.Default.FullWindow;
            };
            if (nameof(AppSettings.Default.DefaultFontSize) == e.PropertyName)
            {
                WindowFontSize = AppSettings.Default.DefaultFontSize;
            };
        }
    }
}
