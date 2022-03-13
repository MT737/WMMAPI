using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Services;
using WMMAPITests.DataHelpers;

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
            _tdc.WMMContext.Setup(m => m.Set<Transaction>()).Returns(_tdc.TransactionSet.Object);
        }


        [TestMethod]
        public void TestGetSucceeds()
        {
            // Fabricate test
            var testTransaction = _testData.Transactions.First();
            _tdc.WMMContext.Setup(m => m.Transactions.AsQueryable()).Returns(_testData.Transactions);

            // Initialize service and call method
            TransactionService service = new TransactionService(_tdc.WMMContext.Object);
            var result = service.Get(testTransaction.Id, testTransaction.UserId, false);

            // Confirm mock and assert
            Assert.AreEqual(testTransaction.Id, result.Id);
            Assert.AreEqual(testTransaction.UserId, result.UserId);
            Assert.AreEqual(testTransaction.Amount, result.Amount);
        }

        [TestMethod]
        public void TestGetSucceedsWithRelatedEntities()
        {
            // Fabricate test
            // TODO Come back to this queryable issue
            // Arrange relationships (moq context can't handle queryable includes)
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
            TransactionService service = new TransactionService(_tdc.WMMContext.Object);
            var result = service.Get(testTransaction.Id, testTransaction.UserId, true);

            // Confirm mock and assert
            Assert.AreEqual(testTransaction.Id, result.Id);
            Assert.AreEqual(testTransaction.UserId, result.UserId);
            Assert.AreEqual(testTransaction.Amount, result.Amount);
            Assert.IsNotNull(result.Account);
            Assert.IsNotNull(result.Category);
            Assert.IsNotNull(result.Vendor);            
        }

        [TestMethod]
        public void TestGetReturnsNullIfNothingFound()
        {   
            // Fabricate test
            _tdc.WMMContext.Setup(m => m.Transactions.AsQueryable()).Returns(_testData.Transactions);

            // Initialize service and call method
            TransactionService service = new TransactionService(_tdc.WMMContext.Object);
            var result = service.Get(Guid.NewGuid(), Guid.NewGuid(), false);

            // Confirm mock and assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestGetListSucceeds()
        {
            // Fabricate test
            var testTransaction = _testData.Transactions.First();
            _tdc.WMMContext.Setup(m => m.Transactions.AsQueryable()).Returns(_testData.Transactions);

            // Initialize service and call method
            TransactionService service = new TransactionService(_tdc.WMMContext.Object);
            var result = service.GetList(testTransaction.UserId, true);

            // Confirm mock and assert
            foreach (var transaction in result)
            {
                Assert.AreEqual(testTransaction.UserId, transaction.UserId);             
            }
        }

        [TestMethod]
        public void TestGetListSucceedsWithRelatedEntities()
        {
            // Fabricate test
            // TODO Come back to this queryable issue
            // Arrange relationships (moq context can't handle queryable includes)
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
            TransactionService service = new TransactionService(_tdc.WMMContext.Object);
            var result = service.GetList(testTransaction.UserId, true);

            // Confirm mock and assert
            foreach (var transaction in result)
            {
                Assert.AreEqual(testTransaction.UserId, transaction.UserId);
                Assert.IsNotNull(transaction.Account);
                Assert.IsNotNull(transaction.Category);
                Assert.IsNotNull(transaction.Vendor);
            }
        }

        [TestMethod]
        public void TestGetListReturnsEmptyListIfNothingFound()
        {
            // Fabricate test
            _tdc.WMMContext.Setup(m => m.Transactions.AsQueryable()).Returns(_testData.Transactions);

            // Initialize service and call method
            TransactionService service = new TransactionService(_tdc.WMMContext.Object);
            var result = service.GetList(Guid.NewGuid(), false);

            // Confirm mock and assert
            Assert.IsTrue(result.Count == 0);
        }

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
            TransactionService service = new TransactionService(_tdc.WMMContext.Object);
            service.AddTransaction(testTransaction);

            // Confirm mock and assert (no asserts, just verify Mock)
            _tdc.TransactionSet.Verify(m => m.Add(It.IsAny<Transaction>()), Times.Once());
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Ignore] // TODO Return to this and implement further tests once validation exists
        [TestMethod]
        public void TestAddTransactionFails()
        {
            throw new NotImplementedException();
        }
        
        [TestMethod]
        public void TestModifyTransactionSucceeds()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyTransactionFailsTransNotFound()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestDeleteTransactionSucceeds()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestDeleteTransactionFailsNotFound()
        {
            throw new NotImplementedException();
        }
    }
}
