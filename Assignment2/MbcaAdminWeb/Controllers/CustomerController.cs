using Microsoft.AspNetCore.Mvc;
using SimpleHashing.Net;
using DataModelLibrary.Models;
using DataModelLibrary.Data;

namespace MbcaAdminWeb.Controllers
{
    [Route("/Mcba/Customer")]
    public class CustomerController : Controller
    {
        private static readonly ISimpleHash s_simpleHash = new SimpleHash();

        //private readonly McbaContext _context;
        private readonly HttpClient _client;

        //public LoginController(McbaContext context)
        //{
        //    _context = context;
        //}

        public CustomerController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("api");
        }

        //public IActionResult Login() => View();

    }
}







