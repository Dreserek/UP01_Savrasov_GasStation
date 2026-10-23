using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UP01_Savrasov_GasStation.Pages
{
    public class CartItem
    {
        public int? FuelTypeID { get; set; }
        public int? ServiceID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }

    public partial class MakeSalePage : Page
    {
        private Users _currentUser;
        private List<CartItem> _cart = new List<CartItem>();

        public MakeSalePage(Users user)
        {
            InitializeComponent();
            _currentUser = user;

            using (var db = new DB_Savrasov_GasStationEntities1())
            {
                CbFuel.ItemsSource = db.FuelTypes.ToList();
                CbService.ItemsSource = db.Services.ToList();
                CbPaymentMethod.ItemsSource = db.PaymentMethods.ToList();
            }
            DgCart.ItemsSource = _cart;
        }

        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"[0-9]")) e.Handled = true;
        }

        private void BtnAddFuel_Click(object sender, RoutedEventArgs e)
        {
            if (CbFuel.SelectedItem == null || string.IsNullOrWhiteSpace(TbFuelQty.Text)) return;

            var fuel = CbFuel.SelectedItem as FuelTypes;
            decimal qty = Convert.ToDecimal(TbFuelQty.Text);
            if (qty <= 0) return;

            _cart.Add(new CartItem
            {
                FuelTypeID = fuel.ID,
                Name = $"Топливо: {fuel.Name}",
                Price = fuel.Price,
                Quantity = qty
            });

            UpdateCart();
            TbFuelQty.Clear();
        }

        private void BtnAddService_Click(object sender, RoutedEventArgs e)
        {
            if (CbService.SelectedItem == null || string.IsNullOrWhiteSpace(TbServiceQty.Text)) return;

            var service = CbService.SelectedItem as Services;
            decimal qty = Convert.ToDecimal(TbServiceQty.Text);
            if (qty <= 0) return;

            _cart.Add(new CartItem
            {
                ServiceID = service.ID,
                Name = $"Услуга: {service.Name}",
                Price = service.Price,
                Quantity = qty
            });

            UpdateCart();
            TbServiceQty.Clear();
        }

        private void UpdateCart()
        {
            DgCart.ItemsSource = null;
            DgCart.ItemsSource = _cart;

            decimal total = _cart.Sum(c => c.Total);
            TbTotalAmount.Text = total.ToString("N2") + " руб.";
        }

        private void BtnCheckout_Click(object sender, RoutedEventArgs e)
        {
            if (_cart.Count == 0) { MessageBox.Show("Корзина пуста!"); return; }
            if (CbPaymentMethod.SelectedItem == null) { MessageBox.Show("Выберите способ оплаты!"); return; }

            try
            {
                using (var db = new DB_Savrasov_GasStationEntities1())
                {
                    var newSale = new Sales
                    {
                        UserID = _currentUser.ID,
                        PaymentMethodID = (CbPaymentMethod.SelectedItem as PaymentMethods).ID,
                        Date = DateTime.Now,
                        TotalAmount = _cart.Sum(c => c.Total)
                    };

                    db.Sales.Add(newSale);
                    db.SaveChanges();

                    foreach (var item in _cart)
                    {
                        var saleItem = new SaleItems
                        {
                            SaleID = newSale.ID,
                            FuelTypeID = item.FuelTypeID,
                            ServiceID = item.ServiceID,
                            Quantity = item.Quantity,
                            Price = item.Price
                        };
                        db.SaleItems.Add(saleItem);
                    }

                    db.SaveChanges();
                }

                MessageBox.Show("Продажа успешно проведена! Чек сохранен.", "Успех");
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}