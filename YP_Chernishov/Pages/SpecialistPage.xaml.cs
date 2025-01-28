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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YP_Chernishov.Pages
{
    /// <summary>
    /// Логика взаимодействия для SpecialistPage.xaml
    /// </summary>
    public partial class SpecialistPage : Page
    {
        private string _userLogin;
        public SpecialistPage(string login)
        {
            InitializeComponent();

            _userLogin = login;

            var currentUsers = YP_ChernishovEntities.GetContext().Request.ToList();
            currentUsers = currentUsers.Where(x => x.User1.UserLogin == _userLogin).OrderByDescending(x => x.RequestData).ToList();
            ListRequests.ItemsSource = currentUsers;
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            TextBoxSearch.Text = string.Empty;
            UpdateUsers();
        }

        private void TextBoxSearch_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateUsers();
        }

        private void UpdateUsers()
        {
            var currentUsers = YP_ChernishovEntities.GetContext().Request.ToList();

            currentUsers = currentUsers.Where(x => x.User1.UserLogin == _userLogin).ToList();

            if (!string.IsNullOrEmpty(TextBoxSearch.Text))
            {
                var matchingUserIds = YP_ChernishovEntities.GetContext().User
                    .Where(x => x.UserFIO.ToLower().Contains(TextBoxSearch.Text.ToLower()))
                    .Select(x => x.UserId)
                    .ToList();

                currentUsers = currentUsers.Where(x => matchingUserIds.Contains((int)x.RequestPatient)).ToList();
            }

            currentUsers = currentUsers.OrderByDescending(x => x.RequestData).ToList();
            ListRequests.ItemsSource = currentUsers;
        }
    }
}
