using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace UP01_Savrasov_GasStation
{
    public partial class MainWindow : Window
    {
        private int _currentTheme = 1;

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigated += MainFrame_Navigated;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new System.Windows.Threading.DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, args) => TbDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            timer.Start();
            TbDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                e.Cancel = true;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack) MainFrame.GoBack();
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is Page page) TbPageTitle.Text = page.Title ?? "АЗС";
        }

        private void BtnTheme_Click(object sender, RoutedEventArgs e)
        {
            _currentTheme++;
            if (_currentTheme > 3) _currentTheme = 1;

            string themeFile = "DictionaryLight.xaml";
            if (_currentTheme == 2) themeFile = "DictionaryDark.xaml";
            if (_currentTheme == 3) themeFile = "DictionaryAlt.xaml";

            var uri = new Uri($"Resources/{themeFile}", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;

            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
    }
}