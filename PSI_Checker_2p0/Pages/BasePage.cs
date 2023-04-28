using PSI_Checker_2p0.Enums;
using PSI_Checker_2p0.ViewModel.ViewModels;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PSI_Checker_2p0.Pages
{
    public class BasePage<VM> : Page
        where VM : BaseVM, new()
    {
        #region Private Members
        private VM mViewModel;
        #endregion

        #region Public Properties

        public PageAnimation PageLoadAnimation { get; set; } = PageAnimation.SlideAndFadeInFromRight;
        public PageAnimation PageUnloadAnimation { get; set; } = PageAnimation.SlideAndFadeOutToLeft;
        public float SlideSeconds { get; set; } = 0.8f;
        public VM ViewModel
        {
            get => mViewModel;
            set
            {
                if (mViewModel == value) return;

                mViewModel = value;

                this.DataContext = mViewModel;
            }
        }
        #endregion

        #region Constructor
        public BasePage()
        {
            if (this.PageLoadAnimation != PageAnimation.None)
                this.Visibility = System.Windows.Visibility.Collapsed;

            this.Loaded += BasePage_Loaded;
            this.ViewModel = new VM();
        }

        #endregion

        #region Animation Handle
        private async void BasePage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await AnimateIn();
        }

        public async Task AnimateIn()
        {
            if (this.PageLoadAnimation == PageAnimation.None)
                return;

            switch (this.PageLoadAnimation)
            {
                case PageAnimation.SlideAndFadeInFromRight:
                    await this.SlideAndFadeInFromRight(this.SlideSeconds);
                    break;
            }
        }

        public async Task AnimateOut()
        {
            if (this.PageLoadAnimation == PageAnimation.None)
                return;

            switch (this.PageUnloadAnimation)
            {
                case PageAnimation.SlideAndFadeOutToLeft:
                    await this.SlideAndFadeOutToLeft(this.SlideSeconds);
                    break;
            }
        }
        #endregion
    }
}
