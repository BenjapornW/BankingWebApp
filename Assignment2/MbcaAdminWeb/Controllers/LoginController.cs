using Microsoft.AspNetCore.Mvc;
using AdminWebAPI.Models;


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

        public IActionResult Login()
        {
            // If the user is already logged in, redirect to the index.
            if (HttpContext.Session.GetString("JWTToken") != null)
            {
                return RedirectToAction("Index", "Home"); // Adjust the "Home" to your dashboard controller if different
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(string loginID, string password)
        {
            // Replace "User" with the appropriate model that has UserName and Password properties.
            // Since the username and password are hardcoded as "admin", "admin", you can directly check them here.
            if (loginID == "admin" && password == "admin")
            {
                var response = await _client.PostAsJsonAsync("/security/token/create", new { UserName = loginID, Password = password });

                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    // Store the token in a secure place (e.g., in session or cookie) for future requests.
                    HttpContext.Session.SetString("JWTToken", token);

                    // After successful login, redirect to a main dashboard or customer list page.
                    return RedirectToAction("Index");
                }
            }

            // If credentials are not valid, display a login error.
            ModelState.AddModelError("LoginFailed", "Invalid username or password.");
            return View();
        }


        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear the session.
            return RedirectToAction("Login");
        }



    }
}


