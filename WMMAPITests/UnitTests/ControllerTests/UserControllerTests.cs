using Microsoft.Extensions.Options;
using WMMAPI.Models.UserModels;

namespace WMMAPITests.UnitTests.ControllerTests
{
    [TestClass]
    public class UserControllerTests
    {
        private Mock<ILogger<UserController>> _mockLogger;
        private Mock<IUserService> _mockUserService;
        private Mock<IOptions<AppSettings>> _mockAppSettings;

        [TestInitialize]
        public void InitializeTest()
        {
            _mockLogger = new Mock<ILogger<UserController>>();
            _mockUserService = new Mock<IUserService>();
            _mockAppSettings = new Mock<IOptions<AppSettings>>();
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
            
            UserController controller = new UserController(_mockLogger.Object, _mockUserService.Object, _mockAppSettings.Object);
            controller.UserId = userId;
            controller.Secret = "Keepitsecretkeepitsafe";

            // Call action
            var result = controller.AuthenticateUser(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var obj = (OkObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(AuthenticatedUserModel));
            var resp = (AuthenticatedUserModel)obj.Value;
            Assert.AreEqual(userId, resp.Id);
        }
        #endregion

        #region Register
        #endregion

        #region Get
        #endregion

        #region Modify
        #endregion

        #region Delete
        #endregion

    }
}
