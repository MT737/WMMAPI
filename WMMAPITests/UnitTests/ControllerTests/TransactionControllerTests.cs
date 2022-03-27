using WMMAPI.Models.TransactionModels;

namespace WMMAPITests.UnitTests.ControllerTests
{
    [TestClass]
    public class TransactionControllerTests
    {
        private TestData _td;
        private Mock<ILogger<TransactionController>> _mockLogger;
        private Mock<ITransactionService> _mockTransactionService;

        [TestInitialize]
        public void Initialize()
        {
            _td = new TestData();
            _mockLogger = new Mock<ILogger<TransactionController>>();
            _mockTransactionService = new Mock<ITransactionService>();
        }

        #region GetList
        [TestMethod]
        public void GetShouldReturnViewModel()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            var testTransactions = GenerateTestTransactions(userId);
            _mockTransactionService.Setup(m => m.GetList(userId, true)).Returns(testTransactions);
            TransactionController controller = new(_mockLogger.Object, _mockTransactionService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.GetTransactions();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var obj = (OkObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(IList<TransactionModel>));
            var resp = (IList<TransactionModel>)obj.Value;
            Assert.IsTrue(resp.Count == 5);
        }

        [TestMethod]
        public void GetAppExceptionHandled()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            var testTransactions = GenerateTestTransactions(userId);
            _mockTransactionService.Setup(m => m.GetList(userId, true)).Throws(new AppException());
            TransactionController controller = new(_mockLogger.Object, _mockTransactionService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.GetTransactions();

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
            var userId = Guid.NewGuid();
            var testTransactions = GenerateTestTransactions(userId);
            _mockTransactionService.Setup(m => m.GetList(userId, true)).Throws(new Exception());
            TransactionController controller = new(_mockLogger.Object, _mockTransactionService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.GetTransactions();

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
            TransactionController controller = new(_mockLogger.Object, _mockTransactionService.Object);
            controller.UserId = Guid.Empty;

            // Call action
            var result = controller.GetTransactions();

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
            var userId = Guid.NewGuid();
            var testTransaction = GenerateTestTransactions(userId).First();
            AddTransactionModel model = new AddTransactionModel()
            {
                TransactionDate = testTransaction.TransactionDate,
                AccountId = testTransaction.AccountId,
                CategoryId = testTransaction.CategoryId,
                VendorId = testTransaction.VendorId,
                Amount = testTransaction.Amount,
                Description = testTransaction.Description
            };
            _mockTransactionService.Setup(m => m.Get(It.IsAny<Guid>(), userId, true)).Returns(testTransaction);
            TransactionController controller = new(_mockLogger.Object, _mockTransactionService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.AddTransaction(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var obj = (OkObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(TransactionModel));
            var resp = (TransactionModel)obj.Value;
            Assert.AreEqual(testTransaction.Id, resp.Id);
        }

        [TestMethod]
        public void AddAppExceptionHandled()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            var testTransaction = GenerateTestTransactions(userId).First();
            AddTransactionModel model = new AddTransactionModel()
            {
                TransactionDate = testTransaction.TransactionDate,
                AccountId = testTransaction.AccountId,
                CategoryId = testTransaction.CategoryId,
                VendorId = testTransaction.VendorId,
                Amount = testTransaction.Amount,
                Description = testTransaction.Description
            };
            _mockTransactionService.Setup(m => m.Get(It.IsAny<Guid>(), userId, true)).Throws(new AppException());
            TransactionController controller = new(_mockLogger.Object, _mockTransactionService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.AddTransaction(model);

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
            var userId = Guid.NewGuid();
            var testTransaction = GenerateTestTransactions(userId).First();
            AddTransactionModel model = new AddTransactionModel()
            {
                TransactionDate = testTransaction.TransactionDate,
                AccountId = testTransaction.AccountId,
                CategoryId = testTransaction.CategoryId,
                VendorId = testTransaction.VendorId,
                Amount = testTransaction.Amount,
                Description = testTransaction.Description
            };
            _mockTransactionService.Setup(m => m.Get(It.IsAny<Guid>(), userId, true)).Throws(new Exception());
            TransactionController controller = new(_mockLogger.Object, _mockTransactionService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.AddTransaction(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(GenericErrorMessage, resp.Message);
        }

        [TestMethod]
        public void AddEmptyUserGuidHandled()
        {
            // Arrange test    
            TransactionController controller = new(_mockLogger.Object, _mockTransactionService.Object);
            controller.UserId = Guid.Empty;

            // Call action
            var result = controller.AddTransaction(new AddTransactionModel());

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
        public void ModifyShouldReturnEmpty()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            var testTransaction = GenerateTestTransactions(userId).First();
            TransactionModel model = new TransactionModel(testTransaction);
            TransactionController controller = new(_mockLogger.Object, _mockTransactionService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.ModifyTransaction(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));            
        }

        [TestMethod]
        public void ModifyAppExceptionHandled()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            var testTransaction = GenerateTestTransactions(userId).First();
            TransactionModel model = new TransactionModel(testTransaction);            
            _mockTransactionService.Setup(m => m.ModifyTransaction(It.IsAny<Transaction>())).Throws(new AppException());
            TransactionController controller = new(_mockLogger.Object, _mockTransactionService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.ModifyTransaction(model);

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
            var userId = Guid.NewGuid();
            var testTransaction = GenerateTestTransactions(userId).First();
            TransactionModel model = new TransactionModel(testTransaction);
            _mockTransactionService.Setup(m => m.ModifyTransaction(It.IsAny<Transaction>())).Throws(new Exception());
            TransactionController controller = new(_mockLogger.Object, _mockTransactionService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.ModifyTransaction(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(GenericErrorMessage, resp.Message);
        }

        [TestMethod]
        public void ModifyEmptyUserGuidHandled()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            var testTransaction = GenerateTestTransactions(userId).First();
            TransactionModel model = new TransactionModel(testTransaction);
            TransactionController controller = new(_mockLogger.Object, _mockTransactionService.Object);
            controller.UserId = Guid.Empty;

            // Call action
            var result = controller.ModifyTransaction(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(AuthenticationError, resp.Message);
        }
        #endregion

        #region Delete
        [TestMethod]
        public void DeleteShouldReturnEmpty()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            var testTransactionId = Guid.NewGuid();
            TransactionController controller = new(_mockLogger.Object, _mockTransactionService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.DeleteTransaction(testTransactionId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void DeleteAppExceptionHandled()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            var testTransactionId = Guid.NewGuid();
            _mockTransactionService.Setup(m => m.DeleteTransaction(userId, testTransactionId)).Throws(new AppException());
            TransactionController controller = new(_mockLogger.Object, _mockTransactionService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.DeleteTransaction(testTransactionId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual("Exception of type 'WMMAPI.Helpers.AppException' was thrown.", resp.Message);
        }

        [TestMethod]
        public void DeleteExceptionHandled()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            var testTransactionId = Guid.NewGuid();
            _mockTransactionService.Setup(m => m.DeleteTransaction(userId, testTransactionId)).Throws(new Exception());
            TransactionController controller = new(_mockLogger.Object, _mockTransactionService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.DeleteTransaction(testTransactionId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(GenericErrorMessage, resp.Message);
        }

        [TestMethod]
        public void DeleteEmptyUserGuidHandled()
        {
            // Arrange test
            TransactionController controller = new(_mockLogger.Object, _mockTransactionService.Object);
            controller.UserId = Guid.Empty;

            // Call action
            var result = controller.DeleteTransaction(Guid.NewGuid());

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(AuthenticationError, resp.Message);
        }
        #endregion

        #region TestHelpers
        private IList<Transaction> GenerateTestTransactions(Guid userId)
        {
            IList<Transaction> transactions = new List<Transaction>();
            for (int i = 0; i < 5; i++)
            {
                transactions.Add(new Transaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TransactionDate = DateTime.Now,
                    AccountId = Guid.NewGuid(),
                    Account = new Account { Name = $"TestAccount{i}" },
                    CategoryId = Guid.NewGuid(),
                    Category = new Category { Name = $"TestCategory{i}" },
                    VendorId = Guid.NewGuid(),
                    Vendor = new Vendor { Name = $"TestVendor{i}" },
                    IsDebit = false,
                    Amount = 500M,
                    Description = "Test description"
                });
            };

            return transactions;
        }
        #endregion
    }
}
