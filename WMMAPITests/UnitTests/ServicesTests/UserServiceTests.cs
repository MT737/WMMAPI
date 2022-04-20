using WMMAPI.Services.UserService;

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
            Assert.IsTrue(result.Categories.Any());
            Assert.IsTrue(result.Vendors.Any());
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

        #region Modify
        [TestMethod]
        public void TestModifySucceeds()
        {
            // Fabricate test
            User testUser = _testData.Users.First();
            testUser.FirstName = "ModifiedFirstName";
            testUser.LastName = "ModifiedLastName";
            testUser.DOB = DateTime.Now.AddYears(-60);
            string password = "newPassword";

            // Initiate service and call method
            IUserService service = new UserService(_tdc.WMMContext.Object);
            service.Modify(testUser, password);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Users, Times.Exactly(2));
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());

            User contextUser = _tdc.WMMContext.Object.Users.First(u => u.Id == testUser.Id);
            Assert.AreEqual(testUser.FirstName, contextUser.FirstName);
            Assert.AreEqual(testUser.LastName, contextUser.LastName);
            Assert.AreEqual(testUser.DOB, contextUser.DOB);
            // TODO Is there a way to easily confirm the password is updated?
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyUserNotFound()
        {
            // Fabricate Test
            User testUser = _testData.CreateTestUser();

            // Initiate service and call method
            IUserService service = new UserService(_tdc.WMMContext.Object);
            service.Modify(testUser, null);

            // Confirm mock and assert (no need, exception expected)
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyEmailAlreadyInUse()
        {
            // Fabricate Test
            User testUser = _testData.Users.First();
            string usedEmail = _testData.Users.Last().EmailAddress;
            testUser.EmailAddress = usedEmail;

            // Initiate service and call method
            IUserService service = new UserService(_tdc.WMMContext.Object);
            service.Modify(testUser, null);

            // Confirm mock and assert (no need, exception expected)
        }
        #endregion

        #region GetById
        [TestMethod]
        public void TestGetByIdSucceeds()
        {
            // Fabricate test
            User user = _testData.Users.First();

            // Initialize service and call method
            IUserService service = new UserService(_tdc.WMMContext.Object);
            User result = service.GetById(user.Id);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Users, Times.Once());
            Assert.AreEqual(user.Id, result.Id);
            Assert.AreEqual(user.FirstName, result.FirstName);
        }

        [TestMethod]
        public void TestGetByIdReturnsNullAsUserDoesNotExist()
        {
            // Fabricate test
            Guid userId = Guid.NewGuid();

            // Initialize service and call method
            IUserService service = new UserService(_tdc.WMMContext.Object);
            User result = service.GetById(userId);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Users, Times.Once());
            Assert.IsNull(result);
        }
        #endregion

        #region Remove
        [TestMethod]
        public void TestRemoveUserSucceeds()
        {
            // Fabricate test
            Guid userId = _testData.Users.First().Id;

            // Initialize service and call method
            IUserService service = new UserService(_tdc.WMMContext.Object);
            service.RemoveUser(userId);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Users, Times.Once());
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsTrue(_tdc.WMMContext.Object.Users.First(u => u.Id == userId).IsDeleted);
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestRemoveUserUserNotFound()
        {
            // Fabricate Test
            Guid userId = Guid.NewGuid();

            // Initialize service and call method
            IUserService service = new UserService(_tdc.WMMContext.Object);
            service.RemoveUser(userId);

            // Confirm mock and assert (not needed, exception expected)
        }
        #endregion
    }
}
