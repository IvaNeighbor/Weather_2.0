using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;

namespace Погодка
{
    public partial class SaveFile : Window
    {
        private DatabaseManager dbManager = new DatabaseManager();

        public SaveFile()
        {
            InitializeComponent();
            SaveFileButton.Click += SaveFileButton_Click;
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(MonthComboBox.SelectedItem is ComboBoxItem selectedMonth))
            {
                MessageBox.Show("Будь ласка, оберіть місяць.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int selectedMonthNumber = int.Parse(selectedMonth.Tag.ToString());

            var weatherData = dbManager.GetAllWeatherData();

            StringBuilder content = new StringBuilder();

            if (SaveOption1CheckBox.IsChecked == true)
            {
                // Перша опція — без фільтрації по місяцю
                var filteredDays = new List<string>();
                foreach (var day in weatherData)
                {
                    if (day.Temperature > 0 && day.Precipitation == "Так")
                    {
                        string date = $"{day.Day:D2}.{day.Month:D2}.2025";
                        filteredDays.Add(date);
                    }
                }

                content.AppendLine("Кількість днів та дати, коли температура > 0 та йшов дощ:");
                content.AppendLine($" - {filteredDays.Count} днів: {string.Join(", ", filteredDays)}");
                content.AppendLine();
            }

            if (SaveOption2CheckBox.IsChecked == true)
            {
                // Друга опція — фільтруємо за вибраним місяцем
                var monthData = weatherData.FindAll(day => day.Month == selectedMonthNumber);

                if (monthData.Count > 0)
                {
                    double avgTemp = 0;
                    double avgPressure = 0;

                    foreach (var w in monthData)
                    {
                        avgTemp += w.Temperature;
                        avgPressure += w.Pressure;
                    }

                    avgTemp /= monthData.Count;
                    avgPressure /= monthData.Count;

                    content.AppendLine("Середньомісячна температура та середній тиск:");
                    content.AppendLine($" - Температура: {avgTemp:F1}°C");
                    content.AppendLine($" - Тиск: {avgPressure:F0} гПа");
                }
                else
                {
                    content.AppendLine("Дані відсутні для розрахунку середніх значень за обраний місяць.");
                }
            }

            if (content.Length == 0)
            {
                MessageBox.Show("Будь ласка, оберіть принаймні одну опцію для збереження.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Текстові файли (*.txt)|*.txt|Всі файли (*.*)|*.*",
                Title = "Зберегти файл"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, content.ToString(), Encoding.UTF8);
                    MessageBox.Show("Файл успішно збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Сталася помилка при збереженні файлу:\n" + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
