using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Assignment2.Controllers;
using DataModelLibrary.Data;
using DataModelLibrary.Models;

namespace Assignment2.Test
{
    public class BillPayControllerTests
    {
        private readonly Mock<DbSet<BillPay>> _mockSet;
        private readonly Mock<McbaContext> _mockContext;
        private readonly BillPayController _controller;

        public BillPayControllerTests()
        {
            _mockSet = new Mock<DbSet<BillPay>>();
            _mockContext = new Mock<McbaContext>();
            _mockContext.Setup(m => m.BillPays).Returns(_mockSet.Object);

            _controller = new BillPayController(_mockContext.Object);
        }


        //[Fact]
        //public void Create_ReturnsViewWithModel()
        //{
        //    // Act
        //    var result = _controller.Create();

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.IsAssignableFrom<BillPay>(viewResult.ViewData.Model);
        //}

        //[Fact]
        //public void SubmitBill_ValidBillPay_RedirectsToSuccessMessage()
        //{
        //    // Arrange
        //    var billPay = new BillPay
        //    {
        //        // Populate with valid data
        //    };

        //    // Act
        //    var result = _controller.SubmitBill(billPay).Result;

        //    // Assert
        //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Message", redirectResult.ActionName);
        //    // Additional assertions as needed
        //}

        //[Fact]
        //public void CancelBill_ExistingBillPay_RedirectsToBillPaySummary()
        //{
        //    // Arrange
        //    var billPayId = 1; // Assuming this ID exists
        //    _mockSet.Setup(m => m.FindAsync(billPayId))
        //        .ReturnsAsync(new BillPay { BillPayID = billPayId });

        //    // Act
        //    var result = _controller.CancelBill(billPayId).Result;

        //    // Assert
        //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal(nameof(BillPayController.BillPaySummary), redirectResult.ActionName);
        //}

        //[Fact]
        //public void ConfirmBill_ValidModel_ReturnsViewWithModel()
        //{
        //    // Arrange
        //    var billPay = new BillPay
        //    {
        //        // Populate with valid data
        //    };

        //    // Act
        //    var result = _controller.ConfirmBill(billPay).Result;

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    Assert.IsAssignableFrom<BillPay>(viewResult.ViewData.Model);
        //}

        //// Additional tests for RetryBill, BillPaySummary, etc.

        //private void SetupBillPays(List<BillPay> billPays)
        //{
        //    var mockSet = new Mock<DbSet<BillPay>>();
        //    var queryable = billPays.AsQueryable();

        //    mockSet.As<IQueryable<BillPay>>().Setup(m => m.Provider).Returns(queryable.Provider);
        //    mockSet.As<IQueryable<BillPay>>().Setup(m => m.Expression).Returns(queryable.Expression);
        //    mockSet.As<IQueryable<BillPay>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        //    mockSet.As<IQueryable<BillPay>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        //    _mockContext.Setup(c => c.BillPays).Returns(mockSet.Object);
        //}
    }
}
