using System;
using AdminWebAPI.Models.DataManager;
using DataModelLibrary.Data;
using DataModelLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdminWebAPI.Test
{
	public class CustomerManagerTests
	{
        private readonly McbaContext _context;

        public CustomerManagerTests()
		{
            _context = new McbaContext(new DbContextOptionsBuilder<McbaContext>().
          UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared").Options);
            _context.Database.EnsureCreated();
            SeedData.Initialize(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void GetAll_ReturnsAllCustomers()
        {
            // Arrange
            var manager = new CustomerManager(_context);

            // Act
            var customers = manager.GetAll();
            var existingCustomers = _context.Customers.ToList();
            // Assert
            Assert.NotNull(customers);
            Assert.Equal(existingCustomers, customers);
        }

        [Theory]
        [InlineData(2100)]
        [InlineData(2200)]
        public void Get_ValidId_ReturnsCustomer(int id)
        {
            // Arrange
            var manager = new CustomerManager(_context);

            // Act
            var customer = manager.Get(id);

            // Assert
            Assert.NotNull(customer);
            Assert.Equal(id, customer.CustomerID);
        }

        [Theory]
        [InlineData(3300)] 
        [InlineData(1100)]
        public void Get_InvalidId_ReturnsNull(int id)
        {
            // Arrange
            var manager = new CustomerManager(_context);

            // Act
            var customer = manager.Get(id);

            // Assert
            Assert.Null(customer);
        }

        [Theory]
        [InlineData(2100, "John Doe", "123 434 443", "123 Main St", "Anytown", "VIC", "3000", "0412 345 678")]
        [InlineData(2200, "Jane Smith", "567 833 443", "456 Elm St", "Othertown", "NSW", "2000", "0413 456 789")]
        public void Update_ValidId_ReturnsUpdatedCustomerId(int idToUpdate, string name, string tfn, string address, string city, string state, string postCode, string mobile)
        {
            // Arrange
            var manager = new CustomerManager(_context);
            Customer updatedCustomer = new Customer
            {
                CustomerID = idToUpdate,
                Name = name,
                TFN = tfn,
                Address = address,
                City = city,
                State = state,
                PostCode = postCode,
                Mobile = mobile
            };

            // Act
            int result = manager.Update(idToUpdate, updatedCustomer);

            // Assert
            Assert.Equal(idToUpdate, result);
            var customerInDatabase = _context.Customers.Find(idToUpdate);
            Assert.Equal(updatedCustomer.Name, customerInDatabase.Name);
            Assert.Equal(updatedCustomer.TFN, customerInDatabase.TFN);
            Assert.Equal(updatedCustomer.Address, customerInDatabase.Address);
            Assert.Equal(updatedCustomer.City, customerInDatabase.City);
            Assert.Equal(updatedCustomer.State, customerInDatabase.State);
            Assert.Equal(updatedCustomer.PostCode, customerInDatabase.PostCode);
            Assert.Equal(updatedCustomer.Mobile, customerInDatabase.Mobile);
        }

        [Theory]
        [InlineData(4300, "John Doe", "123 434 443", "123 Main St", "Anytown", "VIC", "3000", "0412 345 678")]
        [InlineData(2800, "Jane Smith", "567 833 443", "456 Elm St", "Othertown", "NSW", "2000", "0413 456 789")]
        public void Update_InValidId_ReturnZero(int idToUpdate, string name, string tfn, string address, string city, string state, string postCode, string mobile)
        {
            // Arrange
            var manager = new CustomerManager(_context);
            Customer updatedCustomer = new Customer
            {
                CustomerID = idToUpdate,
                Name = name,
                TFN = tfn,
                Address = address,
                City = city,
                State = state,
                PostCode = postCode,
                Mobile = mobile
            };

            // Act
            int result = manager.Update(idToUpdate, updatedCustomer);

            // Assert
            Assert.Equal(0, result);
        }

        [Theory]
        [InlineData(2100)]
        [InlineData(2200)]
        public void ToggleLockCustomer_ValidId_LocksOrUnlocksCustomer(int customerId)
        {
            // Arrange
            var manager = new CustomerManager(_context);
            var customer = _context.Customers.Find(customerId);
            bool initialLockState = customer.Locked;

            // Act
            int result = manager.ToggleLockCustomer(customerId);

            // Assert
            Assert.Equal(customerId, result);
            Assert.NotEqual(initialLockState, customer.Locked);
        }

        [Theory]
        [InlineData(5500)]
        [InlineData(2400)]
        public void ToggleLockCustomer_InValidId_ReturnZero(int customerId)
        {
            // Arrange
            var manager = new CustomerManager(_context);

            // Act
            int result = manager.ToggleLockCustomer(customerId);

            // Assert
            Assert.Equal(0, result);
        }
    }


}

