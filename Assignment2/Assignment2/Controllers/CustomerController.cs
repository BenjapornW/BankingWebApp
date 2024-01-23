using System;
using Assignment2.Data;
using McbaExample.Models;
using Microsoft.AspNetCore.Mvc;
using Assignment2.Utilities;
using Assignment2.Filters;
using Microsoft.EntityFrameworkCore;


namespace McbaExample.Controllers;

// Can add authorize attribute to controllers.
[AuthorizeCustomer]
public class CustomerController : Controller
{
    private readonly McbaContext _context;

    // ReSharper disable once PossibleInvalidOperationException
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public CustomerController(McbaContext context)
    {
        _context = context;
    }

    // Can add authorize attribute to actions.
    //[AuthorizeCustomer]
    public async Task<IActionResult> Index()
    {
        // Lazy loading.
        // The Customer.Accounts property will be lazy loaded upon demand.
        var customer = await _context.Customers.FindAsync(CustomerID);

        // OR
        // Eager loading.
        //var customer = await _context.Customers.Include(x => x.Accounts).
        //    FirstOrDefaultAsync(x => x.CustomerID == CustomerID);

        return View(customer);
    }

    public async Task<IActionResult> Deposit(int id) => View(await _context.Accounts.FindAsync(id));

    [HttpPost]
    public async Task<IActionResult> Deposit(int id, decimal amount)
    {
        var account = await _context.Accounts.FindAsync(id);

        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        else if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");

        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            return View(account);
        }

        // Note this code could be moved out of the controller, e.g., into the Model.
        account.Balance += amount;
        account.Transactions.Add(
            new Transaction
            {
                TransactionType = TransactionType.Deposit,
                Amount = amount,
                TransactionTimeUtc = DateTime.UtcNow
            });

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // ... Your existing using directives ...


    public async Task<IActionResult> SelectAccountForStatement()
    {
        var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));

        if (!customerID.HasValue)
            return RedirectToAction("Login", "Customer");

        var customer = await _context.Customers
                                     .Include(c => c.Accounts)
                                     .FirstOrDefaultAsync(c => c.CustomerID == customerID.Value);

        if (customer == null)
            return NotFound();

        return View(customer.Accounts);
    }

    public async Task<IActionResult> Statement(int accountNumber)
    {
        var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));

        if (!customerID.HasValue)
            return RedirectToAction("Login", "Customer");

        var account = await _context.Accounts
                                    .Where(a => a.AccountNumber == accountNumber && a.CustomerID == customerID.Value)
                                    .Include(a => a.Transactions)
                                    .FirstOrDefaultAsync();

        if (account == null)
            return NotFound();

        ViewBag.AvailableBalance = CalculateAvailableBalance(account);

        return View("Statement", account.Transactions.OrderByDescending(t => t.TransactionTimeUtc));
    }

    private decimal CalculateAvailableBalance(Account account)
    {
        const decimal minimumBalanceRequiredForChecking = 300m;
        return account.AccountType == "C" && account.Balance > minimumBalanceRequiredForChecking
               ? account.Balance - minimumBalanceRequiredForChecking
               : account.Balance;
    }


}

