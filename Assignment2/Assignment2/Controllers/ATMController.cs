using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModelLibrary.Data;
using DataModelLibrary.Models;
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
                AccountNumber = id
            };
            if (actionType == TransactionType.Transfer.ToString())
            {
                var accounts = _context.Accounts.Where(account => account.AccountNumber != id).ToList();
                viewModel.AllAccounts = accounts;
            }

            return View(viewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> ConfirmTransaction(TransactionFormViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                var account = await _context.Accounts.FindAsync(viewModel.AccountNumber);
                viewModel.CurrentAccount = account;
                if (viewModel.ActionType == TransactionType.Transfer.ToString())
                {
                    var accounts = _context.Accounts.Where(account => account.AccountNumber != viewModel.AccountNumber).ToList();
                    viewModel.AllAccounts = accounts;
                }
                return View("TransactionForm", viewModel);

            }

            return View("ConfirmTransaction", viewModel);
        }




        [HttpPost]
        public async Task<IActionResult> TransactionForm(int accountNumber, decimal amount, string comment, string actionType, int? destinationAccountNumber)
        {
            var account = await _context.Accounts.FindAsync(accountNumber);

            if (!ModelState.IsValid)
            {
                //foreach (var key in ModelState.Keys)
                //{
                //    var errors = ModelState[key].Errors;
                //    foreach (var error in errors)
                //    {
                //        var errorMessage = error.ErrorMessage;
                //        Console.WriteLine($"Property: {key}, Error: {errorMessage}");
                //    }
                //}
                return RedirectToAction("Message", "Customer", new { success = false, message = "Something went wrong!" });
            }

            var newTransaction = new Transaction
            {
                Amount = amount,
                Comment = comment,
                TransactionTimeUtc = DateTime.Now
            };

            switch (actionType)
            {
                case nameof(TransactionType.Deposit):
                    await ProcessDeposit(account, amount, newTransaction);
                    break;

                case nameof(TransactionType.Withdraw):
                    await ProcessWithdraw(account, amount, newTransaction);
                    break;

                case nameof(TransactionType.Transfer):
                    await ProcessTransfer(account, destinationAccountNumber, amount, newTransaction);
                    break;
            }

            account.Transactions.Add(newTransaction);
            await _context.SaveChangesAsync();

            return RedirectToAction("Message", "Customer", new { success = true, message = "Your transation has been processed successfully!" });
        }

        private async Task ProcessDeposit(Account account, decimal amount, Transaction newTransaction)
        {
            account.Balance += amount;
            newTransaction.TransactionType = TransactionType.Deposit;
        }

        private async Task ProcessWithdraw(Account account, decimal amount, Transaction newTransaction)
        {
            account.Balance -= amount;
            newTransaction.TransactionType = TransactionType.Withdraw;
            await ChargeServiceFee(account, ServiceFee.Withdraw);
        }

        private async Task ProcessTransfer(Account account, int? destinationAccountNumber, decimal amount, Transaction newTransaction)
        {
            account.Balance -= amount;
            newTransaction.TransactionType = TransactionType.Transfer;

            if (destinationAccountNumber.HasValue)
            {
                var destinationAccount = await _context.Accounts.FindAsync(destinationAccountNumber.Value);
                destinationAccount.Balance += amount;
                destinationAccount.Transactions.Add(new Transaction
                {
                    TransactionType = TransactionType.IncomingTransfer,
                    Amount = amount,
                    TransactionTimeUtc = DateTime.Now
                });
                await ChargeServiceFee(account, ServiceFee.Transfer);
            }
        }

        private async Task ChargeServiceFee(Account account, decimal serviceFee)
        {
            if (!AccountUtilities.AccountQualifiesForFreeServiceFee(account))
            {
                account.Balance -= serviceFee;
                account.Transactions.Add(new Transaction
                {
                    TransactionType = TransactionType.ServiceCharge,
                    Amount = serviceFee,
                    TransactionTimeUtc = DateTime.Now
                });
            }
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


        



    }
}

