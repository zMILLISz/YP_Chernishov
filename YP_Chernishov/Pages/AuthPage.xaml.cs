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
        public AuthPage()
        {
            InitializeComponent();
        }

        private void TextBoxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtHintLogin.Visibility = Visibility.Visible;
            if (TextBoxLogin.Text.Length > 0)
            {
                txtHintLogin.Visibility = Visibility.Hidden;
            }
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return
                string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void EntranceBut_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHintLogin.Text) || string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            string PassHash = GetHash(PasswordBox.Password);

            using (var db = new YP_ChernishovEntities())
            {
                var user = db.User
                    .AsNoTracking()
                    .FirstOrDefault(u => u.UserLogin == TextBoxLogin.Text && u.UserPassword == PassHash);

                if (user == null)
                {
                    MessageBox.Show("Пользователь с такими данными не найден!");
                    return;
                }
                else
                {
                    MessageBox.Show("Пользователь успешно найден!");

                    switch (user.UserRole)
                    {
                        case 1:
                            NavigationService.Navigate(new AdminPage(TextBoxLogin.Text));
                            break;
                        case 2:
                            NavigationService.Navigate(new SpecialistPage(TextBoxLogin.Text));
                            break;
                        case 3:
                            NavigationService.Navigate(new UserPage(TextBoxLogin.Text));
                            break;
                    }
                }
            }
        }

        private void RegBut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }
    }
}
