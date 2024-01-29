using System;
using DataModelLibrary.Data;
using DataModelLibrary.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Assignment2.Utilities;

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
                var currentTime = DateTime.Now;
                Console.WriteLine("Bill paid test");
                // find the bill that need to pay
                foreach (var bill in billPays)
                {
                    var scheduledTime = bill.ScheduleTimeUtc;
                    Console.WriteLine($"Current time {currentTime}, scheduled time {scheduledTime}");
                    // process bills and missing bills
                    if (scheduledTime <= currentTime)
                    {   
                        await AccountUtilities.ChargeBill(bill, context);
                    }
                    Console.WriteLine("Bill paid test finish");


                }
            }
        }

        //public async Task UpdateMonthlyBillStatus()
        //{
        //    using (IServiceScope scope = _serviceProvider.CreateScope())
        //    using (var context = scope.ServiceProvider.GetRequiredService<McbaContext>())
        //    {
        //        Console.WriteLine("Update Monthly paid test");
        //        var billPays = context.BillPays.Where(bill => bill.Status == StatusType.Paid).ToList();
        //        if (billPays.Count != 0)
        //        {
        //            foreach (var bill in billPays)
        //            {
        //                bill.Status = StatusType.Scheduled;
        //            }
        //            await context.SaveChangesAsync();
        //        }
        //    }
        //}

    }
}

