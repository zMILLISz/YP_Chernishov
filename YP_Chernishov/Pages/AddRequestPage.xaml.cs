using System;
using System.Collections.Generic;
using System.Data;
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
using System.Data.Entity;

namespace YP_Chernishov.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddRequestPage.xaml
    /// </summary>
    public partial class AddRequestPage : Page
    {
        private Request _currentRequests = new Request();
        private int _userId;
        public AddRequestPage(Request selectedRequests, string login)
        {
            InitializeComponent();
            
            LoadData();

            var user = YP_ChernishovEntities.GetContext().User.FirstOrDefault(u => u.UserLogin == login);
            if (user != null)
            {
                _userId = user.UserId;

                if (user.UserRole == 3)
                {
                    cmbPatient.Visibility = Visibility.Hidden;
                    PatientBlock.Visibility = Visibility.Hidden;

                    _currentRequests.RequestPatient = _userId;
                }
            }
            else
            {
                MessageBox.Show("Пользователь не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (selectedRequests != null)
            {
                _currentRequests = selectedRequests;

               cmbPatient.SelectedValue = _currentRequests.RequestPatient;
               cmbSpecialist.SelectedValue = _currentRequests.RequestSpecialist;
               
               Title = "Редактирование записи на прием";
            }
            else
            {
                _currentRequests.RequestPatient = _userId;
            }

            DataContext = _currentRequests;
        }

        private void LoadData()
        {
            var currentUser = YP_ChernishovEntities.GetContext().User.ToList();
            cmbPatient.ItemsSource = currentUser.Where(x => x.UserRole == 3).ToList();
            cmbSpecialist.ItemsSource = currentUser.Where(x => x.UserRole == 2).ToList();
        }

        private void calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? selectedDate = CalendarRequest.SelectedDate;
            var selectedSpecialist = cmbSpecialist.SelectedItem as User;

            if (selectedDate.HasValue)
            {
                if (selectedDate.Value.Date < DateTime.Today)
                {
                    MessageBox.Show("Вы не можете выбрать прошедшую дату.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    CalendarRequest.SelectedDate = null;
                }
                else
                {
                    _currentRequests.RequestData = selectedDate.Value;

                    if (selectedSpecialist != null)
                    {
                        var availableTimes = GetAvailableTimes(selectedDate.Value, selectedSpecialist.UserId);
                        cmbTime.ItemsSource = availableTimes;
                        cmbTime.IsDropDownOpen = true;
                    }
                }
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var selectedPatient = cmbPatient.SelectedItem as User;
            var selectedSpecialist = cmbSpecialist.SelectedItem as User;

            if (cmbPatient.Visibility == Visibility.Visible && selectedPatient != null)
            {
                _currentRequests.RequestPatient = selectedPatient.UserId;
            }
            else if (cmbPatient.Visibility == Visibility.Hidden)
            {
                _currentRequests.RequestPatient = _currentRequests.RequestPatient;
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пациента", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedSpecialist != null)
            {
                _currentRequests.RequestSpecialist = selectedSpecialist.UserId;
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите специалиста", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbTime.SelectedItem != null)
            {
                var selectedTime = cmbTime.SelectedItem.ToString();
                TimeSpan timeSpan;
                if (TimeSpan.TryParse(selectedTime, out timeSpan))
                {
                    _currentRequests.RequestData = _currentRequests.RequestData.Date + timeSpan;
                }
            }

            var existingRequest = YP_ChernishovEntities.GetContext().Request
                .Any(r => r.RequestPatient == _currentRequests.RequestPatient &&
                r.RequestSpecialist == selectedSpecialist.UserId &&
                DbFunctions.TruncateTime(r.RequestData) == _currentRequests.RequestData.Date);

            if (existingRequest)
            {
                MessageBox.Show("Вы уже записаны к данному специалисту на этот день.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_currentRequests.RequestId == 0)
                YP_ChernishovEntities.GetContext().Request.Add(_currentRequests);

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

        private List<string> GetAvailableTimes(DateTime selectedDate, int specialistId)
        {
            var allRequests = YP_ChernishovEntities.GetContext().Request
                .Where(r => r.RequestSpecialist == specialistId &&
                            DbFunctions.TruncateTime(r.RequestData) == selectedDate.Date) 
                .ToList();

            var allTimes = new List<string> { "09:00", "09:30", "10:00", "10:30", "11:00", "11:30", "12:00", "12:30", "13:00", "13:30", "14:00", "14:30" };

            foreach (var request in allRequests)
            {
                var timeSlot = request.RequestData.TimeOfDay.ToString(@"hh\:mm");
                allTimes.Remove(timeSlot);
            }

            return allTimes;
        }

        private void cmbSpecialist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDate = CalendarRequest.SelectedDate;
            var selectedSpecialist = cmbSpecialist.SelectedItem as User;

            if (selectedDate.HasValue && selectedSpecialist != null)
            {
                var availableTimes = GetAvailableTimes(selectedDate.Value, selectedSpecialist.UserId);
                cmbTime.ItemsSource = availableTimes;
                cmbTime.IsDropDownOpen = true;
            }
        }
    }
}
