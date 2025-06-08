using System.Windows;

namespace Погодка
{
    public static class Helpers
    {
        public static void ErrorShow(string message, string title = "Помилка")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void InfoShow(string message, string title = "Інформація")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
    