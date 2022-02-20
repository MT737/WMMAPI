using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
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


        [TestMethod]
        public void TestingMock()
        {   
            var service = new AccountService(_mockContext.Object);

            service.AddAccount(TestDataHelper.CreateTestAccount());

            _mockAccountSet.Verify(m => m.Add(It.IsAny<Account>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
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
            Account testAccount = TestDataHelper.CreateTestAccount();
            testAccount.Name = _accounts.First().Name;
            testAccount.UserId = _accounts.First().UserId;

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
            testAccount.Name = "Test";

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
            Account testAccount = TestDataHelper.CreateTestAccount();
            testAccount.Name = "   ";

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
            Account testAccount = TestDataHelper.CreateTestAccount();
            testAccount.Name = _accounts.First().Name;
            testAccount.UserId = _accounts.First().UserId;

            // Initialize service and call method
            AccountService service = new AccountService(_mockContext.Object);
            service.ValidateAccount(testAccount);

            // Confirm mock -- No assertion, exception expected
            _mockContext.Verify(m => m.Accounts, Times.Once());
        }

        // GetBalance
        [TestMethod]
        public void TestGetBalanceSucceeds()
        {
            // TODO Over in TestDataHelper; generate a method for generating a list of test transactions
            // Build in such a way as to be able to set the balance...
        }

        #endregion

        // Get

        // GetList

        // AddAccount

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
