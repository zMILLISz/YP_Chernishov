using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YP_Chernishov.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private AuthService _authService;
        public AuthPage()
        {
            InitializeComponent();

            _authService = new AuthService();
        }

        private void TextBoxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtHintLogin.Visibility = Visibility.Visible;
            if (TextBoxLogin.Text.Length > 0)
            {
                txtHintLogin.Visibility = Visibility.Hidden;
            }
        }

        private void EntranceBut_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHintLogin.Text) || string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            var user = _authService.Login(TextBoxLogin.Text, PasswordBox.Password);

            if (user == null)
            {
                MessageBox.Show("Пользователь с такими данными не найден!");
                return;
            }
            else
            {
                MessageBox.Show("Пользователь успешно найден!");

                NavigateToUserPage((int)user.UserRole, TextBoxLogin.Text);
            }
        }

        private void NavigateToUserPage(int userRole, string userLogin)
        {
            switch (userRole)
            {
                case 1:
                    NavigationService.Navigate(new AdminPage(userLogin));
                    break;
                case 2:
                    NavigationService.Navigate(new SpecialistPage(userLogin));
                    break;
                case 3:
                    NavigationService.Navigate(new UserPage(userLogin));
                    break;
            }
        }

        private void RegBut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }
    }
}
