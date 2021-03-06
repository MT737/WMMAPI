using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;

namespace WMMAPITests.DataHelpers
{
    internal class TestData
    {
        internal IQueryable<User> Users { get; set; } = new List<User>().AsQueryable();
        internal IQueryable<Account> Accounts { get; set; } = new List<Account>().AsQueryable();
        internal IQueryable<Transaction> Transactions { get; set; } = new List<Transaction>().AsQueryable();
        internal IQueryable<Category> Categories { get; set; } = new List<Category>().AsQueryable();
        internal IQueryable<Vendor> Vendors { get; set; } = new List<Vendor>().AsQueryable();
                
        internal static Random _random = new Random();

        internal TestData()
        {
            CreateTestData();            
        }
       
        internal void CreateTestData()
        {
            List<User> users = new List<User>();
            for (int i = 0; i < 2; i++)
            {
                users.Add(CreateTestUser());
            }
            
            Users = users.AsQueryable();            
            foreach (User user in Users)
            {
                List<Account> accounts = CreateTestAccounts(user.Id);
                List<Category> categories = CreateDefaultCategories(user.Id);
                for (int i = 0; i < 3; i++)
                {
                    categories.Add(CreateTestCategory(true, null, user.Id, false));
                }

                List<Vendor> vendors = CreateDefaultVendors(user.Id);
                for (int i = 0; i < 10; i++)
                {
                    vendors.Add(CreateTestVendor(true, user.Id, false));
                }

                List<Transaction> transactions = new List<Transaction>();
                foreach (Account account in accounts)
                {
                    transactions.Add(
                    CreateTestTransaction(
                        account,
                        false,
                        _random.Next(250, 7000),
                        categories.First(c => c.Name == Globals.DefaultCategories.NewAccount).Id,
                        vendors.First(v => v.Name == Globals.DefaultVendors.NA).Id,
                        "Initial Account Setup"
                        )
                    );
                }

                Accounts = Accounts.Concat(accounts);
                Categories = Categories.Concat(categories);
                Vendors = Vendors.Concat(vendors);
                Transactions = Transactions.Concat(transactions);
            }            
        }

        internal User CreateTestUser(string password = null, string firstName = null, string lastName = null, string email = null, bool isDeleted = false)
        {
            password = password ?? Guid.NewGuid().ToString();            
            byte[] passwordSalt;
            byte[] passwordHash;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

            int rand = _random.Next(0, 1000);
            return new User
            {
                Id = Guid.NewGuid(),
                FirstName = firstName ?? $"FirstName{rand}",
                LastName = lastName ?? $"LastName{rand}",
                EmailAddress = email ?? $"testemail{rand}@address.com",
                DOB = DateTime.Now.AddYears(_random.Next(-55, -25)),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                IsDeleted = isDeleted
            };
        }

        internal List<Account> CreateTestAccounts(Guid userId)
        {
            List<Account> accounts = new();
            for (int i = 0; i < 10; i++)
            {
                accounts.Add(CreateTestAccount(userId, $"TestAccount{i}"));                
            }

            return accounts;
        }

        internal Account CreateTestAccount(Guid? userid = null, string accountName = null)
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

        internal List<Category> CreateDefaultCategories(Guid? userid = null)
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

        internal Category CreateTestCategory(bool isDisplayed, string name = null, Guid? userId = null, bool isDefault = true)
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

        internal List<Vendor> CreateDefaultVendors(Guid? userGuid = null)
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

        internal Vendor CreateTestVendor(bool isDisplayed, Guid? userId = null, bool isDefault = true, string name = null)
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

        internal Transaction CreateTestTransaction(Account account, bool isDebit, decimal amount, Guid categoryId, Guid vendorId, string description = null)
        {
            return new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = account.UserId,
                AccountId = account.Id,
                TransactionDate = DateTime.UtcNow,
                IsDebit = isDebit,
                Amount = amount,
                CategoryId = categoryId,
                VendorId = vendorId,
                Description = description ?? "No description provided"
            };
        }       
    }
}
