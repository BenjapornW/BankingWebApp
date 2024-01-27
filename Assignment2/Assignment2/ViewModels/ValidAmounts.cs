using System;
using System.ComponentModel.DataAnnotations;
using DataModelLibrary.Data;
using DataModelLibrary.Models;
using Assignment2.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Assignment2.ViewModels;

public class ValidAmounts : ValidationAttribute
{

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var viewModel = (TransactionFormViewModel)validationContext.ObjectInstance;
        var amount = viewModel.Amount;
        
        if (amount.HasMoreThanTwoDecimalPlaces())
        {
            return new ValidationResult("Amount cannot have more than 2 decimal places.");
        }

 
        if (viewModel.ActionType != TransactionType.Deposit.ToString())
        {

            decimal balance = viewModel.Balance;

            decimal feeAmount = viewModel.ActionType == TransactionType.Withdraw.ToString() ? ServiceFee.Withdraw : ServiceFee.Transfer;

            if (viewModel.FreeService == "notFree" && amount + feeAmount > balance)
            {
                return new ValidationResult($"Insufficient balance (This transaction includes service fee ${feeAmount:F2})");
            }
            else if (amount > balance)
            {
                return new ValidationResult("Insufficient funds.");
              
            }
            else if (viewModel.FreeService == "notFree" && viewModel.AccountType == AccountType.Checking && amount + feeAmount > balance - 300)
            {
                return new ValidationResult($"Insufficient funds (The minimum balance of checking account is $300 and service fee charge ${feeAmount:F2} this time");
            }
            else if (viewModel.AccountType == AccountType.Checking && amount > balance - 300)
            {
                return new ValidationResult($"Insufficient funds (The minimum balance of checking account is $300)");
      
            }
        }

        return ValidationResult.Success;
    }
}

