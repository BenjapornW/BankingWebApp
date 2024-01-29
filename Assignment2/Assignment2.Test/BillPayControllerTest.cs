using Microsoft.EntityFrameworkCore;
using Xunit;
using Assignment2.Controllers;
using DataModelLibrary.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DataModelLibrary.Models;
using Assignment2.ViewModels;

namespace Assignment2.Test
{
    public class BillPayControllerTests
    {
        private readonly McbaContext _context;
        private readonly BillPayController _controller;

        public BillPayControllerTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<McbaContext>()
                .UseInMemoryDatabase(databaseName: "TestDb").Options;

            _context = new McbaContext(options);

            // Seed data for testing
            _context.BillPays.Add(new BillPay { BillPayID = 1 /*, other properties */ });
            _context.SaveChanges();

            _controller = new BillPayController(_context);
        }

        [Fact]
        public void Create_ReturnsViewWithModel()
        {
            // Arrange & Act
            var result = _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<BillPay>(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task BillPaySummary_ReturnsViewWithModel()
        {
            // Arrange & Act
            var result = await _controller.BillPaySummary();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<BillPaySummaryViewModel>(viewResult.ViewData.Model);
        }

    }
}
