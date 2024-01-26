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

        public void PayScheduledBills()
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<McbaContext>())
            {
                var logins = context.Logins.ToList();

                foreach (var login in logins)
                {
                    Console.WriteLine(login.LoginID);
                }
            }
        }
    }
}

