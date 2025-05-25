using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using MySql.Data.MySqlClient;

namespace Погодка
{
    public class DatabaseManager
    {
        string connectionString = "server=127.0.0.1;port=3306;database=WeatherData;User id=root;pwd=234234234;";


        public List<WeatherInfo> GetAllWeatherData()
        {
            var weatherData = new List<WeatherInfo>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Оновлений запит SELECT з ORDER BY
                string query = "SELECT Day, Month, Temperature, Precipitation, Pressure FROM weather ORDER BY Month ASC, Day ASC";
                // ASC означає сортування за зростанням (від меншого до більшого)

                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var weather = new WeatherInfo
                        {
                            Day = reader.GetInt32("Day"),
                            Month = reader.GetInt32("Month"),
                            Temperature = reader.GetDouble("Temperature"), // Переконайтеся, що Temperature в базі даних має числовий тип
                            Precipitation = reader.GetBoolean("Precipitation") ? "Так" : "Ні", // Це читає 0/1 як булеве
                            Pressure = reader.GetInt32("Pressure") // Переконайтеся, що Pressure в базі даних має числовий тип
                        };

                        weatherData.Add(weather);
                    }
                }
            }

            return weatherData;
        }
        public List<WeatherInfo> GetFilteredWeatherData(int? month, int? day)
{
    var weatherData = new List<WeatherInfo>();

    using (var connection = new MySqlConnection(connectionString))
    {
        connection.Open();

        // Формуємо запит динамічно в залежності від переданих параметрів
        StringBuilder queryBuilder = new StringBuilder("SELECT Day, Month, Temperature, Precipitation, Pressure FROM weather");
        List<string> conditions = new List<string>();

        if (month.HasValue)
            conditions.Add("Month = @month");
        if (day.HasValue)
            conditions.Add("Day = @day");

        if (conditions.Count > 0)
            queryBuilder.Append(" WHERE " + string.Join(" AND ", conditions));

        queryBuilder.Append(" ORDER BY Month ASC, Day ASC");

        using (var command = new MySqlCommand(queryBuilder.ToString(), connection))
        {
            if (month.HasValue)
                command.Parameters.AddWithValue("@month", month.Value);
            if (day.HasValue)
                command.Parameters.AddWithValue("@day", day.Value);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var weather = new WeatherInfo
                    {
                        Day = reader.GetInt32("Day"),
                        Month = reader.GetInt32("Month"),
                        Temperature = reader.GetDouble("Temperature"),
                        Precipitation = reader.GetBoolean("Precipitation") ? "Так" : "Ні",
                        Pressure = reader.GetInt32("Pressure")
                    };
                    weatherData.Add(weather);
                }
            }
        }
    }
    return weatherData;
}

    }
}
