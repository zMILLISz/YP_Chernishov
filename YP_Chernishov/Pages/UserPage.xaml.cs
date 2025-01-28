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
    /// Логика взаимодействия для UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        private string _userLogin;
        public UserPage(string login)
        {
            InitializeComponent();
            _userLogin = login;

            var currentUsers = YP_ChernishovEntities.GetContext().User
                                 .Where(user => user.UserRole == 2)
                                 .ToList();
            ListUser.ItemsSource = currentUsers;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RequestPage(_userLogin));
        }
    }
}
