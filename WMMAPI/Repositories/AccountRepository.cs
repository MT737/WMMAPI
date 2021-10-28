using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Entities;
using WMMAPI.Interfaces;

namespace WMMAPI.Repositories
{
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        public AccountRepository(WMMContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves a record of a single account. 
        /// </summary>
        /// <param name="id">Guid: the AccountID of the account to be retrieved.</param>
        /// <param name="userId">Guid: the UserID of which account to be retrieved.</param>
        /// <returns></returns>
        public Account Get(Guid id, Guid userId)
        {
            return Context.Accounts
                .Where(a => a.AccountId == id && a.UserId == userId)
                .SingleOrDefault();
        }

        /// <summary>
        /// Retrieves a list of accounts that exist in the database.
        /// </summary>
        /// <param name="userID">Guid: UserID of which to pull accounts.</param>
        /// <returns>Returns and IList of Account entities.</returns>
        public IList<Account> GetList(Guid userId)
        {
            return Context.Accounts
                .Where(a => a.UserId == userId)
                .OrderBy(a => a.Name)
                .ToList();
        }

        /// <summary>
        /// Retrieves the ID of the account based on the name.
        /// </summary>
        /// <param name="name">String: name of the account of which the ID is desired.</param>
        /// <param name="userID">Guid: UserID of the account.</param>
        /// <returns></returns>
        public Guid GetId(string name, Guid userId)
        {
            return Context.Accounts
                .Where(a => a.Name == name && a.UserId == userId)
                .SingleOrDefault().AccountId;
        }

        /// <summary>
        /// Gets a count of accounts in the database.
        /// </summary>
        /// <param name="userID">Guid: UserID of which to get a count of accounts.</param>
        /// <returns>Returns an integer representing the count of accounts in the database.</returns>
        public int GetCount(Guid userId)
        {
            return Context.Accounts.Where(a => a.UserId == userId).Count();
        }

        /// <summary>
        /// Calculates an account balance based on transactions in the database.
        /// </summary>
        /// <param name="accountId">Guid: AccountID of the account balance to be calculated.</param>
        /// <param name="userID">Guid: UserID associated to the account. Only included here for security reasons.</param>
        /// <param name="isAsset">Bool: IsAsset classification of the account balance to be calculated.</param>
        /// <returns></returns>
        public decimal GetBalance(Guid accountId, Guid userId, bool isAsset)
        {
            //TODO: I really want to simplify this to a single query (there's already enough communications with the DB happening as is).
            decimal balance;

            //TODO: Further test account balances
            //TODO: Replace magic string with reference to Global
            var paymentTo = Context.Transactions
                 .Where(t => t.AccountId == accountId && t.UserId == userId && t.TransactionType.Name == "Payment To")
                 .ToList()
                 .Sum(t => t.Amount);

            var paymentFrom = Context.Transactions
                .Where(t => t.AccountId == accountId && t.UserId == userId && t.TransactionType.Name == "Payment From")
                .ToList()
                .Sum(t => t.Amount);

            // Asset balance = payments to less payments from.
            if (isAsset)
            {
                return balance = paymentTo - paymentFrom;
            }
            // Liability balance = payments from - payments to.
            else
            {
                return paymentFrom - paymentTo;
            }
        }

        /// <summary>
        /// Indicates the existence of the passed account name.
        /// </summary>
        /// <param name="desiredAccountName">String: Desired account name. </param>
        /// <param name="accountId">Guid: Account Id of which the name existence is desired.</param>
        /// <param name="userID">Guid: UserID of the account.</param>
        /// <returns>Bool: Indication of the account name's current existence in the user's DB profile.</returns>
        public bool NameExists(string desiredAccountName, Guid accountId, Guid userId)
        {
            return Context.Accounts
                 .Where(a => a.UserId == userId
                    && a.Name.ToLower() == desiredAccountName.ToLower()
                    && a.AccountId != accountId)
                 .Any();
        }

        /// <summary>
        /// Indicates if the user owns the specified account.
        /// </summary>
        /// <param name="id">Guid: ID of the specified account.</param>
        /// <param name="userID">Guid: User's ID.</param>
        /// <returns>Bool: Indication of the user's ownership fo the account.</returns>
        public bool UserOwnsAccount(Guid id, Guid userID)
        {
            return Context.Accounts.Where(a => a.AccountId == id && a.UserId == userID).Any();
        }
    }
}
