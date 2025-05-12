using System;
using System.Windows;

namespace Погодка
{
    public partial class MainWindow : Window
    {
        private string userRole;

        public MainWindow()
        {
            InitializeComponent();
            LoadWeatherData();
            AuthButton.Click += AuthButton_Click;
            EditButton.Click += EditButton_Click;
            FilterButton.Click += FilterButton_Click;
            FileButton.Click += FileButton_Click;
        }

        public void SetUserRole(string role)
        {
            userRole = role;
            if (userRole == "admin")
            {
                EditButton.IsEnabled = true;
            }
            else
            {
                EditButton.IsEnabled = false;
            }
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (userRole == "admin")
            {
                EditWeatherWindow editWindow = new EditWeatherWindow();
                editWindow.ShowDialog();
                LoadWeatherData();
            }
            else
            {
                MessageBox.Show("У вас немає доступу до редагування.", "Доступ заборонено", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilterPanel.Visibility == Visibility.Visible)
            {
                FilterPanel.Visibility = Visibility.Collapsed;
                MainContentGrid.ColumnDefinitions[0].Width = new GridLength(0);
            }
            else
            {
                FilterPanel.Visibility = Visibility.Visible;
                MainContentGrid.ColumnDefinitions[0].Width = new GridLength(180);
            }
        }

        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFile saveWindow = new SaveFile();
            saveWindow.ShowDialog();
        }

        private void LoadWeatherData()
        {
            try
            {
                DatabaseManager dbManager = new DatabaseManager();
                var weatherData = dbManager.GetAllWeatherData();

                if (weatherData == null || weatherData.Count == 0)
                {
                    MessageBox.Show("Дані відсутні у базі даних.", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                    WeatherDataGrid.ItemsSource = null;
                }
                else
                {
                    WeatherDataGrid.ItemsSource = weatherData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не вдалося завантажити базу даних:\n" + ex.Message,
                                "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                WeatherDataGrid.ItemsSource = null;
            }
        
        }
    }

}
