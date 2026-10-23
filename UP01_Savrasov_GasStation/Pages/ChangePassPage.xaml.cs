using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UP01_Savrasov_GasStation.Pages
{
    public partial class ChangePassPage : Page
    {
        public ChangePassPage() { InitializeComponent(); }

        private void TbLogin_TextChanged(object sender, TextChangedEventArgs e) => LblLoginPh.Visibility = string.IsNullOrEmpty(TbLogin.Text) ? Visibility.Visible : Visibility.Hidden;
        private void PbOldPass_PasswordChanged(object sender, RoutedEventArgs e) => LblOldPassPh.Visibility = string.IsNullOrEmpty(PbOldPass.Password) ? Visibility.Visible : Visibility.Hidden;
        private void PbNewPass_PasswordChanged(object sender, RoutedEventArgs e) => LblNewPassPh.Visibility = string.IsNullOrEmpty(PbNewPass.Password) ? Visibility.Visible : Visibility.Hidden;

        private void LblLoginPh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => TbLogin.Focus();
        private void LblOldPassPh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => PbOldPass.Focus();
        private void LblNewPassPh_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => PbNewPass.Focus();

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TbLogin.Text) || string.IsNullOrEmpty(PbOldPass.Password) || string.IsNullOrEmpty(PbNewPass.Password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (PbNewPass.Password.Length < 6)
            {
                MessageBox.Show("Пароль должен быть от 6 символов!");
                return;
            }
            if (!Regex.IsMatch(PbNewPass.Password, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("Только англ. буквы и цифры!");
                return;
            }
            if (!Regex.IsMatch(PbNewPass.Password, @"[0-9]"))
            {
                MessageBox.Show("Нужна хотя бы одна цифра!");
                return;
            }

            try
            {
                using (var db = new DB_Savrasov_GasStationEntities1())
                {
                    var user = db.Users.FirstOrDefault(u => u.Login == TbLogin.Text && u.Password == HashHelper.GetHash(PbOldPass.Password));

                    if (user == null)
                    {
                        MessageBox.Show("Неверный логин или старый пароль!");
                        return;
                    }

                    user.Password = HashHelper.GetHash(PbNewPass.Password);
                    db.SaveChanges();

                    MessageBox.Show("Пароль успешно изменен! Войдите с новым паролем.");

                    NavigationService?.Navigate(new AuthPage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка БД: " + ex.Message);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) => NavigationService?.GoBack();
    }
}