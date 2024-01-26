using System;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
namespace Assignment2.Controllers;


public class BillPayController : Controller
{
	//public IActionResult CreateBackgroundJob()
	//{
	//	BackgroundJob.Enqueue(() => Console.WriteLine("Background job Triggered"));
	//	return Ok();
	//}

    public IActionResult CreateScheduledJob()
    {
        var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
        var dateTimeOffset = new DateTimeOffset(scheduleDateTime);
        BackgroundJob.Schedule(() => Console.WriteLine("Scheduled Job Triggered"), dateTimeOffset);

        return Ok();
    }

    public IActionResult CreateRecurringJob()
    {
        RecurringJob.AddOrUpdate("RecurringJob", () => Console.WriteLine("Recurring Job Triggered"), "* * * * *");
        return Ok();
    }
}

