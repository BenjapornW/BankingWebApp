using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment2.Data;
using Assignment2.Models;
using Assignment2.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment2.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Assignment2.Controllers
{
    public class ATMController : Controller
    {
        private readonly McbaContext _context;

        // ReSharper disable once PossibleInvalidOperationException
        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

        public ATMController(McbaContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Deposit(int id) => View(await _context.Accounts.FindAsync(id));

        [HttpPost]
        public async Task<IActionResult> Deposit(int id, decimal amount)
        {
            var account = await _context.Accounts.FindAsync(id);

            AmountValidation(amount);

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
        public async Task<IActionResult> SelectAccountToDeposit()
        {
            //var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            //if (!customerID.HasValue)
            //{
            //    return RedirectToAction("Login", "Customer");
            //}

            //var accounts = await _context.Accounts
            //                             .Where(a => a.CustomerID == customerID.Value)
            //                             .ToListAsync();

            //if (accounts == null || !accounts.Any())
            //{
            //    return NotFound("No accounts found.");
            //}
            var accounts = await GetAccountsForLoggedInCustomerAsync();
            var viewModel = new ATMViewModel
            {
                ActionType = TransactionType.Deposit,
                Accounts = accounts
            };

            return View("ATMSelectAccount",viewModel);
        }

        // Inside CustomerController.cs

        // Action method to display accounts for withdrawal selection
        public async Task<IActionResult> SelectAccountToWithdraw()
        {
            var accounts = await GetAccountsForLoggedInCustomerAsync();
            var viewModel = new ATMViewModel
            {
                ActionType = TransactionType.Withdraw,
                Accounts = accounts
            };

            return View("ATMSelectAccount", viewModel);
        }

        // Action method to display accounts for transfer selection
        public async Task<IActionResult> SelectAccountToTransfer()
        {
            var accounts = await GetAccountsForLoggedInCustomerAsync();
            var viewModel = new ATMViewModel
            {
                ActionType = TransactionType.Transfer,
                Accounts = accounts
            };

            return View("ATMSelectAccount", viewModel);
        }

        public async Task<IActionResult> SelectAccountForStatement()
        {
            var accounts = await GetAccountsForLoggedInCustomerAsync();
            var viewModel = new ATMViewModel
            {
                ActionType = 0,
                Accounts = accounts
            };

            return View("ATMSelectAccount", viewModel);
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


        [HttpPost]
        public async Task<IActionResult> Withdraw(int id, decimal amount)
        {
            var account = await _context.Accounts.FindAsync(id);

            AmountValidation(amount);

            if (!ModelState.IsValid)
            {
                ViewBag.Amount = amount;
                return View(account);
            }

            // Note this code could be moved out of the controller, e.g., into the Model.
            account.Balance -= amount;
            account.Transactions.Add(
                new Transaction
                {
                    TransactionType = TransactionType.Withdraw,
                    Amount = amount,
                    TransactionTimeUtc = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private void AmountValidation(decimal amount)
        {
            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            else if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");

        }

        private async Task<List<Account>> GetAccountsForLoggedInCustomerAsync()
        {
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));

            if (!customerID.HasValue)
            {
                RedirectToAction("Login", "Customer");
                return null; // or throw an exception, handle it as appropriate for your application
            }

            var accounts = await _context.Accounts
                                         .Where(a => a.CustomerID == customerID.Value)
                                         .ToListAsync();

            return accounts;
        }


    }
}

