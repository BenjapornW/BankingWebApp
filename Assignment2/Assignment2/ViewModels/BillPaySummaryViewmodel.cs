using System;
using DataModelLibrary.Models;

namespace Assignment2.ViewModels
{
    public class BillPaySummaryViewModel
    {
        public decimal CurrentMonthTotal { get; set; }
        public decimal NextMonthTotal { get; set; }
        public List<BillPay> UpcomingPayments { get; set; }
    }
}


