using System;
using Microsoft.AspNetCore.Mvc;
using DataModelLibrary.Models;
using DataModelLibrary.Data;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace MbcaAdminWeb.Controllers
{
    public class CustomerController : Controller
    {

        private readonly HttpClient _client;

        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");

        private readonly ILogger<CustomerController> _logger; // Add logger definition

        public CustomerController(IHttpClientFactory clientFactory, ILogger<CustomerController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        //public async Task<IActionResult> Index()
        //{
        //    var response = await Client.GetAsync("api/customers");
        //    //var response = await MovieApi.InitializeClient().GetAsync("api/movies");

        //    if (!response.IsSuccessStatusCode)
        //        throw new Exception();

        //    // Storing the response details received from web api.
        //    var result = await response.Content.ReadAsStringAsync();

        //    // Deserializing the response received from web api and storing into a list.
        //    var customers = JsonConvert.DeserializeObject<List<Customer>>(result);

        //    return View(customers);
        //}


        private async Task<string> GetJwtToken(string username, string password)
        {
            var client = _clientFactory.CreateClient("api");
            var response = await client.PostAsJsonAsync("/security/token/create", new { UserName = username, Password = password });

            if (response.IsSuccessStatusCode)
            {
                var jwtToken = await response.Content.ReadAsStringAsync();
                return jwtToken;
            }
            else
            {
                _logger.LogError("Failed to retrieve JWT token.");
                return null;
            }
        }

        public async Task<IActionResult> Index()
        {
            var jwtToken = await GetJwtToken("admin", "admin"); // Assuming admin/admin is correct
            if (string.IsNullOrEmpty(jwtToken))
            {
                return Unauthorized(); // Or redirect to a login page
            }

            var client = _clientFactory.CreateClient("api");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var response = await client.GetAsync("api/customers");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to retrieve customers.");
                return View("Error"); // Or any other error handling
            }

            var result = await response.Content.ReadAsStringAsync();
            var customers = JsonConvert.DeserializeObject<List<Customer>>(result);

            return View(customers);
        }


        [HttpPost]
        public async Task<IActionResult> EditCustDetail(int id, Customer customer)
        {


            var response = await _client.PutAsJsonAsync($"/api/customers/{id}", customer);


            if (response.IsSuccessStatusCode)
            {
                // Handle success
            }
            else
            {
                // Handle error
            }

            // If there's an error, you might want to return to the same edit view with an error message
            ModelState.AddModelError(string.Empty, "An error occurred while updating the customer details.");
            return View("EditCustDetail"); // Make sure you have a view named EditCustDetail or similar
        }


        [HttpPost]
        public async Task<IActionResult> LockCustomer(int id)
        {

            var response = await _client.PutAsync($"/api/customers/toggle-lock/{id}", null);


            if (response.IsSuccessStatusCode)
            {
                // Assuming you have a LockConfirmation view
                return View("LockCustomer");
            }
            else
            {
                // If there's an error, you might want to return to the same page with an error message
                ModelState.AddModelError(string.Empty, "An error occurred while attempting to lock/unlock the customer.");
                return View(); // Make sure you have a view for this action, or redirect somewhere appropriate
            }
        }




    }
}
