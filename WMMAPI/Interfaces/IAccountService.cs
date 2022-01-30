using System;
using System.Collections.Generic;
using WMMAPI.Database.Entities;
using WMMAPI.Models.AccountModels;

namespace WMMAPI.Interfaces
{
    public interface IAccountService
    {
        AccountModel Get(Guid id, Guid userId);
        IList<AccountModel> GetList(Guid userId);
        void AddAccount(Account account);
        void ModifyAccount(Account account);
    }
}