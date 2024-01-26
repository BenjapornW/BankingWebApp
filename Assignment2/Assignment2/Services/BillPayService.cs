using System;
using Assignment2.Data;
using Hangfire;
namespace Assignment2.Services
{
	public static class BillPayService
	{
        public static void CreateBackgroundJob()
        {
            BackgroundJob.Enqueue(() => Console.WriteLine("Background job Triggered"));
        }

        public static void CreateScheduledJob()
        {
            var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);
            BackgroundJob.Schedule(() => Console.WriteLine("Scheduled Job Triggered"), dateTimeOffset);

        }

        public static void RecurringBillPayJob(McbaContext context)
        {
            RecurringJob.AddOrUpdate("PayBills", () => PayScheduledBills(context), "* * * * *");
        }

        private static void PayScheduledBills(McbaContext context)
        {
            Console.WriteLine("Recurring Job Triggered");
        }
    }
}

