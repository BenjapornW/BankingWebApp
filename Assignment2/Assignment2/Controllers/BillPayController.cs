using System;
using System.Threading.Tasks;
using Assignment2.Data;
using Assignment2.Models;
using Assignment2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Create(BillPay billPay)
        {
            if (ModelState.IsValid)
            {
                billPay.Status = StatusType.Scheduled;
                _context.Add(billPay);
                await _context.SaveChangesAsync();
                return RedirectToAction("Message", "Customer", new { success = true, message = "Your bill pay has been scheduled successfully!" });
            }
            return View(billPay);
        }

        // GET: BillPay/Index
        public async Task<IActionResult> Index()
        {
            var billPays = await _context.BillPays.ToListAsync();
            return View(billPays);
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
    }
}

