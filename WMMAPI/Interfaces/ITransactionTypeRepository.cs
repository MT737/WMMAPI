using System.Collections.Generic;
using WMMAPI.Database.Models;

namespace WMMAPI.Interfaces
{
    public interface ITransactionTypeRepository : IBaseRepository<TransactionType>
    {
        IList<TransactionType> GetList();
        TransactionType GetTransactionType(string name);
    }
}