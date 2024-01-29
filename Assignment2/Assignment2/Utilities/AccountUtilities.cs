using System;
using DataModelLibrary.Data;
using DataModelLibrary.Models;
using Microsoft.EntityFrameworkCore;

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

        public static async Task ChargeBill(BillPay bill, McbaContext context)
        {
            try
            {
                // find that account
                decimal amount = bill.Amount;
                var account = await context.Accounts.FindAsync(bill.AccountNumber);
                if ((account.AccountType == AccountType.Saving && account.Balance - amount >= 0)
                    || (account.AccountType == AccountType.Checking && account.Balance - amount >= 300))
                {
                    // pay bill
                    account.Balance -= amount;
                    account.Transactions.Add(new Transaction
                    {
                        TransactionType = TransactionType.BillPay,
                        Amount = amount,
                        TransactionTimeUtc = DateTime.UtcNow
                    });

                    // update status
                    if (bill.Period == PeriodType.OneOff)
                        context.BillPays.Remove(bill);
                    else
                        bill.ScheduleTimeUtc = bill.ScheduleTimeUtc.AddMonths(1); // add 1 month after paid

                    Console.WriteLine($"Bill {bill.BillPayID} ({bill.Period.ToString()}) has been paid");
                }
                else
                {

                    bill.Status = StatusType.InsufficientBalance;
                    Console.WriteLine($"Bill {bill.BillPayID} ({bill.Period.ToString()}) fail (InsufficientBalance)");
                }
            }
            catch (Exception ex)
            {
                bill.Status = StatusType.Fail;
                Console.WriteLine($"Bill {bill.BillPayID} ({bill.Period.ToString()}) fails : {ex.Message}");
            }
            finally
            {
                await context.SaveChangesAsync();
            }
        }
    }
}

