using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
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
            var weatherData = dbManager.GetAllWeatherData();

            StringBuilder content = new StringBuilder();

            if (SaveOption1CheckBox.IsChecked == true)
            {
                // Фільтруємо дні, де температура > 0 і йшов дощ (Precipitation == "Так")
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
                if (weatherData.Count > 0)
                {
                    double avgTemp = 0;
                    double avgPressure = 0;

                    foreach (var w in weatherData)
                    {
                        avgTemp += w.Temperature;
                        avgPressure += w.Pressure;
                    }

                    avgTemp /= weatherData.Count;
                    avgPressure /= weatherData.Count;

                    content.AppendLine("Середньомісячна температура та середній тиск:");
                    content.AppendLine($" - Температура: {avgTemp:F1}°C");
                    content.AppendLine($" - Тиск: {avgPressure:F0} гПа");
                }
                else
                {
                    content.AppendLine("Дані відсутні для розрахунку середніх значень.");
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
