﻿using System;
using Assignment2.Data;
using Assignment2.Models;
using Microsoft.AspNetCore.Mvc;
using Assignment2.Utilities;
using Assignment2.Filters;
using Microsoft.EntityFrameworkCore;


namespace Assignment2.Controllers;

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

    private decimal CalculateAvailableBalance(Account account)
    {
        const decimal minimumBalanceRequiredForChecking = 300m;
        return account.AccountType == "C" && account.Balance > minimumBalanceRequiredForChecking
               ? account.Balance - minimumBalanceRequiredForChecking
               : account.Balance;
    }


    public async Task<IActionResult> Statement(int accountNumber, int page = 1)
    {
        const int PageSize = 4;
        var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));

        if (!customerID.HasValue)
            return RedirectToAction("Login", "Customer");

        var account = await _context.Accounts
                                    .Where(a => a.AccountNumber == accountNumber && a.CustomerID == customerID.Value)
                                    .FirstOrDefaultAsync();

        if (account == null)
            return NotFound();

        var transactionsQuery = _context.Transactions
                                        .Where(t => t.AccountNumber == accountNumber)
                                        .OrderByDescending(t => t.TransactionTimeUtc);

        // Count the total number of transactions
        var totalTransactions = await transactionsQuery.CountAsync();

        // Get the page of transactions
        var transactions = await transactionsQuery.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

        ViewBag.AccountNumber = accountNumber; // Add this line in Statement action method

        ViewBag.AvailableBalance = CalculateAvailableBalance(account);
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalTransactions / PageSize);

        return View(transactions);
    }

    public async Task<IActionResult> Profile()
    {
        var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
        if (!customerID.HasValue)
        {
            return RedirectToAction("Login", "Customer");
        }

        var customer = await _context.Customers
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(c => c.CustomerID == customerID.Value);

        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }


}
