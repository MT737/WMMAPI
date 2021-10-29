using System;
using System.Collections.Generic;
using WMMAPI.Database.Entities;

namespace WMMAPI.Interfaces
{
    //TODO: Refactor to remove unused methods (limiting controller interactions)
    public interface IAccountRepository : IBaseRepository<Account>
    {
        Account Get(Guid id, Guid userId);
        decimal GetBalance(Guid accountId, Guid userId, bool isAsset);
        int GetCount(Guid userId);
        Guid GetId(string name, Guid userId);
        IList<Account> GetList(Guid userId);
        bool NameExists(Account account);
        bool UserOwnsAccount(Guid id, Guid userID);
        void AddAccount(Account account);
    }
}