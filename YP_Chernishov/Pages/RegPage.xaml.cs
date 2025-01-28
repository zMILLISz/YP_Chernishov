using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        string connectionString = @"Data Source=DESKTOP-22U9FOV\SQLEXPRESS;initial catalog=YP_Chernishov;integrated security=True";
        public RegPage()
        {
            InitializeComponent();

            RolesBox.ItemsSource = YP_ChernishovEntities.GetContext().Role.ToList();
            RolesBox.SelectedIndex = 3;
        }
        
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return
                string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private bool IsLoginExists(string login)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM [User] WHERE UserLogin = @Login";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", login);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public static bool IsPasswordValid(string password)
        {
            if (password.Length < 6)
            {
                return false;
            }

            if (!Regex.IsMatch(password, @"^[a-zA-Z0-9]*$"))
            {
                return false;
            }

            if (!Regex.IsMatch(password, @"[^\d]*\d"))
            {
                return false;
            }

            return true;
        }

        private void RegBut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TextBoxLogin.Text))
                {
                    MessageBox.Show("Введите логин");
                    return;
                }

                if (string.IsNullOrEmpty(PasswordBox.Password) || !IsPasswordValid(PasswordBox.Password))
                {
                    MessageBox.Show("Пароль не соответствует требованиям. Пароль должен быть не менее 6 символов и содержать цифры");
                    return;
                }

                if (string.IsNullOrEmpty(PasswordBoxAgain.Password))
                {
                    MessageBox.Show("Подтвердите пароль");
                    return;
                }

                if (string.IsNullOrEmpty(FIOBox.Text))
                {
                    MessageBox.Show("Введите ФИО");
                    return;
                }

                if (PasswordBox.Password != PasswordBoxAgain.Password)
                {
                    MessageBox.Show("Пароли не совпадают");
                    return;
                }

                if (IsLoginExists(TextBoxLogin.Text))
                {
                    MessageBox.Show("Логин уже существует");
                    return;
                }


                YP_ChernishovEntities db = new YP_ChernishovEntities();

                string roleName = RolesBox.Text;
                var role = db.Role.FirstOrDefault(r => r.RoleName == roleName);

                User userObject = new User
                {
                    UserFIO = FIOBox.Text,
                    UserLogin = TextBoxLogin.Text,
                    UserPassword = GetHash(PasswordBox.Password),
                    UserRole = role.RoleId
                };
                db.User.Add(userObject);
                db.SaveChanges();
                MessageBox.Show("Регистрация успешна!");
                this.NavigationService.GoBack();
            }
            catch
            {
                MessageBox.Show("При добавление произошла ошибка!");
            }
        }
    }
}
