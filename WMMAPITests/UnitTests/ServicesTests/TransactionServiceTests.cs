using WMMAPI.Services;
using WMMAPI.Services.TransactionServices;

namespace WMMAPITests.UnitTests
{
    [TestClass]
    public class TransactionServiceTests
    {
        private TestData _testData;
        private TestDataContext _tdc;

        [TestInitialize]
        public void Init()
        {
            _testData = new TestData();
            _tdc = new TestDataContext(_testData);
        }

        #region Get
        [TestMethod]
        public void TestGetSucceeds()
        {
            // Fabricate test
            var testTransaction = _testData.Transactions.First();
            _tdc.WMMContext.Setup(m => m.Transactions.AsQueryable()).Returns(_testData.Transactions);

            // Initialize service and call method
            ITransactionService service = new TransactionService(_tdc.WMMContext.Object);
            var result = service.Get(testTransaction.Id, testTransaction.UserId, false);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Transactions.AsQueryable(), Times.Once());
            Assert.AreEqual(testTransaction.Id, result.Id);
            Assert.AreEqual(testTransaction.UserId, result.UserId);
            Assert.AreEqual(testTransaction.Amount, result.Amount);
        }

        [TestMethod]
        public void TestGetSucceedsWithRelatedEntities()
        {
            // Fabricate test
            foreach (var transaction in _testData.Transactions)
            {
                transaction.User = _testData.Users.First(u => u.Id == transaction.UserId);
                transaction.Account = _testData.Accounts.First(a => a.Id == transaction.AccountId);
                transaction.Category = _testData.Categories.First(c => c.Id == transaction.CategoryId);
                transaction.Vendor = _testData.Vendors.First(v => v.Id == transaction.VendorId);
            }
            var testTransaction = _testData.Transactions.First();
            _tdc.WMMContext.Setup(m => m.Transactions.AsQueryable()).Returns(_testData.Transactions);

            // Initialize service and call method
            ITransactionService service = new TransactionService(_tdc.WMMContext.Object);
            var result = service.Get(testTransaction.Id, testTransaction.UserId, true);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Transactions.AsQueryable(), Times.Once());
            Assert.AreEqual(testTransaction.Id, result.Id);
            Assert.AreEqual(testTransaction.UserId, result.UserId);
            Assert.AreEqual(testTransaction.Amount, result.Amount);
            Assert.IsNotNull(result.Account);
            Assert.IsNotNull(result.Category);
            Assert.IsNotNull(result.Vendor);
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestGetNothingFoundThrowsException()
        {
            // Fabricate test
            _tdc.WMMContext.Setup(m => m.Transactions.AsQueryable()).Returns(_testData.Transactions);

            // Initialize service and call method
            ITransactionService service = new TransactionService(_tdc.WMMContext.Object);
            var result = service.Get(Guid.NewGuid(), Guid.NewGuid(), false);
        }
        #endregion

