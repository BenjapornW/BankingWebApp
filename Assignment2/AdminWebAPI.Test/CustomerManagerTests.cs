using System;
using AdminWebAPI.Models.DataManager;
using DataModelLibrary.Data;
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

    }
}

