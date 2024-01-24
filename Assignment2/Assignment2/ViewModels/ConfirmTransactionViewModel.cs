using System;
namespace Assignment2.ViewModels
{
	public class ConfirmTransactionViewModel
	{
		public string ActionType { get; set; }
		public int AccountNumber { get; set; }
		public int? DestinationAccountNumber { get; set; }
		public decimal Amount { get; set; }
		public string Comment { get; set; }	
	}
}

