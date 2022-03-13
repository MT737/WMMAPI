using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Models.AccountModels;
using WMMAPI.Services;
using WMMAPITests.DataHelpers;

namespace WMMAPITests.UnitTests
{
    [TestClass]
    public class AccountServiceTests
    {
        private TestData _testData;
        private TestDataContext _tdc;
        
        [TestInitialize]
        public void Init()
        {
            _testData = new TestData();            
            _tdc = new TestDataContext(_testData);
            _tdc.WMMContext.Setup(m => m.Set<Account>()).Returns(_tdc.AccountSet.Object);          
        }

        #region TestingHelpers
        [TestMethod]
        public void TestNameExistsFalse()
        {
            // Fabricate test account
            Account testAccount = _testData.CreateTestAccount();

            // Initialize service and call method
            AccountService service = new AccountService(_tdc.WMMContext.Object);
            bool reuslt = service.NameExists(testAccount);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Accounts, Times.Once());
            Assert.IsFalse(reuslt);
        }

        [TestMethod]
        public void TestNameExistsTrue()
        {
            // Fabricate test account
            Account account = _testData.Accounts.First();
            Account testAccount = _testData.CreateTestAccount(account.UserId, account.Name);
            
            // Initialize service and call method
            AccountService service = new AccountService(_tdc.WMMContext.Object);
            bool result = service.NameExists(testAccount);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Accounts, Times.Once());
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestValidationPasses()
        {
            // Fabricate test account
            Account testAccount = _testData.CreateTestAccount();
            
            // Initialize service and call method
            AccountService service = new AccountService(_tdc.WMMContext.Object);
            service.ValidateAccount(testAccount);

            // Confirm mock -- No assert. Fail state is exception thrown
            _tdc.WMMContext.Verify(m => m.Accounts, Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestValidationFailsNoNameThrowsException()
        {
            // Fabricate test account
            Account testAccount = _testData.CreateTestAccount(accountName: "  ");
            
            // Initialize service and call method
            AccountService service = new AccountService(_tdc.WMMContext.Object);
            service.ValidateAccount(testAccount);

            // Confirm mock -- No assertion, exception expected
            _tdc.WMMContext.Verify(m => m.Accounts, Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestValidationFailsNameExistsThrowsException()
        {
            // Fabricate test account
            Account account = _testData.Accounts.First();
            Account testAccount = _testData.CreateTestAccount(account.UserId, account.Name);
            
            // Initialize service and call method
            AccountService service = new AccountService(_tdc.WMMContext.Object);
            service.ValidateAccount(testAccount);

            // Confirm mock -- No assertion, exception expected
            _tdc.WMMContext.Verify(m => m.Accounts, Times.Once());
        }

        [DataTestMethod]
        [DataRow(true, "75.25|credit;24.75|credit;10.00|debit;25.25|debit", 64.75)]
        [DataRow(false, "10.00|credit;25.25|credit;75.25|debit;24.75|debit", 64.75)]
        [DataRow(true, "10.00|credit;25.25|credit", 35.25)]
        [DataRow(true, "75.25|debit;24.75|debit", -100.00)]
        [DataRow(false, "75.25|debit;24.75|debit", 100.00)]
        [DataRow(false, "10.00|credit;25.25|credit", -35.25)]
        public void TestGetBalanceSucceeds(bool isAsset, string transStructure, double expectedBalance)
        {
            // Fabricate account
            Account testAccount = _testData.CreateTestAccount();
            testAccount.IsAsset = isAsset;

            // Arrange
            GenerateMockTrans(transStructure.Split(";"), testAccount);
            _tdc = new(_testData);
            AccountService service = new AccountService(_tdc.WMMContext.Object);
            decimal result = service.GetBalance(testAccount.Id, isAsset);

            // Confirm mock
            _tdc.WMMContext.Verify(m => m.Transactions, Times.Exactly(2));
            Assert.AreEqual((decimal)expectedBalance, result);
        }
        #endregion

        #region Testing service methods
        [TestMethod]
        public void TestGet()
        {
            // Create test account with transactions
            Account testAccount = new Account
            {
                Id = Guid.NewGuid(),
                Name = "TestingGetAccount",
                UserId = _testData.Users.First().Id,
                IsAsset = true,
                IsActive = true
            };
            _testData.Accounts = _testData.Accounts.Concat( new List<Account> { testAccount });            
            
            string trans = "75.25|credit;24.75|credit;10.00|debit;25.25|debit";
            GenerateMockTrans(trans.Split(';'), testAccount);
                        
            // Arrange; Need to update the dbsets with the new data
            _tdc = new(_testData);
            AccountService service = new AccountService(_tdc.WMMContext.Object);
            AccountModel result = service.Get(testAccount.Id, testAccount.UserId);
            
            // Assert
            _tdc.WMMContext.Verify(m => m.Accounts, Times.Once());
            _tdc.WMMContext.Verify(m => m.Transactions, Times.Exactly(2));
            Assert.AreEqual(testAccount.Id, result.Id);
            Assert.AreEqual(testAccount.Name, result.Name);
            Assert.AreEqual((decimal)(75.25 + 24.75) - (decimal)(10 + 25.25), result.Balance); // TO DO Improve this use of values
        }

        [TestMethod]
        public void TestGetList()
        {
            // Get test accounts
            List<Account> testAccounts = _testData.Accounts.Take(4).ToList();
            Guid userId = Guid.NewGuid();
            foreach (var account in testAccounts)
            {
                account.UserId = userId;
            }

            // Arrange
            string transSample = "75.25|credit;24.75|credit;10.00|debit;25.25|debit";
            GenerateMockTrans(transSample.Split(';'), testAccounts.First());            
            AccountService service = new AccountService(_tdc.WMMContext.Object);
            ICollection<AccountModel> results = service.GetList(userId);

            // Assert
            _tdc.WMMContext.Verify(m => m.Accounts, Times.Once());
            _tdc.WMMContext.Verify(m => m.Transactions, Times.Exactly(8));
            foreach (var result in results)
            {
                Assert.IsTrue(testAccounts.Any(a => a.Name == result.Name));
            }
        }

        [TestMethod]
        public void TestAddAccount()
        {
            var service = new AccountService(_tdc.WMMContext.Object);

            service.AddAccount(_testData.CreateTestAccount());

            _tdc.AccountSet.Verify(m => m.Add(It.IsAny<Account>()), Times.Once());
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
        }
        
        [TestMethod]
        public void TestModifyAccountSucceeds()
        {
            var service = new AccountService(_tdc.WMMContext.Object);
            Account dbAccount = _testData.Accounts.First();
            Account testingAccount = new Account
            {
                Name = "testAccount",
                Id = dbAccount.Id,
                UserId = dbAccount.UserId
            };

            service.ModifyAccount(testingAccount);
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyAccountFailsDueToNotExisting()
        {
            var service = new AccountService(_tdc.WMMContext.Object);
            service.ModifyAccount(_testData.CreateTestAccount());
        }
        #endregion

        #region private methods        
        private void GenerateMockTrans(string[] transStructure, Account account)
        {
            List<Transaction> transList = new();
            foreach (var split in transStructure)
            {
                var tran = split.Split('|');
                transList.Add(
                    _testData.CreateTestTransaction(
                        account, tran[1] == "debit", decimal.Parse(tran[0]), Guid.NewGuid(), Guid.NewGuid()));
            }
                        
            _testData.Transactions = _testData.Transactions.Concat(transList);
        }
        #endregion
    }
}
