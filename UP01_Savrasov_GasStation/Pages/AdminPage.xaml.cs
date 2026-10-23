using System.Windows;
using System.Windows.Controls;

namespace UP01_Savrasov_GasStation.Pages
{
    public partial class AdminPage : Page
    {
        public AdminPage() { InitializeComponent(); }
        private void BtnUsers_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new UsersTabPage());
        private void BtnPayments_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new PaymentTabPage());
        private void BtnDiagrams_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new DiagramPage());
    }
}