using System;
using System.Collections.Generic;
using WMMAPI.Database.Models;

namespace WMMAPI.Interfaces
{
    public interface IAccountRepository : IBaseRepository<Account>
    {
        Account Get(Guid id, Guid userId);
        decimal GetBalance(Guid accountId, Guid userId, bool isAsset);
        int GetCount(Guid userId);
        Guid GetId(string name, Guid userId);
        IList<Account> GetList(Guid userId);
        bool NameExists(string desiredAccountName, Guid accountId, Guid userId);
        bool UserOwnsAccount(Guid id, Guid userID);
    }
}