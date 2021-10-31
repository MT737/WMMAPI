using System;
using System.Collections.Generic;
using WMMAPI.Database.Entities;

namespace WMMAPI.Interfaces
{
    public interface ITransactionRepository : IBaseRepository<Transaction>
    {
        void DeleteTransaction(Guid userId, Guid transactionId);
        Transaction Get(Guid id, Guid userId, bool includeRelatedEntities = false);
        int GetCount(Guid userId);
        IList<Transaction> GetList(Guid userId, bool includeRelatedEntities = false);
        void ModifyTransaction(Transaction model);
        bool UserOwnsTransaction(Guid id, Guid userId);
    }
}