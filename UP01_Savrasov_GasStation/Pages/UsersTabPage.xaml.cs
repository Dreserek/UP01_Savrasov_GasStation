using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UP01_Savrasov_GasStation.Pages
{
    public partial class UsersTabPage : Page
    {
        public UsersTabPage()
        {
            InitializeComponent();
            DgUsers.ItemsSource = new DB_Savrasov_GasStationEntities1().Users.ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddUserPage(null));

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button).DataContext as Users;
            NavigationService?.Navigate(new AddUserPage(user));
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            var selected = DgUsers.SelectedItems.Cast<Users>().ToList();
            if (selected.Count == 0) return;

            if (MessageBox.Show($"Удалить {selected.Count} записей?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new DB_Savrasov_GasStationEntities1())
                    {
                        foreach (var user in selected)
                        {
                            var userToDelete = db.Users.Find(user.ID);
                            if (userToDelete != null)
                            {
                                db.Users.Remove(userToDelete);
                            }
                        }
                        db.SaveChanges();
                    }
                    DgUsers.ItemsSource = new DB_Savrasov_GasStationEntities1().Users.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message);
                }
            }
        }
    }
}