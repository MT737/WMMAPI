using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Models.AccountModels;
using static WMMAPI.Helpers.Globals;

namespace WMMAPI.Services
{
    public class AccountService : BaseService<Account>, IAccountService
    {
        public AccountService(WMMContext context) : base(context)
        {
        }

        /// <summary>
        /// Retrieves the record of the requested account. 
        /// </summary>
        /// <param name="id">Guid: the AccountID of the account to be retrieved.</param>
        /// <param name="userId">Guid: the UserID of which account to be retrieved.</param>
        /// <returns>Account model for the requested account.</returns>
        public AccountModel Get(Guid id, Guid userId)
        {   
            var account = Context.Accounts
                .Where(a => a.AccountId == id && a.UserId == userId)
                .SingleOrDefault();

            if (account == null)
                throw new AppException("Account not found.");

            var model = new AccountModel(account);
            model.Balance = GetBalance(model.AccountId, model.IsAsset);
            return model;
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
                .Select(x => new AccountModel(x)).ToList();

            // Get balance
            foreach(var account in accounts)
            {
                account.Balance = GetBalance(account.AccountId, account.IsAsset);
            }

            return accounts.ToList();
        }

        /// <summary>
        /// Validates and adds account to the database.
        /// </summary>
        /// <param name="newAccount">Account to add to the database. Throws AppException if validation fails.</param>
        public void AddAccount(Account newAccount)
        {
            // Validate account. Validation errors result in thrown exceptions.
            ValidateAccount(newAccount);
                        
            //If still here, validation passed. Add account.
            Add(newAccount);

            //TODO: If no error, add transaction to set balance
        }

        /// <summary>
        /// Validates changes and modifies the passed account.
        /// </summary>
        /// <param name="account">Modifies passed account. Throws AppException if validation fails.</param>
        public void ModifyAccount(Account account)
        {
            // Pull current account from DB
            Account currentAccount = Context.Accounts
                .FirstOrDefault(a => a.AccountId == account.AccountId && a.UserId == account.UserId);            
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

        #region Helpers
        /// <summary>
        /// Calculates an account balance based on transactions in the database.
        /// </summary>
        /// <param name="accountId">Guid: AccountID of the account balance to be calculated.</param>
        /// <param name="isAsset">Bool: IsAsset classification of the account balance to be calculated.</param>
        /// <returns></returns>
        public decimal GetBalance(Guid accountId, bool isAsset)
        {
            //TODO: Further test account balances
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
        public bool NameExists(Account account)
        {
            return Context.Accounts
                 .Where(a => a.UserId == account.UserId
                    && a.Name.ToLower() == account.Name.ToLower()
                    && a.AccountId != account.AccountId)
                 .Any();
        }

        /// <summary>
        /// Validates passed account model
        /// </summary>
        /// <param name="account">Account model to be validated.</param>
        public void ValidateAccount(Account account)
        {
            if (String.IsNullOrWhiteSpace(account.Name))
                throw new AppException("Account name cannot be empty or whitespace only string.");

            if (NameExists(account))
                throw new AppException($"Account name {account.Name} is already in use.");
        }
        #endregion
    }
}
