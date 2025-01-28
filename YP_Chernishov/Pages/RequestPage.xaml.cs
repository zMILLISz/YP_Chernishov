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
    /// Логика взаимодействия для RequestPage.xaml
    /// </summary>
    public partial class RequestPage : Page
    {
        private string _userLogin;
        public RequestPage(string login)
        {
            InitializeComponent();

            _userLogin = login;

            var currentUser = YP_ChernishovEntities.GetContext().Request.ToList();
            currentUser = currentUser.Where(x => x.User.UserLogin == _userLogin).OrderByDescending(x => x.RequestData).ToList();
            ListRequests.ItemsSource = currentUser;

        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.AddRequestPage(null, _userLogin));
        }

        private void ListRequests_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                YP_ChernishovEntities.GetContext().ChangeTracker.Entries().ToList().
                ForEach(x => x.Reload());

                var currentUser = YP_ChernishovEntities.GetContext().Request.ToList();
                currentUser = currentUser.Where(x => x.User.UserLogin == _userLogin).OrderByDescending(x => x.RequestData).ToList();
                ListRequests.ItemsSource = currentUser;
            }
        }
    }
}
