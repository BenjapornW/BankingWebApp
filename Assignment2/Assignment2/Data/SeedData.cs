using System;
using McbaExample.Models;
using Newtonsoft.Json;
using System.Transactions;

namespace Assignment2.Data
{
	public static class SeedData
	{
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<McbaContext>();

            // Look for customers.
            if (context.Customers.Any())
                return; // DB has already been seeded.
        }
    }

    public static async List<Customer> GetCustomer()
    {
        // Check if any people already exist and if they do, stop.
        const string Url = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";

        // Contact webservice.
        using var client = new HttpClient();
        var json = await client.GetStringAsync(Url);

        // Convert JSON into objects.
        var customers = JsonConvert.DeserializeObject<List<Customer>>(json, new JsonSerializerSettings
        {
            DateFormatString = "dd/MM/yyyy"
        });
        return customers;
    }

