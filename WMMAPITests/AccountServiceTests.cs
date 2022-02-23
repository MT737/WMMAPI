using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Models.AccountModels;
using WMMAPI.Services;

namespace WMMAPITests
{
    [TestClass]
    public class AccountServiceTests
    {
        private IQueryable<Account> _accounts;
        private Mock<WMMContext> _mockContext;
        private Mock<DbSet<Account>> _mockAccountSet;
        
        [TestInitialize]
        public void Init()
        {
            _accounts = TestDataHelper.CreateTestAccounts().AsQueryable();
            _mockContext = new Mock<WMMContext>();
            
            _mockAccountSet = new Mock<DbSet<Account>>();
            _mockAccountSet.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(_accounts.Provider);
            _mockAccountSet.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(_accounts.Expression);
            _mockAccountSet.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(_accounts.ElementType);
            _mockAccountSet.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(_accounts.GetEnumerator());
           
            _mockContext.Setup(m => m.Accounts).Returns(_mockAccountSet.Object);
            _mockContext.Setup(m => m.Set<Account>()).Returns(_mockAccountSet.Object);          
        }

        #region TestingHelpers
        [TestMethod]
        public void TestNameExistsFalse()
        {
            // Fabricate test account
            Account testAccount = TestDataHelper.CreateTestAccount();

            // Initialize service and call method
            AccountService service = new AccountService(_mockContext.Object);
            bool reuslt = service.NameExists(testAccount);

            // Confirm mock and assert
            _mockContext.Verify(m => m.Accounts, Times.Once());
            Assert.IsFalse(reuslt);
        }

