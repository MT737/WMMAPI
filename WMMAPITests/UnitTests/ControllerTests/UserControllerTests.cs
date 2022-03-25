using Microsoft.Extensions.Options;
using WMMAPI.Models.UserModels;

namespace WMMAPITests.UnitTests.ControllerTests
{
    [TestClass]
    public class UserControllerTests
    {
        private Mock<ILogger<UserController>> _mockLogger;
        private Mock<IUserService> _mockUserService;
        private IOptions<AppSettings> _appSettings;

        [TestInitialize]
        public void InitializeTest()
        {
            _mockLogger = new Mock<ILogger<UserController>>();
            _mockUserService = new Mock<IUserService>();
            _appSettings = Options.Create(new AppSettings { Secret = "KeepItSecretKeepItSafe" });
        }

        #region Authenticate
        [TestMethod]
        public void SuccessfulAuthenticationReturnsAuthenticatedUserModel()
        {
            // Arrange test
            Guid userId = Guid.NewGuid();
            AuthenticateUserModel model = new AuthenticateUserModel
            {
                EmailAddress = "testemail",
                Password = "testPassword"
            };

            User user = new User
            {
                Id = userId,
                FirstName = "TestName",
                LastName = "TestLastName",
                DOB = DateTime.Now.AddYears(-22),
                EmailAddress = "test@email.com",
                PasswordHash = null,
                PasswordSalt = null,
                IsDeleted = false
            };
            _mockUserService.Setup(m => m.Authenticate(model.EmailAddress, model.Password)).Returns(user);
            
            UserController controller = new UserController(_mockLogger.Object, _mockUserService.Object, _appSettings);
            controller.UserId = userId;

            // Call action
            var result = controller.AuthenticateUser(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var obj = (OkObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(AuthenticatedUserModel));
            var resp = (AuthenticatedUserModel)obj.Value;
            Assert.AreEqual(userId, resp.Id);
        }

        [TestMethod]
        public void AuthenticateExceptionHandled()
        {
            // Arrange test
            Guid userId = Guid.NewGuid();
            AuthenticateUserModel model = new AuthenticateUserModel
            {
                EmailAddress = "testemail",
                Password = "testPassword"
            };

            User user = new User
            {
                Id = userId,
                FirstName = "TestName",
                LastName = "TestLastName",
                DOB = DateTime.Now.AddYears(-22),
                EmailAddress = "test@email.com",
                PasswordHash = null,
                PasswordSalt = null,
                IsDeleted = false
            };
            _mockUserService.Setup(m => m.Authenticate(model.EmailAddress, model.Password)).Throws(new Exception("TestMessage"));

            UserController controller = new UserController(_mockLogger.Object, _mockUserService.Object, _appSettings);
            controller.UserId = userId;

            // Call action
            var result = controller.AuthenticateUser(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));    
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual("TestMessage", resp.Message);
        }

        [TestMethod]
        public void AuthenticateReturnsNullHandled()
        {
            // Arrange test
            Guid userId = Guid.NewGuid();
            AuthenticateUserModel model = new AuthenticateUserModel
            {
                EmailAddress = "testemail",
                Password = "testPassword"
            };

            User user = null;

            _mockUserService.Setup(m => m.Authenticate(model.EmailAddress, model.Password)).Returns(user);

            UserController controller = new UserController(_mockLogger.Object, _mockUserService.Object, _appSettings);
            controller.UserId = Guid.Empty;

            // Call action
            var result = controller.AuthenticateUser(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(InvalidEmailAndPassword, resp.Message);
        }
        #endregion

        #region Register
        [TestMethod]
        public void RegisterUserSucceedsSendsEmptyOK()
        {
            // Arrange test
            Guid userId = Guid.NewGuid();
            RegisterUserModel model = new RegisterUserModel
            {
                FirstName = "TestFName",
                LastName = "TestLName",
                DOB = "05/22/1999",
                EmailAddress = "testemail",
                Password = "testPassword"
            };

            UserController controller = new UserController(_mockLogger.Object, _mockUserService.Object, _appSettings);
            controller.UserId = userId;

            // Call action
            var result = controller.RegisterUser(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void RegisterUserAppExceptionHandled()
        {
            // Arrange test
            Guid userId = Guid.NewGuid();
            RegisterUserModel model = new RegisterUserModel
            {
                FirstName = "TestFName",
                LastName = "TestLName",
                DOB = "05/22/1999",
                EmailAddress = "testemail",
                Password = "testPassword"
            };

            _mockUserService.Setup(m => m.Create(It.IsAny<User>(), It.IsAny<String>())).Throws(new AppException());

            UserController controller = new UserController(_mockLogger.Object, _mockUserService.Object, _appSettings);
            controller.UserId = userId;

            // Call action
            var result = controller.RegisterUser(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual("Exception of type 'WMMAPI.Helpers.AppException' was thrown.", resp.Message);
        }

        [TestMethod]
        public void RegisterUserExceptionHandled()
        {
            // Arrange test
            Guid userId = Guid.NewGuid();
            RegisterUserModel model = new RegisterUserModel
            {
                FirstName = "TestFName",
                LastName = "TestLName",
                DOB = "05/22/1999",
                EmailAddress = "testemail",
                Password = "testPassword"
            };

            _mockUserService.Setup(m => m.Create(It.IsAny<User>(), It.IsAny<String>())).Throws(new Exception());

            UserController controller = new UserController(_mockLogger.Object, _mockUserService.Object, _appSettings);
            controller.UserId = userId;

            // Call action
            var result = controller.RegisterUser(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(GenericErrorMessage, resp.Message);
        }
        #endregion

        #region Get
        #endregion

        #region Modify
        #endregion

        #region Delete
        #endregion

    }
}
