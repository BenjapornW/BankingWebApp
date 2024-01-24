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

        public async Task<IActionResult> TransactionForm(int id, string actionType)  {

            var account = await _context.Accounts.FindAsync(id);
            var viewModel = new TransactionFormViewModel
            {
                CurrentAccount = account,
                ActionType = actionType,
            };
            if (actionType == TransactionType.Transfer.ToString())
            {
                var accounts = _context.Accounts.Where(account => account.AccountNumber != id).ToList();
                viewModel.AllAccounts = accounts;
            }

            return View(viewModel);
        }

        public IActionResult ConfirmTransaction(string actionType, int accountNumber, int? destinationAccountNumber, decimal amount, string comment)
        {
            var viewModel = new ConfirmTransactionViewModel
            {
                ActionType = actionType,
                AccountNumber = accountNumber,
                Amount = amount,
                Comment = comment
            };

            if (actionType == TransactionType.Transfer.ToString())
                viewModel.DestinationAccountNumber = destinationAccountNumber;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> TransactionForm(string actionType, int accountNumber, int? destinationAccountNumber, decimal amount, string comment)
        {
            var account = await _context.Accounts.FindAsync(accountNumber);

            AmountValidation(amount);

            if (!ModelState.IsValid)
            {
                return RedirectToAction("TransactionForm", new { id = accountNumber, actionType = actionType });
            }
            var newTransaction = new Transaction
            {
                Amount = amount,
                Comment = comment,
                TransactionTimeUtc = DateTime.UtcNow
            };
            if (actionType == TransactionType.Deposit.ToString())
            {
                account.Balance += amount;
                newTransaction.TransactionType = TransactionType.Deposit;               
            } else if (actionType == TransactionType.Withdraw.ToString())
            {
                account.Balance -= amount;
                newTransaction.TransactionType = TransactionType.Withdraw;
            }
            else if (actionType == TransactionType.Transfer.ToString())
            {
                account.Balance -= amount;
                newTransaction.TransactionType = TransactionType.Transfer;
                var destinationAccount = await _context.Accounts.FindAsync(destinationAccountNumber);
                destinationAccount.Balance += amount;
                destinationAccount.Transactions.Add(new Transaction
                {
                    TransactionType = TransactionType.IncomingTransfer,
                    Amount = amount,
                    TransactionTimeUtc = DateTime.UtcNow
                });
            }
            account.Transactions.Add(newTransaction);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Customer");

        }

        public async Task<IActionResult> SelectAccount(string actionType)
        {
            
            var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
            if (!customerID.HasValue)
            {
                return RedirectToAction("Login", "Customer");
            }

            var accounts = await _context.Accounts
                                         .Where(a => a.CustomerID == customerID.Value)
                                         .ToListAsync();

            if (accounts == null || !accounts.Any())
            {
                return NotFound("No accounts found.");
            }

            var viewModel = new ATMViewModel
            {
                ActionType = actionType,
                Accounts = accounts
            };

            return View("ATMSelectAccount",viewModel);
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


        private void AmountValidation(decimal amount)
        {
            if (amount <= 0)
                ModelState.AddModelError(nameof(amount), "Amount must be positive.");
            else if (amount.HasMoreThanTwoDecimalPlaces())
                ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");

        }



    }
}

