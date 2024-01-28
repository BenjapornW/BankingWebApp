using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AdminWebAPI.Models.DataManager;
using DataModelLibrary.Models;
using Microsoft.AspNetCore.Authorization;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AdminWebAPI.Controllers
{
    // pass the authentication token as header when calling from api/customers
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private readonly CustomerManager _repo;

        public CustomersController(CustomerManager repo)
        {
            _repo = repo;
        }

        // HTTP Method: GET
        // Url: api/customers
        // Can fetch all the customers in the database
        [HttpGet]
        [Authorize]
        public IEnumerable<Customer> Get()
        {
            return _repo.GetAll();
        }

        // HTTP Method: GET
        // Url: api/customers/{id}
        // Find a customer with an valid customer ID
        [HttpGet("{id}")]
        [Authorize]
        public Customer Get(int id)
        {
            return _repo.Get(id);
        }

        // HTTP Method: POST
        // Url: api/customers
        // Insert a new customer into database
        [HttpPost]
        [Authorize]
        public void Post([FromBody] Customer customer)
        {
            _repo.Add(customer);
        }

        // HTTP Method: PUT
        // Url: api/customers/{id}
        // Update a customer's profile by customer ID
        [HttpPut("{id}")]
        [Authorize]
        public void Put(int id, [FromBody] Customer customer)
        {
            _repo.Update(id, customer);
        }

        // HTTP Method: PUT
        // Url: api/customers/{id}
        // Toggle to lock or unlock a customer by customer ID
        [HttpPut("toggle-lock/{id}")]
        [Authorize]
        public void ToggleLock(int id)
        {
            _repo.ToggleLockCustomer(id);
        }

        // HTTP Method:DELETE
        // Url: api/customers/{id}
        // Delete a customer by customer ID
        [HttpDelete("{id}")]
        [Authorize]
        public int Delete(int id)
        {
            return _repo.Delete(id);
        }
    }
}

