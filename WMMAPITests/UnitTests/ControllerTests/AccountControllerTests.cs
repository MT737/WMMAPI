using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using WMMAPI.Controllers;
using WMMAPI.Interfaces;
using WMMAPI.Models.AccountModels;
using WMMAPITests.DataHelpers;

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

        [TestMethod]
        public void GetShouldReturnViewModel()
        {            
            var userId = Guid.NewGuid();
            var testAccounts = GenerateTestAccountModels(userId);
            _mockAccountService.Setup(m => m.GetList(userId)).Returns(testAccounts);

            AccountController controller = new(_mockLogger.Object, _mockAccountService.Object);
            controller.UserId = userId;


            OkObjectResult result = (OkObjectResult)controller.GetAccounts();
            

            Assert.IsNotNull((int)result.StatusCode == (int)HttpStatusCode.OK);
            Assert.IsInstanceOfType(result.Value, typeof(IList<AccountModel>));
            IList<AccountModel> accounts = (IList<AccountModel>)result.Value;
            Assert.AreEqual(5, accounts.Count);
        }

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
        #endregion
    }
}
