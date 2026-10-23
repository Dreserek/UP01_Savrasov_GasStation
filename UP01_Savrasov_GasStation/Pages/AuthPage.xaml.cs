using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UP01_Savrasov_GasStation.Pages
{
    public partial class AuthPage : Page
    {
        private int failedAttempts = 0;

        public AuthPage() { InitializeComponent(); }

        private void TbLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            LblLoginPlaceholder.Visibility = string.IsNullOrEmpty(TbLogin.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void PbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            LblPassPlaceholder.Visibility = string.IsNullOrEmpty(PbPassword.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private void LblLoginPlaceholder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => TbLogin.Focus();
        private void LblPassPlaceholder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => PbPassword.Focus();

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TbLogin.Text) || string.IsNullOrEmpty(PbPassword.Password))
            { MessageBox.Show("Введите логин и пароль!"); return; }

            string hash = HashHelper.GetHash(PbPassword.Password);
            try
            {
                using (var db = new DB_Savrasov_GasStationEntities1())
                {
                    var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Login == TbLogin.Text && u.Password == hash);
                    if (user == null)
                    {
                        failedAttempts++;
                        MessageBox.Show("Неверный логин или пароль!");
                        if (failedAttempts >= 3) { CaptchaSwitch(); CaptchaChange(); }
                    }
                    else
                    {
                        failedAttempts = 0;
                        MessageBox.Show($"Добро пожаловать, {user.FIO}!");
                        if (user.Role == "Admin") NavigationService?.Navigate(new AdminPage());
                        else NavigationService?.Navigate(new UserPage(user));
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Ошибка БД: " + ex.Message); }
        }

        public void CaptchaSwitch()
        {
            if (CaptchaPanel.Visibility == Visibility.Hidden)
            {
                CaptchaPanel.Visibility = Visibility.Visible;
                LoginPanel.Visibility = Visibility.Hidden;
                LblTitle.Text = "Введите капчу";
            }
            else
            {
                CaptchaPanel.Visibility = Visibility.Hidden;
                LoginPanel.Visibility = Visibility.Visible;
                LblTitle.Text = "Вход в систему АЗС";
                TbLogin.Clear(); PbPassword.Clear(); TbCaptchaInput.Clear();
            }
        }

        public void CaptchaChange()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string pwd = ""; Random r = new Random();
            for (int i = 0; i < 5; i++) pwd += chars[r.Next(0, chars.Length)];
            TbCaptchaText.Text = pwd;
        }

        private void BtnSubmitCaptcha_Click(object sender, RoutedEventArgs e)
        {
            if (TbCaptchaInput.Text != TbCaptchaText.Text) { MessageBox.Show("Неверная капча!"); CaptchaChange(); }
            else { MessageBox.Show("Успешно!"); CaptchaSwitch(); failedAttempts = 0; }
        }

        private void TbCaptchaText_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste) e.Handled = true;
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new RegPage());
        private void BtnChangePass_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new ChangePassPage());
    }
}