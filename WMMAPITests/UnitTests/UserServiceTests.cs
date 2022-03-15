using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Services;
using WMMAPITests.DataHelpers;

namespace WMMAPITests.UnitTests
{
    [TestClass]
    public class UserServiceTests
    {
        private TestData _testData;
        private TestDataContext _tdc;


        [TestInitialize]
        public void Init()
        {
            _testData = new TestData();
            _tdc = new TestDataContext(_testData);
        }

        #region Authenticate
        [TestMethod]
        public void TestAuthenticateSucceeds()
        {
            // Fabricate test
            string testPassword = "testPassword";
            User user = _testData.CreateTestUser(testPassword, "Firsty", "Secondy", @"test@email.com");
            _testData.Users = _testData.Users.Concat(new List<User> { user });
            _tdc = new TestDataContext(_testData);

            // Initiate service and call method
            IUserService service = new UserService(_tdc.WMMContext.Object);
            User result = service.Authenticate(user.EmailAddress, testPassword);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Users, Times.Once());
            Assert.AreEqual(user.Id, result.Id);
            Assert.AreEqual(user.EmailAddress, result.EmailAddress);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void TestAuthenticateEmailNullOrEmptyReturnsNull(string email)
        {
            // Fabricate test
            string testPassword = "testPassword";
            User user = _testData.CreateTestUser(testPassword, "Firsty", "Secondy", @"test@email.com");
            _testData.Users = _testData.Users.Concat(new List<User> { user });
            _tdc = new TestDataContext(_testData);

            // Initiate service and call method
            IUserService service = new UserService(_tdc.WMMContext.Object);
            User result = service.Authenticate(email, testPassword);

            // Confirm mock (not needed) and assert
            Assert.IsNull(result);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void TestAuthenticatePasswordNullOrEmptyReturnsNull(string password)
        {
            // Fabricate test
            string testPassword = "testPassword";
            User user = _testData.CreateTestUser(testPassword, "Firsty", "Secondy", @"test@email.com");
            _testData.Users = _testData.Users.Concat(new List<User> { user });
            _tdc = new TestDataContext(_testData);

            // Initiate service and call method
            IUserService service = new UserService(_tdc.WMMContext.Object);
            User result = service.Authenticate(user.EmailAddress, password);

            // Confirm mock (not needed) and assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestAuthenticateUserDoesntExistReturnsNull()
        {
            // Fabricate test
            string testPassword = "testPassword";
            User user = _testData.CreateTestUser(testPassword, "Firsty", "Secondy", @"test@email.com");
            _testData.Users = _testData.Users.Concat(new List<User> { user });

            // Initiate service and call method
            IUserService service = new UserService(_tdc.WMMContext.Object);
            User result = service.Authenticate(user.EmailAddress, testPassword);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Users, Times.Once());
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestAuthenticatePasswordIncorrectReturnsNull()
        {
            // Fabricate test
            string testPassword = "testPassword";
            User user = _testData.CreateTestUser(testPassword, "Firsty", "Secondy", @"test@email.com");
            _testData.Users = _testData.Users.Concat(new List<User> { user });
            _tdc = new TestDataContext(_testData);

            // Initiate service and call method
            IUserService service = new UserService(_tdc.WMMContext.Object);
            User result = service.Authenticate(user.EmailAddress, "wrongpassword");

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Users, Times.Once());
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestAuthenticateUserIsDeletedReturnsNull()
        {
            // Fabricate test
            string testPassword = "testPassword";
            User user = _testData.CreateTestUser(testPassword, "Firsty", "Secondy", @"test@email.com", true);
            _testData.Users = _testData.Users.Concat(new List<User> { user });
            _tdc = new TestDataContext(_testData);

            // Initiate service and call method
            IUserService service = new UserService(_tdc.WMMContext.Object);
            User result = service.Authenticate(user.EmailAddress, testPassword);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Users, Times.Once());
            Assert.IsNull(result);
        }
        #endregion

        #region Create
        [TestMethod]
        public void TestCreateSucceeds()
        {
            // Fabricate test
            string testPassword = "testPassword";
            User user = _testData.CreateTestUser(testPassword, "Firsty", "Secondy", @"test@email.com");
            
            // Initiate service and call method
            IUserService service = new UserService(_tdc.WMMContext.Object);
            User result = service.Create(user, testPassword);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Users, Times.Once());
            _tdc.UserSet.Verify(m => m.Add(It.IsAny<User>()), Times.Once());
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.AreEqual(user.Id, result.Id);
            Assert.AreEqual(user.EmailAddress, result.EmailAddress);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [ExpectedException(typeof(AppException))]
        public void TestCreatePasswordIsNullOrWhiteSpace(string password)
        {
            // Fabricate test
            User user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "TestL",
                DOB = DateTime.Now.AddYears(-20),
                EmailAddress = "test.test@email",
                IsDeleted = false
            };

            // Initiate service and call method
            IUserService service = new UserService(_tdc.WMMContext.Object);
            User result = service.Create(user, password);

            // Confirm mock and assert (No need, exception expected)
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [ExpectedException(typeof(AppException))]
        public void TestCreateEmailIsNullOrWhiteSpace(string email)
        {
            // Fabricate test
            User user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "TestL",
                DOB = DateTime.Now.AddYears(-20),
                EmailAddress = email,
                IsDeleted = false
            };

            // Initiate service and call method
            IUserService service = new UserService(_tdc.WMMContext.Object);
            User result = service.Create(user, "testPassword");

            // Confirm mock and assert (No need, exception expected)
        }

        [TestMethod]        
        [ExpectedException(typeof(AppException))]
        public void TestCreateEmailAlreadyExists()
        {
            // Fabricate test
            string existingEmail = _testData.Users.First().EmailAddress;
            User user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "TestL",
                DOB = DateTime.Now.AddYears(-20),
                EmailAddress = existingEmail,
                IsDeleted = false
            };

            // Initiate service and call method
            IUserService service = new UserService(_tdc.WMMContext.Object);
            User result = service.Create(user, "testPassword");

            // Confirm mock and assert (No need, exception expected)
        }
        #endregion

        // Modify
        // succeeds
        // user not found
        // email used by other user

        // GetById
        // Succeeds
        // Returns null as user doesn't exist

        // Remove User
        // Succeeds
        // user not found
    }
}
