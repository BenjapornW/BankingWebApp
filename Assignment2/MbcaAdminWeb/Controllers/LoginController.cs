using Microsoft.AspNetCore.Mvc;
using SimpleHashing.Net;
using DataModelLibrary.Models;
using DataModelLibrary.Data;

namespace MbcaAdminWeb.Controllers
{
    [Route("/Mcba/SecureLogin")]
    public class LoginController : Controller
    {
        private static readonly ISimpleHash s_simpleHash = new SimpleHash();

        //private readonly McbaContext _context;
        private readonly HttpClient _client;

        //public LoginController(McbaContext context)
        //{
        //    _context = context;
        //}

        public LoginController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("api");
        }

        public IActionResult Login() => View();

        //[HttpPost]
        //public async Task<IActionResult> Login(string loginID, string password)
        //{
        //    var login = await _context.Logins.FindAsync(loginID);
        //    //if (login == null || string.IsNullOrEmpty(password) || !s_simpleHash.Verify(password, login.PasswordHash))
        //    //{
        //    //    ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
        //    //    return View(new Login { LoginID = loginID });
        //    //}
        //    // Check if loginID is invalid.

        //    if (login == null)
        //    {
        //        ModelState.AddModelError("LoginFailed", "Login failed, invalid loginID.");
        //        return View();
        //    }
        //    //var customer = await _context.Customers.FindAsync(login.)
        //    if (login.Customer.Locked)
        //    {
        //        ModelState.AddModelError("LoginFailed", "Your account has been locked by admin");
        //        return View();
        //    }

        //    // Check if password is invalid.
        //    if (string.IsNullOrEmpty(password) || !s_simpleHash.Verify(password, login.PasswordHash))
        //    {
        //        ModelState.AddModelError("LoginFailed", "Login failed, invalid Password.");
        //        return View(new Login { LoginID = loginID });
        //    }

        //    // Login customer.
        //    HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
        //    HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);

        //    return RedirectToAction("Index", "Customer");
        //}

        //[Route("LogoutNow")]
        //public IActionResult Logout()
        //{
        //    // Logout customer.
        //    HttpContext.Session.Clear();

        //    return RedirectToAction("Index", "Home");
        //}
    }
}





