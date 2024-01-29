using System;
using System.ComponentModel.DataAnnotations;
using AdminWebAPI.Controllers;
using AdminWebAPI.Models;
using AdminWebAPI.Models.DataManager;
using DataModelLibrary.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using DataModelLibrary.Models;
using Moq;

namespace AdminWebAPI.Test;

public class CustomersControllerTests 
{
    private readonly CustomerManager _repo;
    private readonly McbaContext _context;

    public CustomersControllerTests()
	{
        _context = new McbaContext(new DbContextOptionsBuilder<McbaContext>().
           UseSqlite($"Data Source=file:{Guid.NewGuid()}?mode=memory&cache=shared").Options);
        _context.Database.EnsureCreated();
        SeedData.Initialize(_context);
        _repo = new CustomerManager(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Get_ReturnsOkObjectResult()
    {
        // Arrange
        var controller = new CustomersController(_repo);

        // Act
        var result = controller.Get();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Theory]
    [InlineData(2100)]
    [InlineData(2200)]
    [InlineData(2300)]
    public void Get_WithValidId_ReturnsOkObjectResult(int customerId)
    {
        // Arrange
        var controller = new CustomersController(_repo);

        // Act
        var result = controller.Get(customerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var customer = Assert.IsType<Customer>(okResult.Value);
        Assert.Equal(customerId, customer.CustomerID);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(200)]
    [InlineData(null)]
    public void Get_WithInvalidId_ReturnsNotFound(int invalidCustomerId)
    {
        // Arrange
        var controller = new CustomersController(_repo);

        // Act
        var result = controller.Get(invalidCustomerId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Theory]
    [InlineData(2100, "John Doe", "123 434 443", "123 Main St", "Anytown", "VIC", "3000", "0412 345 678")]
    [InlineData(2200, "Jane Smith", "567 833 443", "456 Elm St", "Othertown", "NSW", "2000", "0413 456 789")]
    public void Put_WithValidIdAndMatchingCustomer_ReturnsOkResult(int id, string name, string tfn, string address, string city, string state, string postCode, string mobile)
    {
        // Arrange
        var controller = new CustomersController(_repo);
        var existingCustomer = new Customer
        {
            CustomerID = id,
            Name = name,
            TFN = tfn,
            Address = address,
            City = city,
            State = state,
            PostCode = postCode,
            Mobile = mobile
        };

        // Act
        var result = controller.Put(id, existingCustomer);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Theory]
    [InlineData(2100)]
    [InlineData(2200)]
    public void ToggleLock_WithValidId_ReturnsOkResult(int id)
    {
        // Arrange
        var controller = new CustomersController(_repo);

        // Act
        var result = controller.ToggleLock(id);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public void ToggleLock_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var controller = new CustomersController(_repo);
        var invalidCustomerId = 100; // Assuming there's no customer with ID 100 in the database

        // Act
        var result = controller.ToggleLock(invalidCustomerId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }


}

