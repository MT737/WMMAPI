using System;
using WMMAPI.Database.Entities;

namespace WMMAPI.Models.AccountModels
{
    public class AccountModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsAsset { get; set; }

        public bool IsActive { get; set; }

        public decimal Balance { get; set; }


        public AccountModel(Account account, decimal balance )
        {
            Id = account.Id;
            Name = account.Name;
            IsAsset = account.IsAsset;
            IsActive = account.IsActive;
            Balance = balance;
        }
    }
}