        [TestMethod]
        public void TestNameExistsTrue()
        {
            // Fabricate test account
            Account account = _accounts.First();
            Account testAccount = TestDataHelper.CreateTestAccount(account.UserId, account.Name);
            
            // Initialize service and call method
            AccountService service = new AccountService(_mockContext.Object);
            bool result = service.NameExists(testAccount);

            // Confirm mock and assert
            _mockContext.Verify(m => m.Accounts, Times.Once());
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestValidationPasses()
        {
            // Fabricate test account
            Account testAccount = TestDataHelper.CreateTestAccount();
            
            // Initialize service and call method
            AccountService service = new AccountService(_mockContext.Object);
            service.ValidateAccount(testAccount);

            // Confirm mock -- No assert. Fail state is exception thrown
            _mockContext.Verify(m => m.Accounts, Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestValidationFailsNoNameThrowsException()
        {
            // Fabricate test account
            Account testAccount = TestDataHelper.CreateTestAccount(accountName: "  ");
            
            // Initialize service and call method
            AccountService service = new AccountService(_mockContext.Object);
            service.ValidateAccount(testAccount);

            // Confirm mock -- No assertion, exception expected
            _mockContext.Verify(m => m.Accounts, Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestValidationFailsNameExistsThrowsException()
        {
            // Fabricate test account
            Account account = _accounts.First();
            Account testAccount = TestDataHelper.CreateTestAccount(account.UserId, account.Name);
            
            // Initialize service and call method
            AccountService service = new AccountService(_mockContext.Object);
            service.ValidateAccount(testAccount);

            // Confirm mock -- No assertion, exception expected
            _mockContext.Verify(m => m.Accounts, Times.Once());
        }

        [TestMethod]
        [DataRow(true, "75.25|credit;24.75|credit;10.00|debit;25.25|debit", 64.75)]
        [DataRow(false, "10.00|credit;25.25|credit;75.25|debit;24.75|debit", 64.75)]
        [DataRow(true, "10.00|credit;25.25|credit", 35.25)]
        [DataRow(true, "75.25|debit;24.75|debit", -100.00)]
        [DataRow(false, "75.25|debit;24.75|debit", 100.00)]
        [DataRow(false, "10.00|credit;25.25|credit", -35.25)]
        public void TestGetBalanceSucceeds(bool isAsset, string transStructure, double expectedBalance)
        {
            // Fabricate account
            Account testAccount = TestDataHelper.CreateTestAccount();
            testAccount.IsAsset = isAsset;

            // Arrange
            Mock<DbSet<Transaction>> trans = GenerateMockTrans(transStructure.Split(";"), testAccount);                        
            AccountService service = new AccountService(_mockContext.Object);
            decimal result = service.GetBalance(testAccount.Id, isAsset);

            // Confirm mock
            _mockContext.Verify(m => m.Transactions, Times.Exactly(2));
            Assert.AreEqual((decimal)expectedBalance, result);
        }
        #endregion

        #region Testing service methods
        [TestMethod]
        public void TestGet()
        {
            // Get test account
            Account testAccount = _accounts.First();
            
            // Arrange
            string trans = "75.25|credit;24.75|credit;10.00|debit;25.25|debit";
            Mock<DbSet<Transaction>> mockTransactionSet = GenerateMockTrans(trans.Split(';'), testAccount);            
            AccountService service = new AccountService(_mockContext.Object);
            AccountModel result = service.Get(testAccount.Id, testAccount.UserId);
            
            // Assert
            _mockContext.Verify(m => m.Accounts, Times.Once());
            _mockContext.Verify(m => m.Transactions, Times.Exactly(2));
            Assert.AreEqual(testAccount.Id, result.Id);
            Assert.AreEqual(testAccount.Name, result.Name);
        }

        [TestMethod]
        public void TestGetList()
        {
            // Get test accounts
            List<Account> testAccounts = _accounts.Take(4).ToList();
            Guid userId = Guid.NewGuid();
            foreach (var account in testAccounts)
            {
                account.UserId = userId;
            }

            // Arrange
            string transSample = "75.25|credit;24.75|credit;10.00|debit;25.25|debit";
            Mock<DbSet<Transaction>> mockTransactionSet = GenerateMockTrans(transSample.Split(';'), testAccounts.First());            
            AccountService service = new AccountService(_mockContext.Object);
            ICollection<AccountModel> results = service.GetList(userId);

            // Assert
            _mockContext.Verify(m => m.Accounts, Times.Once());
            _mockContext.Verify(m => m.Transactions, Times.Exactly(8));
            foreach (var result in results)
            {
                Assert.IsTrue(testAccounts.Any(a => a.Name == result.Name));
            }
        }

        [TestMethod]
        public void TestAddAccount()
        {
            var service = new AccountService(_mockContext.Object);

            service.AddAccount(TestDataHelper.CreateTestAccount());

            _mockAccountSet.Verify(m => m.Add(It.IsAny<Account>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        
        [TestMethod]
        public void TestModifyAccountSucceeds()
        {
            var service = new AccountService(_mockContext.Object);
            Account dbAccount = _accounts.First();
            Account testingAccount = new Account
            {
                Name = "testAccount",
                Id = dbAccount.Id,
                UserId = dbAccount.UserId
            };

            service.ModifyAccount(testingAccount);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyAccountFailsDueToNotExisting()
        {
            var service = new AccountService(_mockContext.Object);
            service.ModifyAccount(TestDataHelper.CreateTestAccount());
        }
        #endregion

        #region private methods        
        private Mock<DbSet<Transaction>> GenerateMockTrans(string[] transStructure, Account account)
        {
            List<Transaction> transList = new();
            foreach (var split in transStructure)
            {
                var tran = split.Split('|');
                transList.Add(
                    TestDataHelper.CreateTestTransaction(
                        account, decimal.Parse(tran[0]), tran[1] == "debit"));
            }

            Mock<DbSet<Transaction>> mockTransactionSet = new();
            mockTransactionSet.As<IQueryable<Transaction>>().Setup(m => m.Provider).Returns(transList.AsQueryable().Provider);
            mockTransactionSet.As<IQueryable<Transaction>>().Setup(m => m.Expression).Returns(transList.AsQueryable().Expression);
            mockTransactionSet.As<IQueryable<Transaction>>().Setup(m => m.ElementType).Returns(transList.AsQueryable().ElementType);
            mockTransactionSet.As<IQueryable<Transaction>>().Setup(m => m.GetEnumerator()).Returns(transList.AsQueryable().GetEnumerator());
            _mockContext.Setup(m => m.Transactions).Returns(mockTransactionSet.Object);

            return mockTransactionSet;
        }
        #endregion
    }
}
