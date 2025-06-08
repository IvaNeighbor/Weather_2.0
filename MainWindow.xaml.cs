using System;
using System.Windows;

namespace Погодка
{
    public partial class MainWindow : Window
    {
        private string userRole;
        DatabaseManager dbManager = new DatabaseManager();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public MainWindow()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
            InitializeComponent();
            LoadWeatherData();
            AuthButton.Click += AuthButton_Click;
            EditButton.Click += EditButton_Click;
            FilterButton.Click += FilterButton_Click;
            FileButton.Click += FileButton_Click;
            ApplyFilterButton.Click += ApplyFilterButton_Click;
            ClearFilterButton.Click += ClearFilterButton_Click;

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
                Helpers.InfoShow("У вас немає доступу до редагування.", "Доступ заборонено");
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
                    Helpers.InfoShow("Дані відсутні у базі даних.", "Інформація");
                    WeatherDataGrid.ItemsSource = null;
                }
                else
                {
                    WeatherDataGrid.ItemsSource = weatherData;
                }
            }
            catch (Exception ex)
            {
                Helpers.ErrorShow("Не вдалося завантажити базу даних:\n" + ex.Message,
                                "Помилка");
                WeatherDataGrid.ItemsSource = null;
            }
        
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            int? selectedMonth = null;
            int? selectedDay = null;

            if (MonthComboBox.SelectedIndex >= 0)
            {
                // Місяці у комбоінбоксі з індексу 0 (Січень) = 1 у числовому форматі
                selectedMonth = MonthComboBox.SelectedIndex + 1;
            }

            if (DayComboBox.SelectedIndex >= 0)
            {
                // Дні у комбоінбоксі з індексу 0 (1) = 1 у числовому форматі
                selectedDay = DayComboBox.SelectedIndex + 1;
            }

            try
            {
                var filteredData = dbManager.GetFilteredWeatherData(selectedMonth, selectedDay);
                WeatherDataGrid.ItemsSource = filteredData;

                if (filteredData.Count == 0)
                {
                    Helpers.InfoShow("За вашим фільтром дані не знайдені.", "Результат фільтрації");
                }
            }
            catch (Exception ex)
            {
                Helpers.ErrorShow("Помилка при завантаженні даних з фільтром:\n" + ex.Message,
                                "Помилка");
            }
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Скидаємо вибір місяця та дня
            MonthComboBox.SelectedIndex = -1;
            DayComboBox.SelectedIndex = -1;

            // Завантажуємо всі дані без фільтрації
            LoadWeatherData();
        }


    }

}
