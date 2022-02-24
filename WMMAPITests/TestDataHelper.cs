using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;

namespace WMMAPITests
{
    internal static class TestDataHelper
    {

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

        internal static List<Category> CreateDefaultCategoriesSet()
        {
            List<Category> categoriesSet = new();
            for (int i = 0; i < 10; i++)
            {
                categoriesSet = categoriesSet.Union(CreateDefaultCategories()).ToList();
            }

            return categoriesSet;
        }

        internal static List<Category> CreateDefaultCategories()
        {
            Guid userId = Guid.NewGuid();
            List<Category> categories = new();
            string[] defaultCats = Globals.DefaultCategories.GetAllDefaultCategories();
            string[] notDisplayedCats = Globals.DefaultCategories.GetAllNotDisplayedDefaultCategories();
            foreach (var category in notDisplayedCats)
            {
                categories.Add(CreateTestCategory(category, false, userId));
            }

            foreach (var category in defaultCats.Except(notDisplayedCats))
            {
                categories.Add(CreateTestCategory(category, true, userId));
            }

            return categories;
        }

        internal static Category CreateTestCategory(string name, bool isDisplayed, Guid userId, bool isDefault = true)
        {
            return new Category
            {
                Id = Guid.NewGuid(),
                Name = name,
                IsDefault = isDefault,
                IsDisplayed = isDisplayed,
                UserId = userId
            };
        }

        // Method for generating test transactions...
        internal static Transaction CreateTestTransaction(Account account, decimal amount, bool isDebit)
        {
            return new Transaction
            {
                UserId = account.UserId,
                AccountId = account.Id,
                IsDebit = isDebit,
                Amount = amount
            };
        }
    }
}
