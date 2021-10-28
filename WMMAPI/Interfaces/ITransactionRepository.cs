using System;
using System.Collections.Generic;
using WMMAPI.Database.Entities;

namespace WMMAPI.Interfaces
{
    public interface ITransactionRepository : IBaseRepository<Transaction>
    {
        Transaction Get(Guid id, Guid userId, bool includeRelatedEntities = false);
        int GetCount(Guid userId);
        IList<Transaction> GetList(Guid userId, bool includeRelatedEntities = false);
        bool UserOwnsTransaction(Guid id, Guid userId);
    }
}