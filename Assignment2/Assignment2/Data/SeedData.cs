using System;
using DataModelLibrary.Models;
using DataModelLibrary.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Assignment2.Data
{
    public static class SeedData
    {
        public static void Initialize(McbaContext context)
        {
            //var context = serviceProvider.GetRequiredService<McbaContext>();
            SeedCustomers(context);
            SeedPayees(context);

        }


        private static void SeedCustomers(McbaContext context)
        {
            // Look for customers.
            if (context.Customers.Any())
                return; // DB has already been seeded.

            // load json
            var customers = LoadJSON();

            // Insert into database.
            foreach (var customer in customers)
            {

                foreach (var account in customer.Accounts)
                {
                    var transactions = account.Transactions;
                    foreach (var transaction in transactions)
                    {
                        account.Balance += transaction.Amount;
                        transaction.TransactionType = TransactionType.Deposit;
                    }
                }

                context.Customers.Add(customer);
            }

            context.SaveChanges();
        }



        private static List<Customer> LoadJSON()
        {

            const string Url = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";

            // Contact webservice.
            using var client = new HttpClient();
            var json = client.GetStringAsync(Url).Result;

            // Convert JSON into objects.
            var customers = JsonConvert.DeserializeObject<List<Customer>>(json, new JsonSerializerSettings
            {
                DateFormatString = "dd/MM/yyyy"
            });
            return customers;
        }

        private static void SeedPayees(McbaContext context)
        {
            if (context.Payees.Any())
                return; // DB has already been seeded.

            // Create Payees
            var payee1 = new Payee
            {
                Name = "Greater Western Water",
                Address = "123 Payee St",
                City = "Melbourne",
                State = "VIC",
                PostCode = "3000",
                Phone = "(03) 1234 5678"
            };

            var payee2 = new Payee
            {
                Name = "Optus",
                Address = "456 Payee Rd",
                City = "Sydney",
                State = "NSW",
                PostCode = "2000",
                Phone = "(02) 9876 5432"
            };
            context.Payees.AddRange(payee1, payee2);
            context.SaveChanges();
        }


    }


}