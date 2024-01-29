using Microsoft.AspNetCore.Mvc;
using SimpleHashing.Net;
using DataModelLibrary.Models;
using DataModelLibrary.Data;
namespace MbcaAdminWeb.Controllers
{
    [Route("/Mcba/SecureLogin")]
    public class LoginController : Controller
    {
        private readonly HttpClient _client;

        public LoginController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("api");
        }


        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to the dashboard
            if (HttpContext.Session.GetString("AdminLoggedIn") == "true")
            {
                return RedirectToAction("Index", "Dashboard"); // Replace "Dashboard" with your dashboard controller's name
            }

            return View();
        }

        [HttpPost]
        public IActionResult Login(string loginID, string password)
        {
            // Admin credentials check
            if (loginID == "admin" && password == "admin")
            {
                // Set session to keep admin logged in
                HttpContext.Session.SetString("AdminLoggedIn", "true");

                // Redirect to the admin dashboard or the modify customer page
                return RedirectToAction("Index", "Dashboard"); // Replace "Dashboard" with your dashboard controller's name
            }
            else
            {
                ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
                return View();
            }
        }

        [Route("/Mcba/SecureLogout")]
        public IActionResult Logout()
        {
            // Clear the admin session
            HttpContext.Session.Remove("AdminLoggedIn");

            // Redirect to the login page
            return RedirectToAction("Login");
        }



    }
}

