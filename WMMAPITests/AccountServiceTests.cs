using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMMAPI.Database;
using WMMAPI.Database.Entities;
using WMMAPI.Services;

namespace WMMAPITests
{
    [TestClass]
    public class AccountServiceTests
    {
        [TestMethod]
        public void TestingMock()
        {

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

            var accounts = new List<Account>
            {
                new Account {
                    AccountId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Name = $"FakeInitialAccount",
                    IsAsset = true,
                    IsActive = true                    
                }
            }.AsQueryable();

            //var mockUserSet = new Mock<DbSet<User>>();
            //mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            //mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            //mockUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            //mockUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<WMMContext>();
            var mockAccountSet = new Mock<DbSet<Account>>();
            mockAccountSet.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(accounts.Provider);
            mockAccountSet.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(accounts.Expression);
            mockAccountSet.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(accounts.ElementType);
            mockAccountSet.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(accounts.GetEnumerator());
            
            mockContext.Setup(m => m.Accounts).Returns(mockAccountSet.Object);
            mockContext.Setup(m => m.Set<Account>()).Returns(mockAccountSet.Object);
            //mockContext.Setup(m => m.Users).Returns(mockUserSet.Object);
            
            var service = new AccountService(mockContext.Object);

            Account account = CreateRandomTestAccount();
            service.AddAccount(account);

            mockAccountSet.Verify(m => m.Add(It.IsAny<Account>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }



        // Things worth testing

        // GetAccount

        // Get balance  (figure out how to mock a context)
        //      Fake a context with known values and an assumed balance
        //      Confirm method pulls correct data


        // Name Exists (as above, figure out how to mock a context)
        //      Fake a context with known values and names
        //      Confirm method pulls correct data or ghrows valid exception


        // Worth testing mapping of Account to Account Model?

        #region PrivateHelpers
        private static Account CreateRandomTestAccount()
        {
            return new Account
            {
                AccountId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Name = $"TestName",
                IsAsset = true,
                IsActive = true
            };
        }
        #endregion




        // Dumping here temp
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
