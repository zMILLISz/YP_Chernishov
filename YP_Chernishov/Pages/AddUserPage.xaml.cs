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
    /// Логика взаимодействия для AddUserPage.xaml
    /// </summary>
    public partial class AddUserPage : Page
    {
        private User _currentUser = new User();
        public AddUserPage(User selectedUser)
        {
            InitializeComponent();

            cmbRole.ItemsSource = YP_ChernishovEntities.GetContext().Role.ToList();

            if (selectedUser != null)
            {
                _currentUser = selectedUser;
                cmbRole.Text = _currentUser.Role.RoleName;

                Title = "Редактирование записи на прием";
            }

            DataContext = _currentUser;
        }

        private void cmbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedRole = cmbRole.SelectedItem as Role;
            if (selectedRole != null)
            {
                if (selectedRole.RoleId == 2 || selectedRole.RoleName == "Специалист")
                {
                    TB_UserExperience.Visibility = Visibility.Visible;
                    TB_UserInfo.Visibility = Visibility.Visible;
                    TBox_UserExperience.Visibility = Visibility.Visible;
                    TBox_UserInfo.Visibility = Visibility.Visible;
                }
                else
                {
                    TB_UserExperience.Visibility = Visibility.Collapsed;
                    TB_UserInfo.Visibility = Visibility.Collapsed;
                    TBox_UserExperience.Visibility = Visibility.Collapsed;
                    TBox_UserInfo.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            var selectedRole = cmbRole.SelectedItem as Role;

            if (string.IsNullOrWhiteSpace(_currentUser.UserLogin))
                errors.AppendLine("Укажите логин!");

            if (string.IsNullOrWhiteSpace(_currentUser.UserPassword))
                errors.AppendLine("Укажите пароль!");

            if (string.IsNullOrWhiteSpace(_currentUser.UserPhoto))
                _currentUser.UserPhoto = null;

            if (selectedRole == null)
                errors.AppendLine("Выберите роль!");
            else
            {
                _currentUser.UserRole = selectedRole.RoleId;
            }

            if (string.IsNullOrWhiteSpace(_currentUser.UserFIO))
                errors.AppendLine("Укажите ФИО!");

            if (TBox_UserExperience.Visibility == Visibility.Visible)
            {
                if (string.IsNullOrWhiteSpace(TBox_UserExperience.Text))
                {
                    MessageBox.Show("Пожалуйста, укажите опыт работы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (int.TryParse(TBox_UserExperience.Text, out int experience))
                {
                    if (experience < 0)
                    {
                        MessageBox.Show("Опыт работы должен быть неотрицательным", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _currentUser.UserExperience = experience;
                }
                else
                {
                    MessageBox.Show("Неверный формат опыта работы (необходимо число)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
                _currentUser.UserExperience = null;

            if (TBox_UserInfo.Visibility == Visibility.Visible)
            {
                if (string.IsNullOrWhiteSpace(TBox_UserInfo.Text))
                {
                    MessageBox.Show("Пожалуйста, укажите описание", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
                _currentUser.UserInfo = null;

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            bool isEditingExistingUser = _currentUser.UserId != 0;

            if (!isEditingExistingUser)
            {
                var existingUser = YP_ChernishovEntities.GetContext().User
                    .FirstOrDefault(u => u.UserLogin == _currentUser.UserLogin && u.UserId != _currentUser.UserId);

                if (existingUser != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!");
                    return;
                }
            }

            _currentUser.UserPassword = GetHash(_currentUser.UserPassword);

            if (_currentUser.UserId == 0)
                YP_ChernishovEntities.GetContext().User.Add(_currentUser);

            try
            {
                YP_ChernishovEntities.GetContext().SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
                this.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
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
    }
}
