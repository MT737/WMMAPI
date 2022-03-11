using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;

namespace WMMAPITests.DataHelpers
{
    internal static class TestDataHelper
    {
        internal static Random _random = new Random();
       
        internal static User CreateTestUser(string firstName = null, string lastName = null, string email = null, bool isDeleted = false)
        {
            int rand = _random.Next(0, 1000);
            User user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = firstName ?? $"FirstName{rand}",
                LastName = lastName ?? $"LastName{rand}",
                EmailAddress = $"testemail{rand}@address.com",
                DOB = DateTime.Now.AddYears(_random.Next(-55, -25)),
                //PasswordHash = "",
                //PasswordSalt = "",
                IsDeleted = isDeleted,
                Accounts = new List<Account>(),
                Categories = new List<Category>(),
                Vendors = new List<Vendor>(),
                Transactions = new List<Transaction>()
            };

            user.Accounts = CreateTestAccounts(user.Id);
            user.Categories = CreateDefaultCategories(user.Id);
            user.Vendors = CreateDefaultVendors(user.Id);

            for (int i = 0; i < 10; i++)
            {
                user.Vendors.Add(CreateTestVendor(true, user.Id));
            }

            foreach (var acc in user.Accounts)
            {
                user.Transactions.Add(
                    CreateTestTransaction(
                        acc,
                        false,
                        _random.Next(250, 7000),
                        user.Categories.First(c => c.Name == Globals.DefaultCategories.NewAccount).Id,
                        user.Vendors.First(v => v.Name == Globals.DefaultVendors.NA).Id,
                        "Initial Account Setup"
                        )
                    );
            }

            return user;
        }

        internal static List<Account> CreateTestAccounts(Guid userId)
        {
            List<Account> accounts = new();
            for (int i = 0; i < 10; i++)
            {
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

        internal static List<Category> CreateDefaultCategoriesSet(Guid userId)
        {
            List<Category> categoriesSet = new();
            for (int i = 0; i < 10; i++)
            {
                categoriesSet = categoriesSet.Union(CreateDefaultCategories(userId)).ToList();
            }

            return categoriesSet;
        }

        internal static List<Category> CreateDefaultCategories(Guid? userid = null)
        {
            Guid userId = userid ?? Guid.NewGuid();
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
            return new Category
            {
                Id = Guid.NewGuid(),
                UserId = userId ?? Guid.NewGuid(),
                Name = name ?? $"TestName{_random.Next(0, 1000)}",
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
            return new Vendor
            {
                Id = Guid.NewGuid(),
                UserId = userId ?? Guid.NewGuid(),
                Name = name ?? $"TestName{_random.Next(0, 1000)}",
                IsDisplayed = isDisplayed,
                IsDefault = isDefault
            };
        }

        internal static Transaction CreateTestTransaction(Account account, bool isDebit, decimal amount, Guid categoryId, Guid vendorId, string description = null)
        {
            return new Transaction
            {
                UserId = account.UserId,
                AccountId = account.Id,
                TransactionDate = DateTime.UtcNow,
                IsDebit = isDebit,
                Amount = amount,
                CategoryId = categoryId,
                VendorId = categoryId,
                Description = description ?? "No description provided"
            };
        }       
    }
}
