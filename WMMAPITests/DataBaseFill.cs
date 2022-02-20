using NUnit.Framework;
using System;
using WMMAPI.Database;
using WMMAPI.Database.Entities;
using WMMAPI.Services;

namespace WMMAPITests
{
    public class DatabaseFillTest

    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DBShouldBuildAndFill()
        {
            using (var db = new WMMContext())
            {
                ///////////////////////
                //Create some fake data
                ///////////////////////

                //User
                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    FirstName = "Mr",
                    LastName = "Test",
                    DOB = new DateTime(1900, 11, 11),
                    EmailAddress = "Test@test.com"
                };
                var userRepo = new UserService(db);
                userRepo.Create(user, "testPassword");


                //Account
                var account = new Account
                {
                    AccountId = Guid.NewGuid(),
                    UserId = user.UserId,
                    Name = "BofA",
                    IsAsset = true,
                    IsActive = true
                };

                //Category
                var category = new Category
                {
                    CategoryId = Guid.NewGuid(),
                    UserId = user.UserId,
                    Name = "Shopping",
                    IsDefault = true,
                    IsDisplayed = true
                };

                //Vendor
                var vendor = new Vendor
                {
                    VendorId = Guid.NewGuid(),
                    UserId = user.UserId,
                    Name = "BestBuy",
                    IsDefault = false,
                    IsDisplayed = true
                };

                //Transaction Type
                var transactionTypeCredit = new TransactionType
                {
                    TransactionTypeId = Guid.NewGuid(),
                    Name = "Credit"
                };

                var transactionTypeDebit = new TransactionType
                {
                    TransactionTypeId = Guid.NewGuid(),
                    Name = "Debit"
                };

                //Transaction
                var transaction = new Transaction
                {
                    TransactionId = Guid.NewGuid(),
                    UserId = user.UserId,
                    TransactionDate = DateTime.Now,
                    TransactionTypeId = transactionTypeDebit.TransactionTypeId,
                    AccountId = account.AccountId,
                    CategoryId = category.CategoryId,
                    VendorId = vendor.VendorId,
                    Amount = 255.19M,
                    Description = "New Router"
                };

                //Insert data into the db
                db.Accounts.Add(account);
                db.Categories.Add(category);
                db.Vendors.Add(vendor);
                db.TransactionTypes.Add(transactionTypeDebit);
                db.TransactionTypes.Add(transactionTypeCredit);
                db.Transactions.Add(transaction);
                db.SaveChanges();
            }
        }
    }
}