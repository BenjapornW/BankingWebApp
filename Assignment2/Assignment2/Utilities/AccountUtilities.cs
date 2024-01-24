using System;
using Assignment2.Models;

namespace Assignment2.Utilities
{
	public static class AccountUtilities
	{
        public static bool AccountQualifiesForFreeServiceFee(Account account)
        {
            int count = 0;
            foreach (var transaction in account.Transactions)
            {
                var transactionType = transaction.TransactionType;
                if (transactionType == TransactionType.Withdraw || transactionType == TransactionType.Transfer)
                    count++;
            }
            return count < 2;
        }
    }
}

