using System;
using Assignment2.Models;
namespace Assignment2.ViewModels
{
	public class ATMViewModel
	{
		public TransactionType ActionType { get; set; }
		public Customer CurrentCustomer { get; set; }
	}
}

