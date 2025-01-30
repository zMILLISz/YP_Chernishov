using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YP_Chernishov
{
    public class AdminPageLogic
    {
        private string _adminLogin;

        public AdminPageLogic(string login)
        {
            _adminLogin = login;

        }

        public IEnumerable<User> GetUsers()
        {
            return YP_ChernishovEntities.GetContext().User.ToList();
        }

        public IEnumerable<Request> GetRequests()
        {
            return YP_ChernishovEntities.GetContext().Request.ToList();
        }

        public void RemoveUsers(List<User> usersForRemoving)
        {
            if (usersForRemoving.Count == 0)
            {
                throw new InvalidOperationException("Пожалуйста, выберите пользователей для удаления.");
            }

            YP_ChernishovEntities.GetContext().User.RemoveRange(usersForRemoving);
            YP_ChernishovEntities.GetContext().SaveChanges();
        }

        public void RemoveRequests(List<Request> requestsForRemoving)
        {
            if (requestsForRemoving.Count == 0)
            {
                throw new InvalidOperationException("Пожалуйста, выберите записи для удаления.");
            }

            YP_ChernishovEntities.GetContext().Request.RemoveRange(requestsForRemoving);
            YP_ChernishovEntities.GetContext().SaveChanges();
        }
    }
}
