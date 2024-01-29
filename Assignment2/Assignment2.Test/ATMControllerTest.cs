using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Assignment2.Controllers;
using Assignment2.ViewModels;
using DataModelLibrary.Data;
using DataModelLibrary.Models;

namespace Assignment2.Test;

public class ATMControllerTests
{
    private readonly McbaContext _context;
    private readonly ATMController _controller;

    public ATMControllerTests()
    {
        var options = new DbContextOptionsBuilder<McbaContext>()
            .UseInMemoryDatabase(databaseName: "TestDb") // Use a unique name for each test method
            .Options;

        _context = new McbaContext(options);

        // Seed the in-memory database with test data if necessary

        var mockHttpContext = new Mock<HttpContext>();
        var session = new Mock<ISession>();
        session.Setup(s => s.GetInt32(It.IsAny<string>())).Returns(1); // Example of setting up a session
        mockHttpContext.Setup(c => c.Session).Returns(session.Object);

        _controller = new ATMController(_context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object
            }
        };
    }

    [Fact]
    public async Task TransactionForm_ReturnsViewWithModel()
    {
        // Arrange
        int testAccountId = 1; // Ensure this account is seeded in the in-memory database
        string actionType = "Deposit";

        // Act
        var result = await _controller.TransactionForm(testAccountId, actionType);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<TransactionFormViewModel>(viewResult.Model);
        Assert.Equal(testAccountId, model.AccountNumber);
        Assert.Equal(actionType, model.ActionType);
    }

}