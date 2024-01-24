using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment2.Models;

//public enum AccountType
//{
//    Checking = 1,
//    Saving = 2
//}

public class Account
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Account Number")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Account Number must be 4 digits.")]
    public int AccountNumber { get; set; }

    [Display(Name = "Type")]
    [RegularExpression("^(S|C)$", ErrorMessage = "AccountType must be 'S' or 'C'.")]
    public string AccountType { get; set; }

    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }

    [Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    public decimal Balance { get; set; }

    // Set ambiguous navigation property with InverseProperty annotation or Fluent-API in the McbaContext.cs file.
    [InverseProperty("Account")]
    public virtual List<Transaction> Transactions { get; set; }
}
