using Microsoft.AspNetCore.Mvc;
using SimpleHashing.Net;
using DataModelLibrary.Models;
using DataModelLibrary.Data;
using System.Net.Http.Headers;

namespace MbcaAdminWeb.Controllers
{
    [Route("/Mcba/Customer")]
    public class CustomerController : Controller
    {

        private readonly HttpClient _client;

        public CustomerController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("api");
        }


        private async Task<bool> SetBearerToken()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return true;
            }
            return false;
        }


        [HttpPost]
        public async Task<IActionResult> EditCustDetail(int id, Customer customer)
        {
            // Assuming you have the JWT token stored in a session or cookie
            string jwtToken = HttpContext.Session.GetString("JWTToken");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            if (!await SetBearerToken())
            {
                return Unauthorized(); // If there's no token, return an unauthorized response.
            }

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
            // Assuming you have the JWT token stored in a session or cookie
            string jwtToken = HttpContext.Session.GetString("JWTToken");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

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







