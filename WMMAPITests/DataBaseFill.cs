using NUnit.Framework;
using System;
using WMMAPI.Database;
using WMMAPI.Database.Models;

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
                    FirstName = "Mark",
                    LastName = "Taylor",
                    DOB = new DateTime(1983, 12, 11),
                    EmailAddress = "mark.taylor737@gmail.com"
                };

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
                var transactionType = new TransactionType
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
                    TransactionTypeId = transactionType.TransactionTypeId,
                    AccountId = account.AccountId,
                    CategoryId = category.CategoryId,
                    VendorId = vendor.VendorId,
                    Amount = 255.19M,
                    Description = "New Router"
                };

                //Insert data into the db
                db.Users.Add(user);
                db.Accounts.Add(account);
                db.Categories.Add(category);
                db.Vendors.Add(vendor);
                db.TransactionTypes.Add(transactionType);
                db.Transactions.Add(transaction);
                db.SaveChanges();
            }
        }
    }
}