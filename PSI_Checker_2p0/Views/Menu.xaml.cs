using PSI_Checker_2p0.ViewModel;
using PSI_Checker_2p0.ViewModel.ViewModels;
using System.Windows;

namespace PSI_Checker_2p0
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    /// 
    // TODO
    /*
     * Auto mode: Egyesével végig viszi a patterneket.
     * Raw data plotting: Power off után zoomolható/használható maradjon
     *                    Jobb gombok? :D
     * Scalelődő ablak
     * PSI5Protocol összeállítás: Ne dropdown, hanem clickbox
     * Init és beállítások lap egy oldalon
     * Mérés indítás és leállítás egy gombbal + Power gomb --> Bezárás esetén mindet kezelje le.
     * Control panel + táblázat + plot három ablakban
     * Különleges feature-ök külön ablakban: PSI conformity, PSI ATT
     * 
     */
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
            this.DataContext = DIService.Instance.Get<MenuVM>();
        }
    }
}
