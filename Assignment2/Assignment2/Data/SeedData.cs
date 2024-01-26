using System;
using Assignment2.Models;
using Newtonsoft.Json;

namespace Assignment2.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            
            try
            {
                var context = serviceProvider.GetRequiredService<McbaContext>();

                // Look for customers.

                // Look for customers.
                if (context.Customers.Any())
                    return; // DB has already been seeded.

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

                // Insert Payees into the context
                context.Payees.AddRange(payee1, payee2);

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
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
            }
            

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


    }

    
}

