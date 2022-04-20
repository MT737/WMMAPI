using System;
using System.Collections.Generic;
using WMMAPI.Database.Entities;
using WMMAPI.Services.AccountServices.AccountModels;

namespace WMMAPI.Interfaces
{
    public interface IAccountService
    {
        AccountModel Get(Guid id, Guid userId);
        IList<AccountModel> GetList(Guid userId);
        AccountModel AddAccount(Account account, decimal balance);
        void ModifyAccount(Account account);
    }
}