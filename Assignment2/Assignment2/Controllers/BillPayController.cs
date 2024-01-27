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
                ScheduleTimeUtc = DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour).AddMinutes(DateTime.UtcNow.Minute)
            };

            return View(viewModel);
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

//                 billPay.Status = StatusType.Scheduled;
//                 _context.Add(billPay);
//                 await _context.SaveChangesAsync();
//                 return RedirectToAction("Message", "Customer", new { success = true, message = "Your bill pay has been scheduled successfully!" });

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

        // POST: BillPay/SubmitBill
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitBill(BillPay billPay)
        {
            // Load the Payee again since it's not included in the form submission
            billPay.Payee = _context.Payees.Find(billPay.PayeeID);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(billPay);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(BillPaySummary));
                }
                catch (Exception ex)
                {
                    // Log the exception here
                    ModelState.AddModelError("", "An error occurred while saving the bill payment.");
                }
            }
            // If ModelState is not valid or an exception occurred, return to the view with the current model
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
        [HttpGet]
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




    }
}
