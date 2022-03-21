using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using WMMAPI.Controllers;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Models.AccountModels;
using WMMAPITests.DataHelpers;
using static WMMAPI.Helpers.Globals.ErrorMessages;

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
            OkObjectResult result = (OkObjectResult)controller.GetAccounts();
            
            // Assert
            Assert.AreEqual((int)result.StatusCode, (int)HttpStatusCode.OK);
            Assert.IsInstanceOfType(result.Value, typeof(IList<AccountModel>));
            IList<AccountModel> accounts = (IList<AccountModel>)result.Value;
            Assert.AreEqual(5, accounts.Count);
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
            BadRequestObjectResult result = (BadRequestObjectResult)controller.GetAccounts();

            // Assert
            Assert.AreEqual((int)result.StatusCode, (int)HttpStatusCode.BadRequest);            
            Assert.IsInstanceOfType(result.Value, typeof(ExceptionResponse));
            ExceptionResponse response = (ExceptionResponse)result.Value;
            Assert.AreEqual(response.Message, "Exception of type 'WMMAPI.Helpers.AppException' was thrown.");
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
            BadRequestObjectResult result = (BadRequestObjectResult)controller.GetAccounts();

            // Assert
            Assert.AreEqual((int)result.StatusCode, (int)HttpStatusCode.BadRequest);
            Assert.IsInstanceOfType(result.Value, typeof(ExceptionResponse));
            ExceptionResponse response = (ExceptionResponse)result.Value;
            Assert.AreEqual(response.Message, GenericErrorMessage);
        }

        [TestMethod]
        public void GetEmptyGuidHandled()
        {
            // Arrange test
            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = Guid.Empty;

            // Call action
            BadRequestObjectResult result = (BadRequestObjectResult)controller.GetAccounts();

            // Assert
            Assert.AreEqual((int)result.StatusCode, (int)HttpStatusCode.BadRequest);
            Assert.IsInstanceOfType(result.Value, typeof(ExceptionResponse));
            ExceptionResponse response = (ExceptionResponse)result.Value;
            Assert.AreEqual(response.Message, AuthenticationError);
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

            // Call action
            OkObjectResult result = (OkObjectResult)controller.AddAccount(model);

            // Assert
            Assert.AreEqual((int)result.StatusCode, (int)HttpStatusCode.OK);
            Assert.IsInstanceOfType(result.Value, typeof(AccountModel));
            AccountModel account = (AccountModel)result.Value;
            Assert.AreEqual(model.Name, account.Name);
        }

        [TestMethod]
        public void AddAppExceptionHandled()
        {
            // Arrange test
            AddAccountModel model = GenerateAddAccountModel();
            _mockAccountService.Setup(m => m.AddAccount(It.IsAny<Account>())).Throws(new AppException());            
            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = Guid.NewGuid();

            // Call action
            BadRequestObjectResult result = (BadRequestObjectResult)controller.AddAccount(model);

            // Assert
            Assert.AreEqual((int)result.StatusCode, (int)HttpStatusCode.BadRequest);
            Assert.IsInstanceOfType(result.Value, typeof(ExceptionResponse));
            ExceptionResponse response = (ExceptionResponse)result.Value;
            Assert.AreEqual(response.Message, "Exception of type 'WMMAPI.Helpers.AppException' was thrown.");
        }

        [TestMethod]
        public void AddExceptionHandled()
        {
            // Arrange test
            AddAccountModel model = GenerateAddAccountModel();
            _mockAccountService.Setup(m => m.AddAccount(It.IsAny<Account>())).Throws(new Exception());
            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = Guid.NewGuid();

            // Call action
            BadRequestObjectResult result = (BadRequestObjectResult)controller.AddAccount(model);

            // Assert
            Assert.AreEqual((int)result.StatusCode, (int)HttpStatusCode.BadRequest);
            Assert.IsInstanceOfType(result.Value, typeof(ExceptionResponse));
            ExceptionResponse response = (ExceptionResponse)result.Value;
            Assert.AreEqual(response.Message, GenericErrorMessage);
        }
        [TestMethod]
        public void AddEmptyGuidExceptionHandled()
        {
            // Arrange test
            AddAccountModel model = GenerateAddAccountModel();
            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = Guid.Empty;

            // Call action
            BadRequestObjectResult result = (BadRequestObjectResult)controller.AddAccount(model);

            // Assert
            Assert.AreEqual((int)result.StatusCode, (int)HttpStatusCode.BadRequest);
            Assert.IsInstanceOfType(result.Value, typeof(ExceptionResponse));
            ExceptionResponse response = (ExceptionResponse)result.Value;
            Assert.AreEqual(response.Message, AuthenticationError);
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
            OkResult result = (OkResult)controller.ModifyAccount(model);

            // Assert
            Assert.AreEqual((int)result.StatusCode, (int)HttpStatusCode.OK);            
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
            BadRequestObjectResult result = (BadRequestObjectResult)controller.ModifyAccount(model);

            // Assert
            Assert.AreEqual((int)result.StatusCode, (int)HttpStatusCode.BadRequest);
            Assert.IsInstanceOfType(result.Value, typeof(ExceptionResponse));
            ExceptionResponse response = (ExceptionResponse)result.Value;
            Assert.AreEqual(response.Message, "Exception of type 'WMMAPI.Helpers.AppException' was thrown.");
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
            BadRequestObjectResult result = (BadRequestObjectResult)controller.ModifyAccount(model);

            // Assert
            Assert.AreEqual((int)result.StatusCode, (int)HttpStatusCode.BadRequest);
            Assert.IsInstanceOfType(result.Value, typeof(ExceptionResponse));
            ExceptionResponse response = (ExceptionResponse)result.Value;
            Assert.AreEqual(response.Message, GenericErrorMessage);
        }

        [TestMethod]
        public void ModifyEmptyGuidHandled()
        {
            // Arrange test
            UpdateAccountModel model = GenerateUpdateAccountModel();
            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = Guid.Empty;

            // Call action
            BadRequestObjectResult result = (BadRequestObjectResult)controller.ModifyAccount(model);

            // Assert
            Assert.AreEqual((int)result.StatusCode, (int)HttpStatusCode.BadRequest);
            Assert.IsInstanceOfType(result.Value, typeof(ExceptionResponse));
            ExceptionResponse response = (ExceptionResponse)result.Value;
            Assert.AreEqual(response.Message, AuthenticationError);
        }

        #endregion

        #region TestHelpers
        private IList<AccountModel> GenerateTestAccountModels(Guid userId)
        {            
            IList<AccountModel> models = new List<AccountModel>();
            for (int i = 0; i < 5; i++)
            {
                models.Add(new AccountModel(_td.CreateTestAccount(userId)));
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
