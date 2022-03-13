using System.Collections.Generic;

namespace WMMAPI.Helpers
{
    public class Globals
    {
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
                    EatingOut, Entertainment, GroceriesSundries, Shopping, ReturnsDeposits, Other };
            }

            public static string[] GetAllNotDisplayedDefaultCategories()
            {
                return new string[] { AccountTransfer, AccountCorrection, NewAccount, Income };
            }
        }

        public readonly struct DefaultVendors
        {
            public const string NA = "N/A";
            public const string Amazon = "Amazon";
            public const string TMobile = "T-Mobile";
            public const string LibertyMutual = "Liberty Mutual";

            public static string[] GetAllDevaultVendors()
            {
                return new string[] { NA, Amazon, TMobile, LibertyMutual };
            }

            public static string[] GetAllNotDisplayedDefaultVendors()
            {
                return new string[] { NA };
            }
        }
    }
}
