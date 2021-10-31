using System;
using System.Collections.Generic;
using WMMAPI.Database.Entities;

namespace WMMAPI.Interfaces
{
    //TODO: Refactor to remove unused methods (limiting controller interactions)
    public interface IAccountService
    {
        Account Get(Guid id, Guid userId);
        IList<Account> GetList(Guid userId);
        void AddAccount(Account account);
        void ModifyAccount(Account account);
        decimal GetBalance(Guid accountId, Guid userId, bool isAsset);
    }
}