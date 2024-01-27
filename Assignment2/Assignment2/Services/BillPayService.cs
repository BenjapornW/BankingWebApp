using System;
using DataModelLibrary.Data;
using DataModelLibrary.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Assignment2.Services
{
    public class BillPayService
    {
        private readonly IServiceProvider _serviceProvider;

        // ReSharper disable once PossibleInvalidOperationException

        public BillPayService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task PayScheduledBills()
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<McbaContext>())
            {
                var billPays = context.BillPays.Where(x => x.Status != StatusType.Paid).ToList();
                var currentTime = DateTime.UtcNow;
                Console.WriteLine("Bill paid test");
                // find the bill that need to pay
                foreach (var bill in billPays)
                {
                    var scheduledTime = bill.ScheduleTimeUtc;
                    var periodType = bill.Period;
                    // process bills and missing bills
                    if ((periodType == PeriodType.OneOff && bill.ScheduleTimeUtc <= currentTime) ||
                        (periodType == PeriodType.Monthly &&
                        (scheduledTime.Day < currentTime.Day || (scheduledTime.Day == currentTime.Day && scheduledTime.TimeOfDay <= currentTime.TimeOfDay))))
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
                                if (periodType == PeriodType.OneOff)
                                    context.BillPays.Remove(bill);
                                else
                                    bill.Status = StatusType.Paid;

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
        }

        public async Task UpdateMonthlyBillStatus()
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<McbaContext>())
            {
                Console.WriteLine("Update Monthly paid test");
                var billPays = context.BillPays.Where(bill => bill.Status == StatusType.Paid).ToList();
                if (billPays.Count != 0)
                {
                    foreach (var bill in billPays)
                    {
                        bill.Status = StatusType.Scheduled;
                    }
                    await context.SaveChangesAsync();
                }
            }
        }

    }
}

