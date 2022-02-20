using System;
using System.Collections.Generic;
using WMMAPI.Database.Entities;

namespace WMMAPITests
{
    public static class TestDataHelper
    {  

        // Method for generating test accounts; 10 users with 10 accounts each = 100 accounts
        public static List<Account> CreateTestAccounts()
        {
            var accounts = new List<Account>();
            
            for (int i = 0; i < 10; i++)
            {
                var userId = Guid.NewGuid();

                for (int x = 0; x < 10; x++)
                {
                    var id = Guid.NewGuid();
                    accounts.Add(new Account
                    {
                        AccountId = id,
                        UserId = userId,
                        Name = $"TestAccount: {id}",
                        IsAsset = true,
                        IsActive = true
                    });
                }
            }

            return accounts;
        }

        public static Account CreateTestAccount(Guid? userid = null, string accountName = null)
        {
            return new Account
            {
                AccountId = Guid.NewGuid(),
                UserId = userid ?? Guid.NewGuid(),
                Name = accountName ?? "Test",
                IsAsset = true,
                IsActive= true
            };
        }

        // Method for generating test transactions...
    }
}
