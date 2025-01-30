using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using YP_Chernishov;


namespace YP_Chernishov_Tests
{
[TestClass]
    public class RequestServiceTests
    {
        private Mock<YP_ChernishovEntities> _mockContext;
        private Mock<DbSet<Request>> _mockRequestSet;
        private List<Request> _database;

        [TestInitialize]
        public void TestInitialize()
        {
            _database = new List<Request>
            {
                new Request { RequestId = 1, RequestPatient = 1, RequestSpecialist = 2, RequestData = DateTime.Today },
                new Request { RequestId = 2, RequestPatient = 3, RequestSpecialist = 2, RequestData = DateTime.Today.AddDays(1) }
            };

            _mockRequestSet = new Mock<DbSet<Request>>();
            _mockRequestSet.As<IQueryable<Request>>().Setup(m => m.Provider).Returns(_database.AsQueryable().Provider);
            _mockRequestSet.As<IQueryable<Request>>().Setup(m => m.Expression).Returns(_database.AsQueryable().Expression);
            _mockRequestSet.As<IQueryable<Request>>().Setup(m => m.ElementType).Returns(_database.AsQueryable().ElementType);
            _mockRequestSet.As<IQueryable<Request>>().Setup(m => m.GetEnumerator()).Returns(_database.AsQueryable().GetEnumerator());

            _mockContext = new Mock<YP_ChernishovEntities>();
            _mockContext.Setup(c => c.Request).Returns(_mockRequestSet.Object);
        }

        [TestMethod]
        public void TestSetPatient_SetsPatientId()
        {
            var service = new RequestService(new Request());
            service.SetPatient(1);
            Assert.AreEqual(1, service.CurrentRequest.RequestPatient);
        }

        [TestMethod]
        public void TestSetSpecialist_SetsSpecialistId()
        {
            var service = new RequestService(new Request());
            service.SetSpecialist(2);
            Assert.AreEqual(2, service.CurrentRequest.RequestSpecialist);
        }

        [TestMethod]
        public void TestIsDoctorAvailable_ReturnsTrue_WhenAvailable()
        {
            var service = new RequestService(new Request { RequestPatient = 1 });
            bool isAvailable = service.IsDoctorAvailable(2, DateTime.Today);
            Assert.IsTrue(isAvailable);
        }

        [TestMethod]
        public void TestIsDoctorAvailable_ReturnsFalse_WhenNotAvailable()
        {
            var service = new RequestService(new Request { RequestPatient = 3 });
            bool isAvailable = service.IsDoctorAvailable(2, DateTime.Today.AddDays(1));
            Assert.IsFalse(isAvailable);
        }

        [TestMethod]
        public void TestSaveRequest_AddsRequestToDatabase()
        {
            var service = new RequestService(new Request { RequestPatient = 1, RequestSpecialist = 2, RequestData = DateTime.Today });
            _mockContext.Setup(c => c.SaveChanges()).Verifiable();

            service.SaveRequest();

            _mockRequestSet.Verify(m => m.Add(It.IsAny<Request>()), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void TestSetRequestData_SetsCorrectDateTime()
        {
            var service = new RequestService(new Request());
            service.SetRequestData(DateTime.Today, TimeSpan.FromHours(10));

            Assert.AreEqual(DateTime.Today.AddHours(10), service.CurrentRequest.RequestData);
        }
    }
}
