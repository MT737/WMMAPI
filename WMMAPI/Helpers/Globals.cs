using System.Collections.Generic;

namespace WMMAPI.Helpers
{
    public class Globals
    {

        public struct TransactionTypes
        {
            public const string Debit = "Debit";
            public const string Credit = "Credit";
        }

        public readonly struct DefaultCategories
        {
            public const string AccountTransfer = "Account Transfer";
            public const string AccountCorrection = "Account Correction";
            public const string NewAccount = "New Account";
            public const string Income = "Income";
            public const string ATMWithdrawal = "ATM Withdrawal";
            public const string EatingOut = "Eating Out";
            public const string Entertainment = "Entertainment";
            public const string Gas = "Gas";
            public const string GroceriesSundries = "Groceries/Sundries";
            public const string Shopping = "Shopping";
            public const string ReturnsDeposits = "Returns/Deposits";
            public const string Other = "Other";

            public static string[] GetAllDefaultCategories()
            {
                return new string[] { AccountTransfer, AccountCorrection, NewAccount, Income, ATMWithdrawal,
                    EatingOut, GroceriesSundries, Shopping, ReturnsDeposits, Other };
            }

            public static string[] GetAllNotDisplayedDefaultCategories()
            {
                return new string[] { AccountTransfer, AccountCorrection, NewAccount, Income };
            }
        }
    }
}
