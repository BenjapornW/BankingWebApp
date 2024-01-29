using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Assignment2.Controllers;
using Assignment2.ViewModels;
using DataModelLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace Assignment2.Test;

public class ATMControllerTests
{
    private readonly McbaContext _context;
    private readonly ATMController _controller;
    private readonly HttpContext _mockHttpContext;

    public ATMControllerTests()
    {
        var options = new DbContextOptionsBuilder<McbaContext>()
            .UseInMemoryDatabase(databaseName: "TestDb").Options;

        _context = new McbaContext(options);

        // Create a fake HttpContext with a fake session
        _mockHttpContext = new DefaultHttpContext();
        _mockHttpContext.Session = new FakeSession();
        _mockHttpContext.Session.SetInt32("CustomerID", 1); // Example of setting a session value

        _controller = new ATMController(_context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = _mockHttpContext
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

    [Fact]
    public async Task ConfirmTransaction_ReturnsViewWithModel()
    {
        // Arrange
        var testAccountId = 1; // Assuming this account is seeded in the in-memory database
        var viewModel = new TransactionFormViewModel
        {
            AccountNumber = testAccountId,
            ActionType = "Deposit",
            Amount = 100, 
            Comment = "Test transaction",
        };
        // Act
        var result = await _controller.ConfirmTransaction(viewModel);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var resultViewModel = Assert.IsType<TransactionFormViewModel>(viewResult.Model);
    }


    public class FakeSession : ISession
    {
        private readonly Dictionary<string, byte[]> _sessionStorage = new Dictionary<string, byte[]>();
        public bool IsAvailable => true;
        public string Id => Guid.NewGuid().ToString();
        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public void Clear() => _sessionStorage.Clear();

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Remove(string key) => _sessionStorage.Remove(key);

        public void Set(string key, byte[] value) => _sessionStorage[key] = value;

        public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);

        // Helper methods for int
        public void SetInt32(string key, int value)
        {
            var bytes = BitConverter.GetBytes(value);
            Set(key, bytes);
        }

        public int? GetInt32(string key)
        {
            if (TryGetValue(key, out var bytes) && bytes.Length == 4)
            {
                return BitConverter.ToInt32(bytes, 0);
            }
            return null;
        }

    }
}
