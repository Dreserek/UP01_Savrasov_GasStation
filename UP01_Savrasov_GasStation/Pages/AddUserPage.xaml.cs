using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01_Savrasov_GasStation.Pages
{
    public partial class AddUserPage : Page
    {
        private Users _user;

        public AddUserPage(Users selectedUser)
        {
            InitializeComponent();
            _user = selectedUser ?? new Users();
            DataContext = _user;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbLogin.Text) || string.IsNullOrWhiteSpace(TbFio.Text))
            { MessageBox.Show("Заполните данные!"); return; }

            try
            {
                using (var db = new DB_Savrasov_GasStationEntities1())
                {
                    if (_user.ID == 0)
                    {
                        _user.Login = TbLogin.Text;
                        _user.FIO = TbFio.Text;
                        _user.Role = (CbRole.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "User";
                        if (!string.IsNullOrEmpty(TbPassword.Text)) _user.Password = HashHelper.GetHash(TbPassword.Text);
                        db.Users.Add(_user);
                    }
                    else
                    {
                        var dbUser = db.Users.Find(_user.ID);
                        dbUser.Login = TbLogin.Text;
                        dbUser.FIO = TbFio.Text;
                        dbUser.Role = (CbRole.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "User";
                        if (!string.IsNullOrEmpty(TbPassword.Text)) dbUser.Password = HashHelper.GetHash(TbPassword.Text);
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