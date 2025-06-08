using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using static System.Net.Mime.MediaTypeNames;

namespace Погодка
{
    public partial class EditWeatherWindow : Window
    {
        public EditWeatherWindow()
        {
            InitializeComponent();
            AddRecordButton.Click += AddRecordButton_Click;
            DeleteRecordButton.Click += DeleteRecordButton_Click;
        }

        private void AddRecordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AddDateDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Будь ласка, оберіть дату для додавання.", "Помилка вводу", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DateTime selectedDate = AddDateDatePicker.SelectedDate.Value;

                int day = selectedDate.Day;
                int month = selectedDate.Month;

                string temperature = AddTemperatureTextBox.Text.Trim();
                if (string.IsNullOrEmpty(temperature))
                {
                    MessageBox.Show("Будь ласка, введіть значення для Температури.", "Помилка вводу", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Додайте валідацію для температури, якщо вона має бути числом

                int precipitationValue;
                if (!int.TryParse(AddPrecipitationTextBox.Text.Trim(), out precipitationValue) || (precipitationValue != 0 && precipitationValue != 1))
                {
                    MessageBox.Show("Будь ласка, введіть 0 (Ні) або 1 (Так) для Наявності опадів.", "Помилка вводу", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string pressure = AddWindTextBox.Text.Trim();
                if (string.IsNullOrEmpty(pressure))
                {
                    MessageBox.Show("Будь ласка, введіть значення для Тиску.", "Помилка вводу", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Додайте валідацію для тиску, якщо він має бути числом


                using (var connection = new MySqlConnection("server=127.0.0.1;port=3306;database=WeatherData;User id=root;pwd=234234234;"))
                {
                    connection.Open();

                    // *** ЦЕЙ РЯДОК БУВ ЗМІНЕНИЙ ***
                    string query = @"
                INSERT INTO weather (Day, Month, Temperature, Precipitation, Pressure)
                VALUES (@Day, @Month, @Temperature, @Precipitation, @Pressure)
                ON DUPLICATE KEY UPDATE
                    Temperature = VALUES(Temperature),
                    Precipitation = VALUES(Precipitation),
                    Pressure = VALUES(Pressure);";
                    // ***************************

                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    cmd.Parameters.AddWithValue("@Day", day);
                    cmd.Parameters.AddWithValue("@Month", month);
                    cmd.Parameters.AddWithValue("@Temperature", temperature);
                    cmd.Parameters.AddWithValue("@Precipitation", precipitationValue);
                    cmd.Parameters.AddWithValue("@Pressure", pressure);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Helpers.InfoShow("Запис успішно додано або оновлено!", "Успіх");
                    }
                    else
                    {
                        Helpers.ErrorShow("Не вдалося додати або оновити запис.", "Помилка");
                    }


                    // Очищення полів після успіху
                    AddDateDatePicker.SelectedDate = null;
                    AddTemperatureTextBox.Clear();
                    AddPrecipitationTextBox.Clear();
                    AddWindTextBox.Clear();
                }
            }
            catch (MySqlException ex)
            {
                Helpers.ErrorShow($"Помилка бази даних: {ex.Message}\nКод помилки: {ex.Number}", "Помилка DB");
            }
            catch (Exception ex)
            {
                Helpers.ErrorShow($"Невідома помилка: {ex.Message}\nТип помилки: {ex.GetType().Name}", "Загальна помилка");
            }
        }

        private void DeleteRecordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DeleteDateDatePicker.SelectedDate == null)
                {
                   Helpers.InfoShow("Будь ласка, оберіть дату для видалення.");
                    return;
                }

                DateTime selectedDate = DeleteDateDatePicker.SelectedDate.Value;

                int day = selectedDate.Day;
                int month = selectedDate.Month;

                using (var connection = new MySqlConnection("server=127.0.0.1;port=3306;database=WeatherData;User id=root;pwd=234234234;"))
                {
                    connection.Open();
                    string query = "DELETE FROM weather WHERE Day = @Day AND Month = @Month";

                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Day", day);
                    cmd.Parameters.AddWithValue("@Month", month);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                        Helpers.InfoShow($"Видалено запис");
                    else
                        Helpers.InfoShow("Запис за вказаною датою (день і місяць) не знайдено.");
                }
            }
            catch (Exception ex)
            {
                Helpers.ErrorShow("Помилка видалення: " + ex.Message);
            }
        }
    }
}
