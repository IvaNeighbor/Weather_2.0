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
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.SetUserRole("admin");
                }
                this.Close();
            }
            else if (username == "user" && password == "1111")
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.SetUserRole("user");
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Невірний логін або пароль.");
            }
        }

    }


}
