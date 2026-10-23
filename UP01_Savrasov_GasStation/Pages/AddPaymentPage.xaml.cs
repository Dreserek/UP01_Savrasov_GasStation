using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UP01_Savrasov_GasStation.Pages
{
    public partial class AddPaymentPage : Page
    {
        private Sales _sale; // Используем класс Sales вместо Payment

        public AddPaymentPage(Sales selected)
        {
            InitializeComponent();
            _sale = selected ?? new Sales();
            DataContext = _sale;

            using (var db = new DB_Savrasov_GasStationEntities1())
            {
                CbUser.ItemsSource = db.Users.ToList();
                CbPaymentMethod.ItemsSource = db.PaymentMethods.ToList();
            }
        }

        private void TbNum_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"[0-9,\.]")) e.Handled = true;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbPrice.Text) || DpDate.SelectedDate == null || CbUser.SelectedValue == null || CbPaymentMethod.SelectedValue == null)
            {
                MessageBox.Show("Заполните все поля корректно!");
                return;
            }

            try
            {
                using (var db = new DB_Savrasov_GasStationEntities1())
                {
                    if (_sale.ID == 0)
                    {
                        _sale.Date = DpDate.SelectedDate.Value;
                        _sale.TotalAmount = Convert.ToDecimal(TbPrice.Text);
                        _sale.UserID = (int)CbUser.SelectedValue;
                        _sale.PaymentMethodID = (int)CbPaymentMethod.SelectedValue;
                        db.Sales.Add(_sale);
                    }
                    else
                    {
                        var dbSale = db.Sales.Find(_sale.ID);
                        dbSale.Date = DpDate.SelectedDate.Value;
                        dbSale.TotalAmount = Convert.ToDecimal(TbPrice.Text);
                        dbSale.UserID = (int)CbUser.SelectedValue;
                        dbSale.PaymentMethodID = (int)CbPaymentMethod.SelectedValue;
                    }
                    db.SaveChanges();
                }
                MessageBox.Show("Сохранено!");
                NavigationService?.GoBack();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}