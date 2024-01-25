using System;
using Assignment2.Data;
using Assignment2.Models;
using Microsoft.AspNetCore.Mvc;
using Assignment2.Utilities;
using Assignment2.Filters;
using Microsoft.EntityFrameworkCore;
using Assignment2.ViewModels;
using SimpleHashing.Net;
using Microsoft.AspNetCore.Identity;


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

    [HttpPost]
    public async Task<IActionResult> UpdateProfile(string name, string TFN,
        string address, string city, string state, string postCode, string mobile)
    {
        var customer = await _context.Customers.FindAsync(CustomerID);

        if (!ModelState.IsValid)
        {
            return RedirectToAction("Profile");
        }
        customer.Name = name;
        if (TFN != "")
            customer.TFN = TFN;
        if (address != "")
            customer.Address = address;
        if (city != "")
            customer.City = city;
        if (state != "")
            customer.State = state;
        if (postCode != "")
            customer.PostCode = postCode;
        if (mobile != "")
            customer.Mobile = mobile;

        await _context.SaveChangesAsync();
        var viewModel = new MessageViewModel
        {
            Success = true,
            Message = "Your profile has been updated successfully!"
        };
        return View("Message", viewModel);

    }

    [HttpPost]
    public async Task<IActionResult> UpdatePassword(string oldPassword, string newPassword)
    {
        var login = await _context.Logins.FirstOrDefaultAsync(x => x.CustomerID == CustomerID);
        ISimpleHash simpleHash = new SimpleHash();
        if (simpleHash.Verify(oldPassword, login.PasswordHash))
        {
            login.PasswordHash = simpleHash.Compute(newPassword);
            await _context.SaveChangesAsync();
            var viewModel = new MessageViewModel
            {
                Success = true,
                Message = "Your password has been changed successfully!"
            };
            return View("Message", viewModel);
        }

        return View("Message", new MessageViewModel
        {
            Success = false,
            Message = "Incorrect password. Updated unsuccessfully!"
        });
    }



}
