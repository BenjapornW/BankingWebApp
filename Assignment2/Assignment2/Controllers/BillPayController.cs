using System;
using System.Threading.Tasks;
using Assignment2.Data;
using Assignment2.Models;
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

        public BillPayController(McbaContext context)
        {
            _context = context;
        }

        // GET: BillPay/Create
        public IActionResult Create()
        {
            // TODO: Replace with actual retrieval of the logged-in user's CustomerID
            int customerId = GetLoggedInCustomerId();

            // Populate the Account dropdown list for the specified customer
            ViewData["AccountNumber"] = new SelectList(_context.Accounts
                .Where(a => a.CustomerID == customerId), "AccountNumber", "AccountNumber");

            // Populate the Payee dropdown list
            ViewData["PayeeID"] = new SelectList(_context.Payees, "PayeeID", "Name");

            return View(new BillPay()); // Pass a new BillPay object to the view
        }

        // POST: BillPay/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("AccountNumber,PayeeID,Amount,ScheduleTimeUtc,Period,Status")] BillPay billPay)
        {
            if (billPay.ScheduleTimeUtc <= DateTime.UtcNow)
            {
                ModelState.AddModelError("ScheduleTimeUtc", "The schedule time must be in the future.");
            }

            if (ModelState.IsValid)
            {
                // Pass the BillPay object to the ConfirmBill action via TempData.
                TempData["BillPay"] = JsonConvert.SerializeObject(billPay);
                return RedirectToAction("ConfirmBill");
            }

            // Repopulate dropdown lists if we return to the view with validation errors
            int customerId = GetLoggedInCustomerId();
            ViewData["AccountNumber"] = new SelectList(_context.Accounts
                .Where(a => a.CustomerID == customerId), "AccountNumber", "AccountNumber", billPay.AccountNumber);
            ViewData["PayeeID"] = new SelectList(_context.Payees, "PayeeID", "Name", billPay.PayeeID);

            return View(billPay);
        }



        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitBill(BillPay billPay)
        {
            if (ModelState.IsValid)
            {
                _context.Add(billPay);
                await _context.SaveChangesAsync();
                return RedirectToAction("BillPaySummary");
            }

            return View("ConfirmBill", billPay);
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

            var model = new BillPaySummaryViewModel
            {
                CurrentMonthTotal = currentMonthPayments,
                NextMonthTotal = nextMonthPayments,
                UpcomingPayments = upcomingPayments
            };

            return View(model);
        }

        // GET: BillPay/ConfirmBill
        // This action is only for displaying the confirmation page.
        [HttpGet]
        public IActionResult ConfirmBill()
        {
            if (TempData["BillPay"] is string serializedBillPay)
            {
                var billPay = JsonConvert.DeserializeObject<BillPay>(serializedBillPay);

                // Load the Payee object for the billPay
                billPay.Payee = _context.Payees.Find(billPay.PayeeID);

                if (billPay.Payee == null)
                {
                    // Handle the case where Payee is not found, maybe redirect back with an error message.
                    TempData["Error"] = "Payee not found. Please select a valid Payee.";
                    return RedirectToAction("Create");
                }

                return View(billPay);
            }

            // Handle the case where TempData does not have the BillPay object.
            return RedirectToAction("Create");
        }


        // POST: BillPay/ConfirmBill
        // This action is for actually processing the confirmation.
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

            // If the validation fails, re-display the confirmation page with validation errors.
            return View(billPay);
        }


    }
}

