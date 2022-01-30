using System;
using System.Collections.Generic;
using WMMAPI.Database.Entities;

namespace WMMAPI.Interfaces
{
    public interface ITransactionService
    {
        void DeleteTransaction(Guid userId, Guid transactionId);
        Transaction Get(Guid id, Guid userId, bool includeRelatedEntities = false);
        IList<Transaction> GetList(Guid userId, bool includeRelatedEntities = false);
        void AddTransaction(Transaction transaction);
        void ModifyTransaction(Transaction model);
    }
}