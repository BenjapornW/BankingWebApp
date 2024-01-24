using System;
using Assignment2.Models;
namespace Assignment2.ViewModels
{
	public class TransactionFormViewModel
	{
		public Account CurrentAccount{ get; set; }
		//public int? DestinationAccountNumber { get; set; }
		public string ActionType { get; set; }
		public List<Account> AllAccounts { get; set; }

	}
}

