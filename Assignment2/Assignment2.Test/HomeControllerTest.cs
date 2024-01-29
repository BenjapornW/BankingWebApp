using System;
using Microsoft.AspNetCore.Mvc;
using Assignment2.Controllers;
using Assignment2.ViewModels;
using Xunit;
using Microsoft.Extensions.Logging;
using Moq;

namespace Assignment2.Test;

public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _mockLogger;

    public HomeControllerTests()
    {
        _mockLogger = new Mock<ILogger<HomeController>>();
    }

    [Fact]
    public void Index_ReturnsAViewResult()
    {
        // Arrange
        var controller = new HomeController(_mockLogger.Object);

        // Act
        var result = controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Privacy_ReturnsAViewResult()
    {
        // Arrange
        var controller = new HomeController(_mockLogger.Object);

        // Act
        var result = controller.Privacy();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Error_ReturnsAViewResult_WithAViewModel()
    {
        // Arrange
        var controller = new HomeController(_mockLogger.Object);

        // Act
        var result = controller.Error();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);
        Assert.NotNull(model.RequestId);
    }
}
