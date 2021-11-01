﻿using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Entities;
using WMMAPI.Interfaces;

namespace WMMAPI.Services
{
    public class TransactionTypeService : BaseService<TransactionType>
    {
        public TransactionTypeService(WMMContext context) : base(context)
        {
        }

        /// <summary>
        /// Returns a list of transactiontype entities.
        /// </summary>
        /// <returns>IList of transactiontype entities.</returns>
        public IList<TransactionType> GetList()
        {
            return Context.TransactionTypes
                .OrderBy(tt => tt.TransactionTypeId)
                .ToList();
        }

        /// <summary>
        /// Retrieves the transactionType possessing the transaction type name.
        /// </summary>
        /// <param name="name">String: Name of the transaction type.</param>
        /// <returns>TransactionType: Transaction Type associated to the passed name.</returns>
        public TransactionType GetTransactionType(string name)
        {
            return Context.TransactionTypes
                .Where(tt => tt.Name.ToLower() == name.ToLower())
                .SingleOrDefault();
        }
    }
}