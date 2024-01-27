using System;
using System.Threading.Tasks;
using Assignment2.Data;
using Assignment2.Models;
using Assignment2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Assignment2.Controllers
{
    public class BillPayController : Controller
    {
        private readonly McbaContext _context;

        public BillPayController(McbaContext context)
        {
            _context = context;
        }

        //// GET: BillPay/Create
        //public IActionResult Create()
        //{
        //    return View(new BillPay());
        //}

        //// POST: BillPay/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(BillPay billPay)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(billPay);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(billPay);
        //}

        //// GET: BillPay/Index
        //public async Task<IActionResult> Index()
        //{
        //    var billPays = await _context.BillPays.ToListAsync();
        //    return View(billPays);
        //}

        // GET: BillPay/Create
        public IActionResult Create()
        {
            // Populate the Payee dropdown list
            ViewData["PayeeID"] = new SelectList(_context.Payees, "PayeeID", "Name");
            return View(new BillPay()); // Pass a new BillPay object to the view
        }

        // POST: BillPay/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountNumber,PayeeID,Amount,ScheduleTimeUtc,Period,Status")] BillPay billPay)
        {
            if (ModelState.IsValid)
            {
                _context.Add(billPay);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index"); // Replace with the name of your listing page
            }
            // Repopulate the Payee dropdown list if we return to the view
            ViewData["PayeeID"] = new SelectList(_context.Payees, "PayeeID", "Name", billPay.PayeeID);
            return View(billPay);
        }

        // GET: BillPay/Index
        public async Task<IActionResult> Index()
        {
            // Replace with the logic you need for your index view
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

