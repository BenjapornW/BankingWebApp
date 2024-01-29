using System;
using Microsoft.AspNetCore.Mvc;
using DataModelLibrary.Models;
using DataModelLibrary.Data;

namespace MbcaAdminWeb.Controllers
{
    public class CustomerController : Controller
    {

        private readonly HttpClient _client;

        private readonly IHttpClientFactory _clientFactory;
        private HttpClient Client => _clientFactory.CreateClient("api");

        public CustomerController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public IActionResult Index()
        {
            return View();
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
