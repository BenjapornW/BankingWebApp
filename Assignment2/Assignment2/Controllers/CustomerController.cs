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

    public IActionResult Message(bool success, string message)
    {
        var viewModel = new MessageViewModel
        {
            Success = success,
            Message = message
        };

        return View("Message", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfile(Customer viewModel)
    {
        var customer = await _context.Customers.FindAsync(viewModel.CustomerID);

        if (!ModelState.IsValid)
        {
            return RedirectToAction("Message", new { success = false, message = "Your profile updated unsuccessfully!" });
        }
        customer.Name = viewModel.Name;
        if (viewModel.TFN != "")
            customer.TFN = viewModel.TFN;
        else
            customer.TFN = null;
        if (viewModel.Address != "")
            customer.Address = viewModel.Address;
        else
            customer.Address = null;
        if (viewModel.City != "")
            customer.City = viewModel.City;
        else
            customer.City = null;
        if (viewModel.State != "")
            customer.State = viewModel.State;
        else
            customer.State = null;
        if (viewModel.PostCode != "")
            customer.PostCode = viewModel.PostCode;
        else
            customer.PostCode = null;
        if (viewModel.Mobile != "")
            customer.Mobile = viewModel.Mobile;
        else
            customer.Mobile = null;

        await _context.SaveChangesAsync();

        return RedirectToAction("Message", new {success = true, message = "Your profile has been updated successfully!" });

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

            return RedirectToAction("Message", new { success = true, message = "Your password has been changed successfully!" });
        }
        return RedirectToAction("Message", new { success = false, message = "Incorrect password. Updated unsuccessfully!" });
    }
}




