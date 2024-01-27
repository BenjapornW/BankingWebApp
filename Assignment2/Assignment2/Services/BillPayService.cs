using System;
using Assignment2.Data;
using Assignment2.Models;
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
                var billPays = context.BillPays.ToList();
                var currentTime = DateTime.UtcNow;
                Console.WriteLine("Bill paid test");
                // find the bill that need to pay
                foreach (var bill in billPays)
                {
                    var scheduledTime = bill.ScheduleTimeUtc;
                    if (bill.Period == PeriodType.OneOff)
                    {
                        // process the on-time bills and missing bills
                        if (bill.ScheduleTimeUtc <= currentTime)
                        {
                            try
                            {
                                // find that account
                                decimal amount = bill.Amount;
                                var account = await context.Accounts.FindAsync(bill.AccountNumber);
                                if ((account.AccountType == AccountType.Saving && account.Balance - amount >= 0)
                                    || (account.AccountType == AccountType.Checking && account.Balance - amount >= 300))
                                {
                                    account.Balance -= amount;
                                    account.Transactions.Add(new Transaction
                                    {
                                        TransactionType = TransactionType.BillPay,
                                        Amount = amount,
                                        TransactionTimeUtc = DateTime.UtcNow
                                    });
                                    billPays.Remove(bill);
                                    Console.WriteLine($"Bill {bill.BillPayID} ({bill.Period.ToString()}) has been paid");
                                }
                                else
                                {
                                    bill.Status = StatusType.InsufficientBalance;
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
                        
              
                    } else
                    {
                        if (scheduledTime.Day == currentTime.Day && scheduledTime.TimeOfDay == currentTime.TimeOfDay)
                        {
                            try
                            {
                                // find that account
                                decimal amount = bill.Amount;
                                var account = await context.Accounts.FindAsync(bill.AccountNumber);
                                if ((account.AccountType == AccountType.Saving && account.Balance - amount >= 0)
                                    || (account.AccountType == AccountType.Checking && account.Balance - amount >= 300))
                                {
                                    account.Balance -= amount;
                                    account.Transactions.Add(new Transaction
                                    {
                                        TransactionType = TransactionType.BillPay,
                                        Amount = amount,
                                        TransactionTimeUtc = DateTime.UtcNow
                                    });
                
                                    Console.WriteLine($"Bill {bill.BillPayID} ({bill.Period.ToString()}) has been paid");
                                }
                                else
                                {
                                    bill.Status = StatusType.InsufficientBalance;
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
        }

    }
}

