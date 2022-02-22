using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
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
        public void TestGetBalanceSucceeds(bool isAsset, string transStructure, double expectedBalance)
        {
            // Fabricate account
            Account account = TestDataHelper.CreateTestAccount();
            account.IsAsset = isAsset;
            
            // Fabricate transactions
            List<Transaction> transList = new List<Transaction>();
            var transSplit = transStructure.Split(';');
            foreach (var split in transSplit)
            {
                var tran = split.Split('|');
                transList.Add(
                    TestDataHelper.CreateTestTransaction(
                        account, decimal.Parse(tran[0]), tran[1] == "debit"));
            }
            IQueryable<Transaction> trans = transList.AsQueryable();

            // Arrange Mock transactions
            Mock<DbSet<Transaction>> mockTransactionSet = new();
            mockTransactionSet.As<IQueryable<Transaction>>().Setup(m => m.Provider).Returns(trans.Provider);
            mockTransactionSet.As<IQueryable<Transaction>>().Setup(m => m.Expression).Returns(trans.Expression);
            mockTransactionSet.As<IQueryable<Transaction>>().Setup(m => m.ElementType).Returns(trans.ElementType);
            mockTransactionSet.As<IQueryable<Transaction>>().Setup(m => m.GetEnumerator()).Returns(trans.GetEnumerator());

            _mockContext.Setup(m => m.Transactions).Returns(mockTransactionSet.Object);

            // Initialize service and call method
            AccountService service = new AccountService(_mockContext.Object);
            decimal result =service.GetBalance(account.AccountId, isAsset);

            // Confirm mock -- No assertion, exception expected
            _mockContext.Verify(m => m.Transactions, Times.Exactly(2));
            Assert.AreEqual((decimal)expectedBalance, result);
        }
        #endregion

        // Get
        [TestMethod]
        public void TestGet()
        {
            Account testAccount = _accounts.First();
            AccountService service = new AccountService(_mockContext.Object);
            AccountModel result = service.Get(testAccount.AccountId, testAccount.UserId);

            // TODO Account needs some transaction data so balance can be pulled.

            _mockContext.Verify(m => m.Accounts, Times.Once());
            Assert.AreEqual(testAccount.AccountId, result.AccountId);
            Assert.AreEqual(testAccount.Name, result.Name);
        }

        // GetList

        [TestMethod]
        public void TestAddAccount()
        {
            var service = new AccountService(_mockContext.Object);

            service.AddAccount(TestDataHelper.CreateTestAccount());

            _mockAccountSet.Verify(m => m.Add(It.IsAny<Account>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        // ModifyAccount




        // Dumping here temp

        //User user = new User
        //{
        //    UserId = Guid.NewGuid(),
        //    FirstName = "Test",
        //    LastName = "Name",
        //    DOB = DateTime.Now.AddYears(-25),
        //    EmailAddress = "test@email",
        //    PasswordHash = Encoding.ASCII.GetBytes("PWHash"),
        //    PasswordSalt = Encoding.ASCII.GetBytes("PWsalt"),
        //    IsDeleted = false
        //};

        //var users = new List<User>
        //{
        //    new User {
        //        UserId = user.UserId,
        //        FirstName = "Test",
        //        LastName = "Test",
        //        DOB = user.DOB,
        //        EmailAddress = user.EmailAddress,
        //        IsDeleted = user.IsDeleted,
        //        PasswordHash = user.PasswordHash,
        //        PasswordSalt = user.PasswordSalt
        //    }
        //}.AsQueryable();

        //var mockUserSet = new Mock<DbSet<User>>();
        //mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
        //mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
        //mockUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
        //mockUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

        //[Test]
        //public void DBShouldBuildAndFill()
        //{
        //    using (var db = new WMMContext())
        //    {
        //        ///////////////////////
        //        //Create some fake data
        //        ///////////////////////

        //        //User
        //        var user = new User
        //        {
        //            UserId = Guid.NewGuid(),
        //            FirstName = "Mr",
        //            LastName = "Test",
        //            DOB = new DateTime(1900, 11, 11),
        //            EmailAddress = "Test@test.com"
        //        };
        //        var userRepo = new UserService(db);
        //        userRepo.Create(user, "testPassword");


        //        //Account
        //        var account = new Account
        //        {
        //            AccountId = Guid.NewGuid(),
        //            UserId = user.UserId,
        //            Name = "BofA",
        //            IsAsset = true,
        //            IsActive = true
        //        };

        //        //Category
        //        var category = new Category
        //        {
        //            CategoryId = Guid.NewGuid(),
        //            UserId = user.UserId,
        //            Name = "Shopping",
        //            IsDefault = true,
        //            IsDisplayed = true
        //        };

        //        //Vendor
        //        var vendor = new Vendor
        //        {
        //            VendorId = Guid.NewGuid(),
        //            UserId = user.UserId,
        //            Name = "BestBuy",
        //            IsDefault = false,
        //            IsDisplayed = true
        //        };

        //        //Transaction Type
        //        var transactionTypeCredit = new TransactionType
        //        {
        //            TransactionTypeId = Guid.NewGuid(),
        //            Name = "Credit"
        //        };

        //        var transactionTypeDebit = new TransactionType
        //        {
        //            TransactionTypeId = Guid.NewGuid(),
        //            Name = "Debit"
        //        };

        //        //Transaction
        //        var transaction = new Transaction
        //        {
        //            TransactionId = Guid.NewGuid(),
        //            UserId = user.UserId,
        //            TransactionDate = DateTime.Now,
        //            TransactionTypeId = transactionTypeDebit.TransactionTypeId,
        //            AccountId = account.AccountId,
        //            CategoryId = category.CategoryId,
        //            VendorId = vendor.VendorId,
        //            Amount = 255.19M,
        //            Description = "New Router"
        //        };

        //        //Insert data into the db
        //        db.Accounts.Add(account);
        //        db.Categories.Add(category);
        //        db.Vendors.Add(vendor);
        //        db.TransactionTypes.Add(transactionTypeDebit);
        //        db.TransactionTypes.Add(transactionTypeCredit);
        //        db.Transactions.Add(transaction);
        //        db.SaveChanges();
        //    }
        //}
    }
}
