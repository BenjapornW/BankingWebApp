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
    private readonly Mock<McbaContext> _mockContext;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly ATMController _controller;

    public ATMControllerTests()
    {
        _mockContext = new Mock<McbaContext>();
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new ATMController(_mockContext.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext.Object
            }
        };
        // Setup mock session and other HttpContext features if necessary
    }

    [Fact]
    public async Task TransactionForm_ReturnsViewWithModel()
    {
        // Arrange
        int testAccountId = 1;
        string actionType = "Deposit";

        // Act
        var result = await _controller.TransactionForm(testAccountId, actionType);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<TransactionFormViewModel>(viewResult.Model);
        Assert.Equal(testAccountId, model.AccountNumber);
        Assert.Equal(actionType, model.ActionType);
    }


    [Fact]
    public async Task SelectAccount_ReturnsViewWithModel()
    {
        // Arrange
        string actionType = "Withdraw";

        // Mocking session to have a valid customer ID
        var session = new Mock<ISession>();
        session.Setup(s => s.GetInt32(It.IsAny<string>())).Returns(1);
        _mockHttpContext.Setup(c => c.Session).Returns(session.Object);

        // Act
        var result = await _controller.SelectAccount(actionType);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ATMViewModel>(viewResult.Model);
        Assert.Equal(actionType, model.ActionType);
    }



}
