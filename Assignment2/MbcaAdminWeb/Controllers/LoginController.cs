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
                return RedirectToAction("Index", "Customer"); // Replace "Dashboard" with your dashboard controller's name
            }

            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            // Admin credentials check
            if (user.UserName == UserLogin.UserName && user.Password == UserLogin.Password)
            {
                // Set session to keep admin logged in
                HttpContext.Session.SetString("AdminLoggedIn", "true");

                // Redirect to the admin dashboard or the modify customer page
                return RedirectToAction("Index", "Customer"); 
            }
            else
            {
                ModelState.AddModelError("LoginFailed", "Incorrect username or password");
                return View("Login");
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

