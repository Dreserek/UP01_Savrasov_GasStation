using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UP01_Savrasov_GasStation.Pages
{
    public partial class RegPage : Page
    {
        public RegPage() { InitializeComponent(); }

        private void TbRegLogin_TextChanged(object sender, TextChangedEventArgs e) => LblRegLoginPh.Visibility = string.IsNullOrEmpty(TbRegLogin.Text) ? Visibility.Visible : Visibility.Hidden;
        private void TbRegFio_TextChanged(object sender, TextChangedEventArgs e) => LblRegFioPh.Visibility = string.IsNullOrEmpty(TbRegFio.Text) ? Visibility.Visible : Visibility.Hidden;
        private void PbRegPass1_PasswordChanged(object sender, RoutedEventArgs e) => LblRegPass1Ph.Visibility = string.IsNullOrEmpty(PbRegPass1.Password) ? Visibility.Visible : Visibility.Hidden;
        private void PbRegPass2_PasswordChanged(object sender, RoutedEventArgs e) => LblRegPass2Ph.Visibility = string.IsNullOrEmpty(PbRegPass2.Password) ? Visibility.Visible : Visibility.Hidden;

        private void LblRegLoginPh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => TbRegLogin.Focus();
        private void LblRegFioPh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => TbRegFio.Focus();
        private void LblRegPass1Ph_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => PbRegPass1.Focus();
        private void LblRegPass2Ph_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => PbRegPass2.Focus();

        private void BtnDoRegister_Click(object sender, RoutedEventArgs e)
        {
            string login = TbRegLogin.Text.Trim();
            string fio = TbRegFio.Text.Trim();
            string p1 = PbRegPass1.Password;
            string p2 = PbRegPass2.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(fio) || string.IsNullOrEmpty(p1) || string.IsNullOrEmpty(p2))
            { MessageBox.Show("Заполните все поля!"); return; }

            if (p1.Length < 6) { MessageBox.Show("Пароль должен быть от 6 символов!"); return; }
            if (!Regex.IsMatch(p1, @"^[a-zA-Z0-9]+$")) { MessageBox.Show("Только англ. буквы и цифры!"); return; }
            if (!Regex.IsMatch(p1, @"[0-9]")) { MessageBox.Show("Нужна хотя бы одна цифра!"); return; }
            if (p1 != p2) { MessageBox.Show("Пароли не совпадают!"); return; }

            try
            {
                using (var db = new DB_Savrasov_GasStationEntities1())
                {
                    if (db.Users.Any(u => u.Login == login)) { MessageBox.Show("Логин занят!"); return; }
                    db.Users.Add(new Users { Login = login, FIO = fio, Password = HashHelper.GetHash(p1), Role = (CbRole.SelectedItem as ComboBoxItem).Content.ToString() });
                    db.SaveChanges();
                    MessageBox.Show("Успешно!");
                    NavigationService?.GoBack();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) => NavigationService?.GoBack();
    }
}