using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Models;
using WMMAPI.Interfaces;

namespace WMMAPI.Repositories
{
    public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(WMMContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves the transaction associated to the passed transactionId if it belongs to the passed UserId.
        /// </summary>
        /// <param name="id">Guid: TransactionId</param>
        /// <param name="userId">Guid: UserId</param>
        /// <param name="includeRelatedEntities">Bool: indicates the need to include related entities. Defaults to false.</param>
        /// <returns>Returns a transaction entity along with its related entities</returns>
        public Transaction Get(Guid id, Guid userId, bool includeRelatedEntities = false)
        {
            var transaction = Context.Transactions.AsQueryable();

            if (includeRelatedEntities)
            {
                transaction
                .Include(t => t.TransactionType)
                .Include(t => t.Account)
                .Include(t => t.Category)
                .Include(t => t.Vendor);
            }

            return transaction
                .Where(t => t.TransactionId == id && t.UserId == userId)
                .SingleOrDefault();
        }

        /// <summary>
        /// Retrieves a list of transactions belonging to the passed userId.
        /// </summary>
        /// <param name="userId">Guid: UserId for which to pull a list of transactions.</param>
        /// <param name="includeRelatedEntities">Bool: indicates the need to include related entities. Defaults to false.</param>
        /// <returns>Returns IList of Transaction entities.</returns>
        public IList<Transaction> GetList(Guid userId, bool includeRelatedEntities = false)
        {
            var transactions = Context.Transactions.AsQueryable();

            if (includeRelatedEntities)
            {
                transactions
                    .Include(t => t.TransactionType)
                    .Include(t => t.Account)
                    .Include(t => t.Category)
                    .Include(t => t.Vendor);
            }

            return transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();
        }

        /// <summary>
        /// Returns an integer representing the count of transactions belonging to the passed user.
        /// </summary>
        /// <param name="userID">Guid: UserId of which to get a count of transactions.</param>
        /// <returns>Integer: count of transactions belonging to the user.</returns>
        public int GetCount(Guid userId)
        {
            return Context.Transactions.Where(t => t.UserId == userId).Count();
        }

        /// <summary>
        /// Idicates the user's ownership status of a given transaction.
        /// </summary>
        /// <param name="id">Guid: TransactionId of the transaction to be inspected.</param>
        /// <param name="userId">Guid: UserId of the user to be compared to the transaction.</param>
        /// <returns>Bool: true if the user does own the transaction and false if not.</returns>
        public bool UserOwnsTransaction(Guid id, Guid userId)
        {
            return Context.Transactions.Where(t => t.TransactionId == id && t.UserId == userId).Any();
        }
    }
}
