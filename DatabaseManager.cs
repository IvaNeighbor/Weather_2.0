using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Погодка
{
    public class DatabaseManager
    {
        string connectionString = "server=127.0.1.1;port=3306;database=WeatherData;User id=root;pwd=234234234;";


        public List<WeatherInfo> GetAllWeatherData()
        {
            var weatherData = new List<WeatherInfo>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT Day, Month, Temperature, Precipitation, Pressure FROM weather";
                using (var command = new MySqlCommand(query, connection))
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

            return weatherData;
        }

    }
}
