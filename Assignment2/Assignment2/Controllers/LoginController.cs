using System;
using Microsoft.AspNetCore.Mvc;
using Assignment2.Data;
using SimpleHashing.Net;
using McbaExample.Models;

namespace Assignment2.Controllers
{
    [Route("/Mcba/SecureLogin")]
    public class LoginController : Controller
    {
        private static readonly ISimpleHash s_simpleHash = new SimpleHash();

        private readonly McbaContext _context;

        public LoginController(McbaContext context)
        {
            _context = context;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string loginID, string password)
        {
            var login = await _context.Logins.FindAsync(loginID);
            if (login == null || string.IsNullOrEmpty(password) || !s_simpleHash.Verify(password, login.PasswordHash))
            {
                ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
                return View(new Login { LoginID = loginID });
            }

            // Login customer.
            HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
            HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);

            return RedirectToAction("Index", "Customer");
        }

        [Route("LogoutNow")]
        public IActionResult Logout()
        {
            // Logout customer.
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}
