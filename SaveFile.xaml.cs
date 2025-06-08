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
                Helpers.ErrorShow("Будь ласка, оберіть місяць.", "Помилка");
                return;
            }

            int selectedMonthNumber = 10; 

            if (selectedMonth?.Tag != null && int.TryParse(selectedMonth.Tag.ToString(), out selectedMonthNumber))
            {
                // успішно
            }
            else
            {
                Helpers.ErrorShow("Помилка: не вдалося визначити номер місяця.");
            }

            // selectedMonthNumber тут гарантовано ініціалізований

            var weatherData = dbManager.GetAllWeatherData();
            var monthData = weatherData.FindAll(day => day.Month == selectedMonthNumber);

            StringBuilder content = new StringBuilder();

            if (SaveOption1CheckBox.IsChecked == true)
            {
                var filteredDays = WeatherProcessor.GetWarmRainyDays(monthData);



                content.AppendLine("Кількість днів та дати, коли температура > 0 та йшов дощ:");
                content.AppendLine($" - {filteredDays.Count} днів: {string.Join(", ", filteredDays)}");
                content.AppendLine();
            }

            if (SaveOption2CheckBox.IsChecked == true)
            {
                if (monthData.Count > 0)
                {
                    

                    var (avgTemp, avgPressure) = WeatherProcessor.GetAverages(monthData);


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
                Helpers.ErrorShow("Будь ласка, оберіть принаймні одну опцію для збереження.", "Помилка");
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

                    Helpers.InfoShow("Файл успішно збережено у форматі .docx!", "Успіх");
                    this.Close();
                }
                catch (Exception ex)
                {
                    Helpers.ErrorShow("Сталася помилка при збереженні файлу:\n" + ex.Message, "Помилка");
                }
            }
        }


    }

    public static class WeatherProcessor
    {
        public static List<string> GetWarmRainyDays(List<WeatherInfo> monthData)
        {
            var result = new List<string>();

            foreach (var day in monthData)
            {
                if (day.Temperature > 0 && day.Precipitation == "Так")
                {
                    string date = $"{day.Day:D2}.{day.Month:D2}.2025";
                    result.Add(date);
                }
            }

            return result;
        }

        public static (double avgTemp, double avgPressure) GetAverages(List<WeatherInfo> monthData)
        {
            if (monthData == null || monthData.Count == 0)
                return (0, 0);

            double tempSum = 0;
            double pressureSum = 0;

            foreach (var item in monthData)
            {
                tempSum += item.Temperature;
                pressureSum += item.Pressure;
            }

            return (tempSum / monthData.Count, pressureSum / monthData.Count);
        }
    }

}




