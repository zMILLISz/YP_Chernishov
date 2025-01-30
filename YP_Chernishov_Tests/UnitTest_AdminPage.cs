using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using YP_Chernishov;

namespace YP_Chernishov_Tests
{
    [TestClass]
    public class AdminPageLogicTests
    {
        private AdminPageLogic _logic;

        [TestInitialize]
        public void Setup()
        {
            _logic = new AdminPageLogic("admin_login");
        }

        [TestMethod]
        public void GetUsers_ReturnsUsers_NotNull()
        {
            var users = _logic.GetUsers();
            Assert.IsNotNull(users);
        }

        [TestMethod]
        public void RemoveUsers_ValidInput_Success()
        {
            var userList = new List<User> { new User {  } };

            _logic.RemoveUsers(userList);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveUsers_EmptyList_ThrowsException()
        {
            _logic.RemoveUsers(new List<User>());
        }

        [TestMethod]
        public void RemoveRequests_ValidInput_Success()
        {
            var requestList = new List<Request> { new Request { } };

            _logic.RemoveRequests(requestList);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveRequests_EmptyList_ThrowsException()
        {
            _logic.RemoveRequests(new List<Request>());
        }
    }
}
