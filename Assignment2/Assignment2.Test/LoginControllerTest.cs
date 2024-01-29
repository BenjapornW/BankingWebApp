using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Assignment2.Controllers;
using DataModelLibrary.Data;
using DataModelLibrary.Models;
using Microsoft.EntityFrameworkCore;
using SimpleHashing.Net;
using Moq;

namespace Assignment2.Test
{
    public class LoginControllerTests
    {
        private readonly McbaContext _context;
        private readonly LoginController _controller;

        public LoginControllerTests()
        {
            var options = new DbContextOptionsBuilder<McbaContext>()
            .UseInMemoryDatabase(databaseName: "TestLoginDb") // Now should work
            .Options;

            _context = new McbaContext(options);

            // Seed the database here if necessary
            _context.Logins.Add(new Login { LoginID = "user123", PasswordHash = "hashedPassword" }); // Example
            _context.SaveChanges();

            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new Mock<ISession>();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession.Object);

            _controller = new LoginController(_context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                }
            };
        }

        [Fact]
        public void Login_ReturnsViewResult()
        {
            // Act
            var result = _controller.Login();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_WithInvalidLoginID_ReturnsViewWithError()
        {
            // Arrange
            var loginID = "invalidUser";
            var password = "password";

            // Act
            var result = await _controller.Login(loginID, password);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsViewWithError()
        {
            // Arrange
            var loginID = "user123";
            var password = "wrongPassword";

            // Act
            var result = await _controller.Login(loginID, password);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }

        [Fact]
        public async Task Login_Successful_RedirectsToCustomerIndex()
        {
            // Arrange
            var loginID = "user123";
            var password = "password"; // Use the correct password that matches the hashed password

            // Act
            var result = await _controller.Login(loginID, password);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Customer", redirectResult.ControllerName);
        }

        [Fact]
        public void Logout_RedirectsToHomeIndex()
        {
            // Act
            var result = _controller.Logout();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }
    }
}