        #region GetList
        [TestMethod]
        public void TestGetListSucceeds()
        {
            // Fabricate test
            var testTransaction = _testData.Transactions.First();
            _tdc.WMMContext.Setup(m => m.Transactions.AsQueryable()).Returns(_testData.Transactions);

            // Initialize service and call method
            ITransactionService service = new TransactionService(_tdc.WMMContext.Object);
            var result = service.GetList(testTransaction.UserId, false);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Transactions.AsQueryable(), Times.Once());
            foreach (var transaction in result)
            {
                Assert.AreEqual(testTransaction.UserId, transaction.UserId);
            }
        }

        [TestMethod]
        public void TestGetListSucceedsWithRelatedEntities()
        {
            // Fabricate test
            foreach (var transaction in _testData.Transactions)
            {
                transaction.User = _testData.Users.First(u => u.Id == transaction.UserId);
                transaction.Account = _testData.Accounts.First(a => a.Id == transaction.AccountId);
                transaction.Category = _testData.Categories.First(c => c.Id == transaction.CategoryId);
                transaction.Vendor = _testData.Vendors.First(v => v.Id == transaction.VendorId);
            }
            var testTransaction = _testData.Transactions.First();
            _tdc.WMMContext.Setup(m => m.Transactions.AsQueryable()).Returns(_testData.Transactions);

            // Initialize service and call method
            ITransactionService service = new TransactionService(_tdc.WMMContext.Object);
            var result = service.GetList(testTransaction.UserId, true);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Transactions.AsQueryable(), Times.Once());
            foreach (var transaction in result)
            {
                Assert.AreEqual(testTransaction.UserId, transaction.UserId);
                Assert.IsNotNull(transaction.Account);
                Assert.IsNotNull(transaction.Category);
                Assert.IsNotNull(transaction.Vendor);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestGetListNothingFoundReturnsAppException()
        {
            // Fabricate test
            _tdc.WMMContext.Setup(m => m.Transactions.AsQueryable()).Returns(_testData.Transactions);

            // Initialize service and call method
            ITransactionService service = new TransactionService(_tdc.WMMContext.Object);
            var result = service.GetList(Guid.NewGuid(), false);
        }
        #endregion

        #region Add
        [TestMethod]
        public void TestAddTransactionSucceeds()
        {
            // Fabricate test
            var transaction = _testData.Transactions.First();
            var testTransaction = _testData.CreateTestTransaction(
                _testData.Accounts.First(a => a.Id == transaction.AccountId),
                transaction.IsDebit, transaction.Amount, transaction.CategoryId, transaction.VendorId,
                "This is a test transaction");

            // Initialize service and call method
            ITransactionService service = new TransactionService(_tdc.WMMContext.Object);
            service.AddTransaction(testTransaction);

            // Confirm mock and assert (no asserts, just verify Mock)
            _tdc.TransactionSet.Verify(m => m.Add(It.IsAny<Transaction>()), Times.Once());
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsTrue(_tdc.WMMContext.Object.Transactions.Any(t => t.Id == transaction.Id));
        }

        [Ignore] // TODO Return to this and implement further tests once validation exists
        [TestMethod]
        public void TestAddTransactionFails()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Modify
        [TestMethod]
        public void TestModifyTransactionSucceeds()
        {
            // Fabricate test
            var testTransaction = _testData.Transactions.First();
            testTransaction.TransactionDate = DateTime.Now.AddDays(2);
            testTransaction.Description = "test modification";
            testTransaction.Amount = 111.22M;

            // Initialize service and call method
            ITransactionService service = new TransactionService(_tdc.WMMContext.Object);
            service.ModifyTransaction(testTransaction);

            // Confirm mock and assert (no asserts, just verify mock)
            _tdc.WMMContext.Verify(m => m.Transactions, Times.Once());
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.AreEqual(testTransaction.Description, _tdc.WMMContext.Object.Transactions
                .First(t => t.Id == testTransaction.Id).Description);
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyTransactionFailsTransNotFound()
        {
            // Fabricate test
            var testTransaction = _testData
                .CreateTestTransaction(_testData.Accounts.First(), true, 100.00M, Guid.NewGuid(), Guid.NewGuid());

            // Initialize service and call method
            ITransactionService service = new TransactionService(_tdc.WMMContext.Object);
            service.ModifyTransaction(testTransaction);

            // Confirm mock and assert (not needed, exception expected)
        }
        #endregion

        #region Delete
        [TestMethod]
        public void TestDeleteTransactionSucceeds()
        {
            // Fabricate test
            var testTransaction = _testData.Transactions.First();
            _tdc.TransactionSet.Setup(m => m.Find(It.IsAny<Guid>())).Returns(testTransaction);

            // Initialize service and call method
            ITransactionService service = new TransactionService(_tdc.WMMContext.Object);
            service.DeleteTransaction(testTransaction.UserId, testTransaction.Id);

            // Confirm mock and assert
            _tdc.TransactionSet.Verify(m => m.Remove(testTransaction), Times.Once());
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestDeleteTransactionFailsNotFound()
        {
            // Fabricate test
            var testTransaction = _testData.Transactions.First();

            // Initialize service and call method
            ITransactionService service = new TransactionService(_tdc.WMMContext.Object);
            service.DeleteTransaction(testTransaction.UserId, Guid.NewGuid());

            // Confirm mock and assert (not needed, exception expected)
        }
        #endregion        
    }
}
