using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModelLibrary.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace McbaAdminWebAPI.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private readonly CustomerManager _repo;

        public CustomersController(CustomerManager repo)
        {
            _repo = repo;
        }

        // GET: api/customers
        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return _repo.GetAll();
        }

        // GET api/customers/5
        [HttpGet("{id}")]
        public Customer Get(int id)
        {
            return _repo.Get(id);
        }

        // POST api/customers
        [HttpPost]
        public void Post([FromBody] Customer customer)
        {
            _repo.Add(customer);
        }

        // PUT api/customers/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Customer customer)
        {
            _repo.Update(id, customer);
        }

        // PUT api/customers/5
        [HttpPut("toggle-lock/{id}")]
        public void ToggleLock(int id)
        {
            _repo.ToggleLockCustomer(id);
        }

        // DELETE api/customers/5
        [HttpDelete("{id}")]
        public int Delete(int id)
        {
            return _repo.Delete(id);
        }
    }
}

