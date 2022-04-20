using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Services.AccountServices.AccountModels;

namespace WMMAPI.Services.AccountServices
{
    public class AccountService : BaseService<Account>, IAccountService
    {
        public AccountService(WMMContext context) : base(context)
        {
        }

        #region ServiceMethods
        /// <summary>
        /// Retrieves the record of the requested account. 
        /// </summary>
        /// <param name="id">Guid: the AccountID of the account to be retrieved.</param>
        /// <param name="userId">Guid: the UserID of which account to be retrieved.</param>
        /// <returns>Account model for the requested account.</returns>
        public AccountModel Get(Guid id, Guid userId)
        {   
            var account = Context.Accounts
                .Where(a => a.Id == id && a.UserId == userId)
                .SingleOrDefault();

            if (account == null)
                throw new AppException("Account not found.");

            return new AccountModel(account, GetBalance(account.Id, account.IsAsset));
        }

        /// <summary>
        /// Retrieves an IList of accounts that exist in the database.
        /// </summary>
        /// <param name="userID">Guid: UserID of which to pull accounts.</param>
        /// <returns>An IList of Accounts with balances.</returns>
        public IList<AccountModel> GetList(Guid userId)
        {
            var accounts = Context.Accounts
                .Where(a => a.UserId == userId)
                .OrderBy(a => a.Name)
                .ToList();

            if (accounts.Count == 0)
                throw new AppException("No accounts found.");
            
            return accounts
                .Select(a => new AccountModel(a, GetBalance(a.Id, a.IsAsset)))
                .ToList();
        }

        /// <summary>
        /// Validates and adds account to the database.
        /// </summary>
        /// <param name="newAccount">Account to add to the database. Throws AppException if validation fails.</param>
        public AccountModel AddAccount(Account newAccount, decimal balance)
        {
            // Validate account. Validation errors result in thrown exceptions.
            ValidateAccount(newAccount);

            var thingy = Context.Vendors
                    .Single(v => v.UserId == newAccount.UserId && v.Name == Globals.DefaultVendors.NA).Id;

            // If still here, validation passed. Add a new account transaction.
            newAccount.Transactions = new List<Transaction>
            {
                new Transaction
                {
                    Id = Guid.NewGuid(),
                    UserId = newAccount.UserId,
                    TransactionDate = DateTime.UtcNow,
                    AccountId = newAccount.Id,
                    CategoryId = Context.Categories
                        .Single(c => c.UserId == newAccount.UserId && c.Name == Globals.DefaultCategories.NewAccount).Id,
                    VendorId = Context.Vendors
                        .Single(v => v.UserId == newAccount.UserId && v.Name == Globals.DefaultVendors.NA).Id,
                    IsDebit = false,
                    Amount = balance,
                    Description = Globals.DefaultMessages.InitialAccountTransaction
                }
            };
            
            // Now add entities to the DB
            Add(newAccount);

            return (new AccountModel(newAccount, balance));
        }

        /// <summary>
        /// Validates changes and modifies the passed account.
        /// </summary>
        /// <param name="account">Modifies passed account. Throws AppException if validation fails.</param>
        public void ModifyAccount(Account account)
        {
            // Pull current account from DB
            Account currentAccount = Context.Accounts
                .FirstOrDefault(a => a.Id == account.Id && a.UserId == account.UserId);            
            if (currentAccount == null)
                throw new AppException("Account not found.");

            // Validate account modification. Validation errors result in thrown exceptions.
            ValidateAccount(account);
            
            // If still here, validation passed. Update properties and call update.
            //currentAccount.IsAsset = account.IsAsset; //TODO: Not currently allowing altering of IsAsset
            currentAccount.Name = account.Name;
            currentAccount.IsActive = account.IsActive;
            Update(currentAccount);
        }
        #endregion

        #region Private Helpers
        /// <summary>
        /// Calculates an account balance based on transactions in the database.
        /// </summary>
        /// <param name="accountId">Guid: AccountID of the account balance to be calculated.</param>
        /// <param name="isAsset">Bool: IsAsset classification of the account balance to be calculated.</param>
        /// <returns></returns>
        private decimal GetBalance(Guid accountId, bool isAsset)
        {
            //TODO: Further test account balances
            // TODO: Replace this whole method with a method to get balances for a list a transactions in order to prevent multiple calls to the db
            var paymentTo = Context.Transactions
                 .Where(t => t.AccountId == accountId && !t.IsDebit)
                 .Sum(t => t.Amount);

            var paymentFrom = Context.Transactions
                .Where(t => t.AccountId == accountId && t.IsDebit)
                .Sum(t => t.Amount);

            // Asset balance = payments to less payments from.
            if (isAsset)
            {
                return paymentTo - paymentFrom;
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
        private bool NameExists(Account account)
        {
            return Context.Accounts
                 .Where(a => a.UserId == account.UserId
                    && a.Name.ToLower() == account.Name.ToLower()
                    && a.Id != account.Id)
                 .Any();
        }

        /// <summary>
        /// Validates passed account model
        /// </summary>
        /// <param name="account">Account model to be validated.</param>
        private void ValidateAccount(Account account)
        {
            if (String.IsNullOrWhiteSpace(account.Name))
                throw new AppException("Account name cannot be empty or whitespace only string.");

            if (NameExists(account))
                throw new AppException($"Account name {account.Name} is already in use.");
        }
        #endregion
    }
}
