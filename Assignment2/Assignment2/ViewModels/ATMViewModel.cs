using System;
using DataModelLibrary.Models;

namespace Assignment2.ViewModels
{
	public class ATMViewModel
	{
		public string ActionType { get; set; }
		public List<Account> Accounts { get; set; }
	}
}

