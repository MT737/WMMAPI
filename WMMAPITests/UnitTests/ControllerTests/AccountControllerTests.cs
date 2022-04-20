using Microsoft.AspNetCore.Http;
using WMMAPI.Services.AccountServices.AccountModels;

namespace WMMAPITests.UnitTests.ControllerTests
{
    [TestClass]
    public class AccountControllerTests
    {
        private TestData _td;
        private Mock<ILogger<AccountController>> _mockLogger;
        private Mock<IAccountService> _mockAccountService;

        [TestInitialize]
        public void Initialize()
        {
            _td = new TestData();
            _mockLogger = new Mock<ILogger<AccountController>>();
            _mockAccountService = new Mock<IAccountService>();
        }

        #region Get
        [TestMethod]
        public void GetShouldReturnViewModel()
        {            
            // Arrange test
            var userId = Guid.NewGuid();
            var testAccounts = GenerateTestAccountModels(userId);
            _mockAccountService.Setup(m => m.GetList(userId)).Returns(testAccounts);
            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.GetAccounts();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var obj = (OkObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(IList<AccountModel>));
            IList<AccountModel> accounts = (IList<AccountModel>)obj.Value;
            Assert.AreEqual(accounts.Count, 5);
        }

        [TestMethod]
        public void GetAppExceptionHandled()
        {
            // Arrange test
            Guid userId = Guid.NewGuid();
            _mockAccountService.Setup(m => m.GetList(userId)).Throws(new AppException());
            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.GetAccounts();

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual("Exception of type 'WMMAPI.Helpers.AppException' was thrown.", resp.Message);
        }

        [TestMethod]
        public void GetExceptionHandled()
        {
            // Arrange test
            Guid userId = Guid.NewGuid();
            _mockAccountService.Setup(m => m.GetList(userId)).Throws(new Exception());
            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.GetAccounts();

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(GenericErrorMessage, resp.Message);
        }

        [TestMethod]
        public void GetEmptyUserGuidHandled()
        {
            // Arrange test
            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = Guid.Empty;

            // Call action
            var result = controller.GetAccounts();

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(AuthenticationError, resp.Message);
        }
        #endregion

        #region Add
        [TestMethod]
        public void AddShouldReturnViewModel()
        {
            // Arrange test
            AddAccountModel model = GenerateAddAccountModel();
            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = Guid.NewGuid();
            _mockAccountService.Setup(m => m.AddAccount(It.IsAny<Account>(), It.IsAny<Decimal>()))
                .Returns(new AccountModel(
                    new Account {
                        Id = Guid.NewGuid(),
                        Name = model.Name,
                        IsAsset = model.IsAsset,
                        IsActive = model.IsActive
                    }, 
                    model.Balance)
                );

            // Call action
            var result = controller.AddAccount(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var obj = (ObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(AccountModel));
            var account = (AccountModel)obj.Value;
            Assert.AreEqual(model.Name, account.Name);
        }

        [TestMethod]
        public void AddAppExceptionHandled()
        {
            // Arrange test
            AddAccountModel model = GenerateAddAccountModel();
            _mockAccountService.Setup(m => m.AddAccount(It.IsAny<Account>(), It.IsAny<Decimal>())).Throws(new AppException());            
            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = Guid.NewGuid();

            // Call action
            var result = controller.AddAccount(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual("Exception of type 'WMMAPI.Helpers.AppException' was thrown.", resp.Message);
        }

        [TestMethod]
        public void AddExceptionHandled()
        {
            // Arrange test
            AddAccountModel model = GenerateAddAccountModel();
            _mockAccountService.Setup(m => m.AddAccount(It.IsAny<Account>(), It.IsAny<Decimal>())).Throws(new Exception());
            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = Guid.NewGuid();

            // Call action
            var result = controller.AddAccount(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(GenericErrorMessage, resp.Message);
        }
        [TestMethod]
        public void AddEmptyGuidExceptionHandled()
        {
            // Arrange test
            AddAccountModel model = GenerateAddAccountModel();
            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = Guid.Empty;

            // Call action
            var result = controller.AddAccount(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(AuthenticationError, resp.Message);
        }
        #endregion

        #region Modify
        [TestMethod]
        public void ModifyShouldReturnEmptyOk()
        {
            // Arrange test
            UpdateAccountModel model = GenerateUpdateAccountModel();
            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = Guid.NewGuid();

            // Call action
            var result = controller.ModifyAccount(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            var obj = (StatusCodeResult)result;
            Assert.AreEqual(StatusCodes.Status204NoContent, obj.StatusCode);
        }

        [TestMethod]
        public void ModifyAppExceptionHandled()
        {
            // Arrange test
            UpdateAccountModel model = GenerateUpdateAccountModel();
            _mockAccountService.Setup(m => m.ModifyAccount(It.IsAny<Account>())).Throws(new AppException());
            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = Guid.NewGuid();

            // Call action
            var result = controller.ModifyAccount(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual("Exception of type 'WMMAPI.Helpers.AppException' was thrown.", resp.Message);
        }

        [TestMethod]
        public void ModifyExceptionHandled()
        {
            // Arrange test
            UpdateAccountModel model = GenerateUpdateAccountModel();
            _mockAccountService.Setup(m => m.ModifyAccount(It.IsAny<Account>())).Throws(new Exception());
            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = Guid.NewGuid();

            // Call action
            var result = controller.ModifyAccount(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(GenericErrorMessage, resp.Message);
        }

        [TestMethod]
        public void ModifyEmptyGuidHandled()
        {
            // Arrange test
            UpdateAccountModel model = GenerateUpdateAccountModel();
            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = Guid.Empty;

            // Call action
            var result = controller.ModifyAccount(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(AuthenticationError, resp.Message);
        }

        #endregion

        #region TestHelpers
        private IList<AccountModel> GenerateTestAccountModels(Guid userId)
        {            
            IList<AccountModel> models = new List<AccountModel>();
            for (int i = 0; i < 5; i++)
            {
                models.Add(new AccountModel(_td.CreateTestAccount(userId), 500.00M));
            }
            return models;
        }

        private AddAccountModel GenerateAddAccountModel()
        {
            return new AddAccountModel
            {
                Name = "TestAccount",
                IsAsset = true,
                IsActive = true,
                Balance = 250
            };
        }

        private UpdateAccountModel GenerateUpdateAccountModel()
        {
            return new UpdateAccountModel
            {
                AccountId = Guid.NewGuid(),
                Name = "TestAccount",
                IsActive = true
            };
        }
        #endregion
    }
}
