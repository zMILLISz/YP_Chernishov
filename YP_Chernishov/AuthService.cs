using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YP_Chernishov
{
    public class AuthService
    {
        private const string ConnectionString = @"Data Source=DESKTOP-22U9FOV\SQLEXPRESS;Initial Catalog=YP_Chernishov;Integrated Security=True";
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        public User Login(string login, string password)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var passHash = GetHash(password);
                User user = null;

                using (SqlCommand command = new SqlCommand("SELECT UserId, UserLogin FROM [User] WHERE UserLogin = @login AND UserPassword = @password", connection))
                {
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@password", passHash);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                UserId = (int)reader["UserId"],
                                UserLogin = (string)reader["UserLogin"]
                            };
                        }
                    }
                }

                return user;
            }
        }
    }
}
