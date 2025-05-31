using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

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
            var monthData = weatherData.FindAll(day => day.Month == selectedMonthNumber);

            StringBuilder content = new StringBuilder();

            if (SaveOption1CheckBox.IsChecked == true)
            {
                var filteredDays = new List<string>();
                foreach (var day in monthData)
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
                Filter = "Документ Word (*.docx)|*.docx|Всі файли (*.*)|*.*",
                Title = "Зберегти документ Word"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(
                        saveFileDialog.FileName, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        Body body = new Body();

                        foreach (string line in content.ToString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None))
                        {
                            body.AppendChild(new Paragraph(new Run(new Text(line))));
                        }

                        mainPart.Document.Append(body);
                        mainPart.Document.Save();
                    }

                    MessageBox.Show("Файл успішно збережено у форматі .docx!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
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


    

