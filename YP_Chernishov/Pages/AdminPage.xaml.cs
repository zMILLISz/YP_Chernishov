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
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        private string _adminLogin;
        public AdminPage(string login)
        {
            InitializeComponent();
            _adminLogin = login;
        }

        private void ButtonUsers_Click(object sender, RoutedEventArgs e)
        {
            DataGridUser.Visibility = Visibility.Visible;
            DataGridRequest.Visibility = Visibility.Collapsed;
            DataGridUser.ItemsSource = YP_ChernishovEntities.GetContext().User.ToList();
        }

        private void ButtonRequests_Click(object sender, RoutedEventArgs e)
        {
            DataGridUser.Visibility = Visibility.Collapsed;
            DataGridRequest.Visibility = Visibility.Visible;
            DataGridRequest.ItemsSource = YP_ChernishovEntities.GetContext().Request.ToList();
        }

        private void DataGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                if (DataGridUser.Visibility == Visibility.Visible)
                {
                    YP_ChernishovEntities.GetContext().ChangeTracker.Entries().ToList().
                        ForEach(x => x.Reload());
                    DataGridUser.ItemsSource = YP_ChernishovEntities.GetContext().User.ToList();
                }
                else if (DataGridRequest.Visibility == Visibility.Visible)
                {
                    YP_ChernishovEntities.GetContext().ChangeTracker.Entries().ToList().
                       ForEach(x => x.Reload());
                    DataGridRequest.ItemsSource = YP_ChernishovEntities.GetContext().Request.ToList();
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridUser.Visibility == Visibility.Visible)
            {
                NavigationService.Navigate(new Pages.AddUserPage(null));
            }
            else if (DataGridRequest.Visibility == Visibility.Visible)
            {
                NavigationService.Navigate(new Pages.AddRequestPage(null, _adminLogin));
            }
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridUser.Visibility == Visibility.Visible)
            {
                var usersForRemoving = DataGridUser.SelectedItems.Cast<User>().ToList();

                if (usersForRemoving.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите пользователей для удаления.");
                    return;
                }

                if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {usersForRemoving.Count} элементов?", "Внимание",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        YP_ChernishovEntities.GetContext().User.RemoveRange(usersForRemoving);
                        YP_ChernishovEntities.GetContext().SaveChanges();
                        MessageBox.Show("Данные успешно удалены!");
                        DataGridUser.ItemsSource = YP_ChernishovEntities.GetContext().User.ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
            else if (DataGridRequest.Visibility == Visibility.Visible)
            {
                var requestsForRemoving = DataGridRequest.SelectedItems.Cast<Request>().ToList();

                if (requestsForRemoving.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите записи для удаления.");
                    return;
                }

                if (MessageBox.Show($"Вы точно хотите удалить записи в количестве {requestsForRemoving.Count} элементов?", "Внимание",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        YP_ChernishovEntities.GetContext().Request.RemoveRange(requestsForRemoving);
                        YP_ChernishovEntities.GetContext().SaveChanges();
                        MessageBox.Show("Данные успешно удалены!");
                        DataGridRequest.ItemsSource = YP_ChernishovEntities.GetContext().Request.ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridUser.Visibility == Visibility.Visible)
            {
                NavigationService.Navigate(new Pages.AddUserPage((sender as Button).DataContext as User));
            }
            else if (DataGridRequest.Visibility == Visibility.Visible)
            {
                var selectedRequest = (sender as Button).DataContext as Request;
                if (selectedRequest != null)
                {
                    string userLogin = selectedRequest.User.UserLogin;
                    NavigationService.Navigate(new Pages.AddRequestPage(selectedRequest, userLogin));
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите заявку для редактирования.");
                }
            }
        }
    }
}
