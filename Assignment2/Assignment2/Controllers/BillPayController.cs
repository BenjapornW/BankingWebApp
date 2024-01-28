using System;
using System.Threading.Tasks;
using DataModelLibrary.Data;
using DataModelLibrary.Models;
using Assignment2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Assignment2.Controllers
{
    public class BillPayController : Controller
    {
        private readonly McbaContext _context;
        private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

        public BillPayController(McbaContext context)
        {
            _context = context;
        }

        // GET: BillPay/Create
        public IActionResult Create()
        {

            // list account number and payee
            var payees = _context.Payees.ToList();
            var accounts = _context.Accounts.Where(account => account.CustomerID == CustomerID);
            ViewBag.Payees = payees;
            ViewBag.Accounts = accounts;
            var viewModel = new BillPay
            {
                // Set the default value to the current date and time
                ScheduleTimeUtc = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute)
            };

            return View(viewModel);
        }

        // POST: BillPay/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BillPay billPay)
        {
            // Check if the scheduled time is in the future
            if (billPay.ScheduleTimeUtc <= DateTime.UtcNow)
            {
                ModelState.AddModelError("ScheduleTimeUtc", "The schedule time must be in the future.");
            }

            // Check if the amount is greater than or equal to $0.01
            if (billPay.Amount < 0.01m)
            {
                ModelState.AddModelError("Amount", "The amount must be at least $0.01.");
            }


            if (ModelState.IsValid)
            {

                billPay.Status = StatusType.Scheduled;
                TempData["BillPay"] = JsonConvert.SerializeObject(billPay);
                return RedirectToAction("ConfirmBill", billPay);

            }

   

            // Repopulate dropdown lists if we return to the view with validation errors
            // list account number and payee
            var payees = _context.Payees.ToList();
            var accounts = _context.Accounts.Where(account => account.CustomerID == CustomerID);
            ViewBag.Payees = payees;
            ViewBag.Accounts = accounts;

            return View(billPay);
        }



        // POST: BillPay/SubmitBill
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitBill(BillPay billPay)
        {
            //// Load the Payee again since it's not included in the form submission

            if (ModelState.IsValid)
            {
                _context.Add(billPay);
                await _context.SaveChangesAsync();
                return RedirectToAction("Message", "Customer", new { success = true, message = "Your bill pay has been scheduled successfully!" });

            }

            foreach (var key in ModelState.Keys)
            {
                var errorMessages = ModelState[key].Errors.Select(e => e.ErrorMessage);
                foreach (var errorMessage in errorMessages)
                {
                    Console.WriteLine($"Key: {key}, Error: {errorMessage}");
                }
            }
            //If ModelState is not valid or an exception occurred, return to the view with the current model
            return RedirectToAction("Message", "Customer", new { success = false, message = "Something weng wrong! please try again!" });
        }





        private int GetLoggedInCustomerId()
        {
            // Assuming you store the logged-in customer ID in the session upon login
            var loggedInCustomerId = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));

            if (!loggedInCustomerId.HasValue)
            {
                // Handle the case where the customer ID is not in the session, which could mean the user is not logged in.
                // You might want to redirect to the login page or handle it as per your application's flow.
                // For example:
                // return RedirectToAction("Login", "Customer");

                throw new Exception("User is not logged in."); // Or handle this scenario appropriately.
            }

            return loggedInCustomerId.Value;
        }


        // POST: BillPay/CancelBill
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBill(int billPayId)
        {
            var billPay = await _context.BillPays
                .Include(bp => bp.Account)
                .FirstOrDefaultAsync(bp => bp.BillPayID == billPayId && bp.Account.CustomerID == CustomerID);

            if (billPay == null)
            {
                // BillPay not found or does not belong to the current customer
                return NotFound();
            }

            _context.BillPays.Remove(billPay);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(BillPaySummary));
        }



        // Additional CRUD operations can be added here if needed
        public async Task<IActionResult> BillPaySummary()
        {
            // Calculate the total estimated payments for the current and next month
            var today = DateTime.Today;
            var startOfNextMonth = new DateTime(today.Year, today.Month, 1).AddMonths(1);
            var startOfFollowingMonth = startOfNextMonth.AddMonths(1);

            var currentMonthPayments = await _context.BillPays
                .Where(bp => bp.ScheduleTimeUtc >= today && bp.ScheduleTimeUtc < startOfNextMonth)
                .SumAsync(bp => bp.Amount);

            var nextMonthPayments = await _context.BillPays
                .Where(bp => bp.ScheduleTimeUtc >= startOfNextMonth && bp.ScheduleTimeUtc < startOfFollowingMonth)
                .SumAsync(bp => bp.Amount);

            // Fetch individual upcoming bill payments
            var upcomingPayments = await _context.BillPays
                .Where(bp => bp.ScheduleTimeUtc >= today)
                .OrderBy(bp => bp.ScheduleTimeUtc)
                .ToListAsync();

            // check if there are any upcoming payments here and set a flag
            ViewBag.HasBillsToShow = upcomingPayments.Any();

            // Retrieve all bill payments, not just upcoming ones
            var allPayments = await _context.BillPays
                .OrderByDescending(bp => bp.ScheduleTimeUtc)
                .ToListAsync();

            var model = new BillPaySummaryViewModel
            {
                CurrentMonthTotal = currentMonthPayments,
                NextMonthTotal = nextMonthPayments,
                UpcomingPayments = upcomingPayments,
                AllPayments = allPayments // Add this line
            };


            return View(model);
        }

        public IActionResult ConfirmBill()
        {
            if (TempData["BillPay"] is string serializedBillPay)
            {
                var billPay = JsonConvert.DeserializeObject<BillPay>(serializedBillPay);
                // Load the related Payee data from the database
                billPay.Payee = _context.Payees.Find(billPay.PayeeID);
                if (billPay.Payee == null)
                {
                    // If Payee is null, handle it by redirecting or showing an error
                    ModelState.AddModelError("", "The payee could not be found.");
                    return RedirectToAction("Create");
                }
                return View(billPay);
            }
            return RedirectToAction("Create");
        }


        // POST: BillPay/ConfirmBill
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBill(BillPay billPay)
        {
            if (ModelState.IsValid)
            {
                _context.Add(billPay);
                await _context.SaveChangesAsync();
                return RedirectToAction("BillPaySummary");
            }

            return View(billPay);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RetryBill(int billPayId)
        {
            var bill = await _context.BillPays
                .Include(bp => bp.Account)
                .ThenInclude(a => a.Transactions) // Assuming Transactions is a navigation property
                .FirstOrDefaultAsync(bp => bp.BillPayID == billPayId);

            if (bill == null)
            {
                return NotFound();
            }

            var currentTime = DateTime.UtcNow; // Use UTC to be consistent with your time comparisons
            var scheduledTime = bill.ScheduleTimeUtc;
            var periodType = bill.Period;

            // Check if it's time to pay the bill
            if ((periodType == PeriodType.OneOff && scheduledTime <= currentTime) ||
                (periodType == PeriodType.Monthly && IsTimeToPayMonthlyBill(scheduledTime, currentTime)))
            {
                // Check if there's enough balance to pay the bill
                decimal amount = bill.Amount;
                var account = bill.Account;
                if ((account.AccountType == AccountType.Saving && account.Balance - amount >= 0) ||
                    (account.AccountType == AccountType.Checking && account.Balance - amount >= 300))
                {
                    // Pay the bill
                    account.Balance -= amount;
                    bill.Status = StatusType.Paid; // Update the bill status

                    // Add a transaction for the bill payment
                    account.Transactions.Add(new Transaction
                    {
                        TransactionType = TransactionType.BillPay,
                        Amount = amount,
                        TransactionTimeUtc = DateTime.UtcNow
                    });

                    _context.Update(account);
                    if (periodType == PeriodType.OneOff)
                    {
                        _context.BillPays.Remove(bill);
                    }

                    await _context.SaveChangesAsync();
                    TempData["Message"] = $"Bill {bill.BillPayID} has been paid successfully.";
                    TempData["Success"] = true;
                }
                else
                {
                    // Insufficient balance
                    bill.Status = StatusType.InsufficientBalance;
                    TempData["Message"] = $"Bill {bill.BillPayID} could not be paid due to insufficient balance.";
                    TempData["Success"] = false;
                }
            }
            else
            {
                // Not yet time to pay the bill or already paid
                TempData["Message"] = $"Bill {bill.BillPayID} is either already paid or not yet due.";
                TempData["Success"] = false;
            }

            return RedirectToAction(nameof(BillPaySummary));
        }

        // Helper method to determine if it's time to pay a monthly bill
        private bool IsTimeToPayMonthlyBill(DateTime scheduledTime, DateTime currentTime)
        {
            return (scheduledTime.Day < currentTime.Day || (scheduledTime.Day == currentTime.Day && scheduledTime.TimeOfDay <= currentTime.TimeOfDay));
        }





    }
}
