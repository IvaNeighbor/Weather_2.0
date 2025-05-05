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

namespace Погодка
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginTextBox.Text;
            string password = PasswordPasswordBox.Password;

            if (username == "admin" && password == "1111")
            {
                // Просто оновлюємо роль у головному вікні
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.SetUserRole("admin"); // Встановлюємо роль для admin
                }
                this.Close(); // Закриваємо вікно авторизації
            }
            else if (username == "user" && password == "1111")
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.SetUserRole("user"); // Встановлюємо роль для user
                }
                this.Close(); // Закриваємо вікно авторизації
            }
            else
            {
                MessageBox.Show("Невірний логін або пароль.");
            }
        }

    }


}
