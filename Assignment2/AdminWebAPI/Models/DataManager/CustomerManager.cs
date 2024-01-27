using System;
using AdminWebAPI.Models.Repository;
using DataModelLibrary.Models;
using DataModelLibrary.Data;

namespace AdminWebAPI.Models.DataManager
{
	public class CustomerManager : IDataRepository<Customer, int>
    {
        private readonly McbaContext _context;

        public CustomerManager(McbaContext context)
		{
			_context = context;
		}

        public Customer Get(int id)
        {
        
            return _context.Customers.Find(id);
        }

        public IEnumerable<Customer> GetAll()
        {
            //Console.WriteLine(_context.Customers.ToList());
            return _context.Customers.ToList();
        }

        public int Add(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();

            return customer.CustomerID;
        }

        public int Delete(int id)
        {
            _context.Customers.Remove(_context.Customers.Find(id));
            _context.SaveChanges();

            return id;
        }

        public int Update(int id, Customer customer)
        {
            _context.Update(customer);
            _context.SaveChanges();

            return id;
        }

        public int ToggleLockCustomer(int id)
        {
            var customer = _context.Customers.Find(id);
            //var login = _context.Logins.First(x => x.CustomerID == id);
            if (customer != null)
                if (!customer.Locked)
                    customer.Locked = true;
                else
                    customer.Locked = false;
            _context.SaveChanges();
            return id;
        }


    }
}

