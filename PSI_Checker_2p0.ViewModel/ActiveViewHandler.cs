using PSI_Checker_2p0.Utils;
using PSI_Checker_2p0.ViewModel.ViewModels;

namespace PSI_Checker_2p0.ViewModel
{
    public class ActiveViewHandler
    {
        public static ActiveViewHandler Instance { get; private set; } = new ActiveViewHandler();
        public MenuVM CurrentVM => DIService.Instance.Get<MenuVM>();

        public ImportantInfos CurrentInfo => DIService.Instance.Get<ImportantInfos>();
    }
}
