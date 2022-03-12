using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;

namespace WMMAPI.Services
{
    public class TransactionService : BaseService<Transaction>, ITransactionService
    {
        public TransactionService(WMMContext context) : base(context)
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
                .Include(t => t.Account)
                .Include(t => t.Category)
                .Include(t => t.Vendor);
            }

            return transaction
                .Where(t => t.Id == id && t.UserId == userId)
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
        /// Adds transaction to the database
        /// </summary>
        /// <param name="transaction">Transaction model representing the transaction to be added to the database.</param>
        public void AddTransaction(Transaction transaction)
        {
            //TODO: Add transaction validation
            Add(transaction);
        }

        /// <summary>
        /// Validates changes and modifies the passed transaction.
        /// </summary>
        /// <param name="transaction"></param>
        public void ModifyTransaction(Transaction transaction)
        {
            Transaction currentTransaction = Context.Transactions
                .FirstOrDefault(t => t.Id == transaction.Id 
                    && t.UserId == transaction.UserId);

            if (currentTransaction == null)
                throw new AppException("Transaction not found.");

            //TODO: Use validation that will be used in create

            // Update properties
            currentTransaction.TransactionDate = transaction.TransactionDate;
            currentTransaction.AccountId = transaction.AccountId;
            currentTransaction.CategoryId = transaction.CategoryId;
            currentTransaction.VendorId = transaction.VendorId;
            currentTransaction.Amount = transaction.Amount;
            currentTransaction.Description = transaction.Description;

            Update(currentTransaction);
        }

        /// <summary>
        /// Deletes the specified transaction.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="transactionId"></param>
        public void DeleteTransaction(Guid userId, Guid transactionId)
        {
            bool transactionExists = Context.Transactions.Any(t => t.UserId == userId && t.Id == transactionId);
            if (!transactionExists)
                throw new AppException("Transaction not found.");

            Delete(transactionId);
        }
    }
}
