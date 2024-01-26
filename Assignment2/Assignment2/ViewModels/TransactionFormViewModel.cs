using System;
using System.ComponentModel.DataAnnotations;
using Assignment2.Migrations;
using Assignment2.Models;
namespace Assignment2.ViewModels
{
	public class TransactionFormViewModel 
    {
		public Account CurrentAccount{ get; set; }
		public string ActionType { get; set; }
		public List<Account> AllAccounts { get; set; }

        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "The amount must be a positive number.")]
        [Required, ValidAmounts]
        public decimal Amount { get; set; }

        [StringLength(30, ErrorMessage = "The comment field must be less than 30 characters.")]
        public string Comment { get; set; }

        public int DestinationAccountNumber { get; set; }

        public int AccountNumber { get; set; }

        public string AccountType { get; set; }

        public decimal Balance { get; set; }

        public string FreeService { get; set; }



    }
}

