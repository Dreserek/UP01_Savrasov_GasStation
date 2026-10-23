using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01_Savrasov_GasStation.Pages
{
    public partial class UserPage : Page
    {
        private Users _currentUser;

        public UserPage(Users user)
        {
            InitializeComponent();
            _currentUser = user;
            UpdateData();
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e) => UpdateData();

        private void UpdateData()
        {
            if (DgSales == null) return;
            try
            {
                using (var db = new DB_Savrasov_GasStationEntities1())
                {
                    var query = db.Sales
                                  .Include(s => s.Users)
                                  .Include(s => s.PaymentMethods)
                                  .AsQueryable();

                    if (CbSort.SelectedIndex == 0)
                        query = query.OrderByDescending(p => p.Date);
                    else
                        query = query.OrderBy(p => p.Date);

                    DgSales.ItemsSource = query.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
        }

        private void BtnMakeSale_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MakeSalePage(_currentUser));
        }
    }
}