using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01_Savrasov_GasStation.Pages
{
    public partial class PaymentTabPage : Page
    {
        public PaymentTabPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new DB_Savrasov_GasStationEntities1())
            {
                DgPayments.ItemsSource = db.Sales
                                           .Include(s => s.Users)
                                           .Include(s => s.PaymentMethods)
                                           .ToList();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddPaymentPage(null));

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var p = (sender as Button).DataContext as Sales;
            NavigationService?.Navigate(new AddPaymentPage(p));
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            var selected = DgPayments.SelectedItems.Cast<Sales>().ToList();
            if (selected.Count == 0) return;

            if (MessageBox.Show($"Удалить {selected.Count} записей?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new DB_Savrasov_GasStationEntities1())
                    {
                        foreach (var p in selected)
                        {
                            var saleToDelete = db.Sales.Find(p.ID);
                            if (saleToDelete != null)
                            {
                                db.Sales.Remove(saleToDelete);
                            }
                        }
                        db.SaveChanges();
                    }
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message);
                }
            }
        }
    }
}