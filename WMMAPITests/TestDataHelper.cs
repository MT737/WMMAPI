using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;

namespace WMMAPITests
{
    internal static class TestDataHelper
    {
        internal static User CreateTestUser(string firstName = null, string lastName = null, string email = null, bool isDeleted = false)
        {
            Random random = new Random();
            int rand = random.Next(0, 1000);
            User user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = firstName ?? $"FirstName{rand}",
                LastName = lastName ?? $"LastName{rand}",
                EmailAddress = $"testemail{rand}@address.com",
                DOB = DateTime.Now.AddYears(random.Next(-55, -25)),
                //PasswordHash = "",
                //PasswordSalt = "",
                IsDeleted = isDeleted,
                Accounts = new List<Account>(),
                Categories = new List<Category>(),
                Vendors = new List<Vendor>(),
                Transactions = new List<Transaction>()
            };

            user.Accounts = CreateTestAccounts();
            user.Categories = CreateDefaultCategories(user.Id);
            user.Vendors = CreateDefaultVendors(user.Id);

            for (int i = 0; i < 10; i++)
            {
                user.Vendors.Add(CreateTestVendor(true, user.Id));
            }

            foreach (var acc in user.Accounts)
            {
                user.Transactions.Add(CreateTestTransaction(acc, random.Next(250, 7000), false, "Initial Account Setup"));
            }

            return user;
        }

        // Method for generating test accounts; 10 users with 10 accounts each = 100 accounts
        internal static List<Account> CreateTestAccounts()
        {
            List<Account> accounts = new();
            for (int i = 0; i < 10; i++)
            {
                Guid userId = Guid.NewGuid();
                for (int x = 0; x < 10; x++)
                {
                    accounts.Add(CreateTestAccount(userId, $"TestAccount{x}"));
                }
            }

            return accounts;
        }

        internal static Account CreateTestAccount(Guid? userid = null, string accountName = null)
        {
            return new Account
            {
                Id = Guid.NewGuid(),
                UserId = userid ?? Guid.NewGuid(),
                Name = accountName ?? "Test",
                IsAsset = true,
                IsActive = true
            };
        }

        // Create default categories for 10 random users
        internal static List<Category> CreateDefaultCategoriesSet()
        {
            List<Category> categoriesSet = new();
            for (int i = 0; i < 10; i++)
            {
                categoriesSet = categoriesSet.Union(CreateDefaultCategories()).ToList();
            }

            return categoriesSet;
        }

        internal static List<Category> CreateDefaultCategories(Guid? userGuid = null)
        {
            Guid userId = userGuid ?? Guid.NewGuid();
            List<Category> categories = new();
            string[] defaultCats = Globals.DefaultCategories.GetAllDefaultCategories();
            string[] notDisplayedCats = Globals.DefaultCategories.GetAllNotDisplayedDefaultCategories();
            foreach (var category in notDisplayedCats)
            {
                categories.Add(CreateTestCategory(false, category, userId));
            }

            foreach (var category in defaultCats.Except(notDisplayedCats))
            {
                categories.Add(CreateTestCategory(true, category, userId));
            }

            return categories;
        }

        internal static Category CreateTestCategory(bool isDisplayed, string name = null, Guid? userId = null, bool isDefault = true)
        {
            Random random = new Random();
            return new Category
            {
                Id = Guid.NewGuid(),
                UserId = userId ?? Guid.NewGuid(),
                Name = name ?? $"TestName{random.Next(0, 1000)}",
                IsDefault = isDefault,
                IsDisplayed = isDisplayed
            };
        }

        internal static List<Vendor> CreateDefaultVendors(Guid? userGuid = null)
        {
            Guid userId = userGuid ?? Guid.NewGuid();
            List<Vendor> vendors = new();
            string[] defaultVends = Globals.DefaultVendors.GetAllDevaultVendors();
            foreach (var dVendor in defaultVends)
            {
                vendors.Add(new Vendor
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = dVendor,
                    IsDefault = true,
                    IsDisplayed = true
                });
            }

            return vendors;
        }

        internal static Vendor CreateTestVendor(bool isDisplayed, Guid? userId = null, string name = null, bool isDefault = true)
        {
            Random random = new Random();
            return new Vendor
            {
                Id = Guid.NewGuid(),
                UserId = userId ?? Guid.NewGuid(),
                Name = name ?? $"TestName{random.Next(0, 1000)}",
                IsDisplayed = isDisplayed,
                IsDefault = isDefault
            };
        }

        internal static Transaction CreateTestTransaction(Account account, decimal amount, bool isDebit, string description = null)
        {
            return new Transaction
            {
                UserId = account.UserId,
                AccountId = account.Id,
                IsDebit = isDebit,
                Amount = amount,
                TransactionDate = DateTime.UtcNow,
                //CategoryId = ,
                //VendorId = ,
                Description = description ?? "No description provided"
            };
        }       
    }
}
