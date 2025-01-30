using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using YP_Chernishov;

namespace YP_Chernishov_Tests
{
    [TestClass]
    public class UnitTest_Авторизация
    {
        [TestInitialize]
        public void SetUp()
        {
            _authService = new AuthService();
        }

        private AuthService _authService;
        [TestMethod]
        public void GetHash_ValidPassword_HashIsCorrect()
        {
            string password = "test123";
            string expectedHash = "7288EDD0FC3FFCBE93A0CF06E3568E28521687BC";

            var hash = AuthService.GetHash(password);

            Assert.AreEqual(expectedHash, hash);
        }

        [TestMethod]
        public void Login_ValidCredentials_ReturnsUser()
        {
            string login = "test";
            string password = "test123";

            var user = _authService.Login(login, password);

            Assert.IsNotNull(user);
            Assert.AreEqual(login, user.UserLogin);
        }

        [TestMethod]
        public void Login_InvalidCredentials_ReturnsNull()
        {
            string login = "invaliduser";
            string password = "wrongpassword";

            var user = _authService.Login(login, password);

            Assert.IsNull(user);
        }
    }
}
