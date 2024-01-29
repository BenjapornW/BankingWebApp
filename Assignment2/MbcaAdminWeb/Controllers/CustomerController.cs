using System;
using Microsoft.AspNetCore.Mvc;
using DataModelLibrary.Models;
using DataModelLibrary.Data;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;

namespace MbcaAdminWeb.Controllers
{
    public class CustomerController : Controller
    {

        private readonly HttpClient _client;

        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");
        //private HttpClient TokenClient => _clientFactory.CreateClient("security");

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
            var user = new User { UserName = username, Password = password };

            try
            {
                var response = await Client.PostAsJsonAsync("security/Token/create", user);

                if (response.IsSuccessStatusCode)
                {
                    var jwtToken = await response.Content.ReadAsStringAsync();
                    return jwtToken;
                }
                else
                {
                    _logger.LogError("Failed to retrieve JWT token. Status Code: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving JWT token.");
                return null;
            }
        }


        public async Task<IActionResult> Index()
        {
            //var jwtToken = await GetJwtToken(UserLogin.UserName, UserLogin.Password); // Use actual admin credentials
            var jwtToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJJZCI6ImE4ZDE3ZDA2LTg5MTEtNDZmZC05NGY5LTY2ODNlOTk2MmYzMSIsInN1YiI6ImFkbWluIiwiZW1haWwiOiJhZG1pbiIsImp0aSI6ImNmOGRjMTg2LTRkMTItNGUxMC1hZTk3LWFiMjQ3ZTM4Y2EyMiIsIm5iZiI6MTcwNjUyMjYyMiwiZXhwIjoxNzA2NTIyOTIyLCJpYXQiOjE3MDY1MjI2MjIsImlzcyI6Ikdyb3VwOSIsImF1ZCI6ImxvY2FsaG9zdDo1MDA3In0.HxjBxXg_fqakD0HGkwYZf1SAxSIPbH7n9k9IaCB9uqOs0hX1bajt3iMnQppvL3VsNcuistd3uWBqOCllP-jKaw";
            if (string.IsNullOrEmpty(jwtToken))
            {
                return Unauthorized(); // Or redirect to a login page
            }

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            try
            {
                var response = await Client.GetAsync("api/customers");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to retrieve customers. Status Code: {StatusCode}", response.StatusCode);
                    return View("Error"); // Or any other error handling
                }

                var result = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(result);

                return View(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving customers.");
                return View("Error"); // Or any other error handling
            }
        }


        [HttpPost]
        public async Task<IActionResult> EditCustDetail(int id, Customer customer)
        {


            var response = await Client.PutAsJsonAsync($"/api/customers/{id}", customer);


            if (!response.IsSuccessStatusCode)
            {

                // If there's an error, you might want to return to the same edit view with an error message
                ModelState.AddModelError(string.Empty, "An error occurred while updating the customer details.");
                return View("EditCustDetail"); // Make sure you have a view named EditCustDetail or similar
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> LockCustomer(int id)
        {

            var response = await Client.PutAsync($"/api/customers/toggle-lock/{id}", null);


            if (response.IsSuccessStatusCode)
            {
                // Assuming you have a LockConfirmation view
                return RedirectToAction("Index");
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