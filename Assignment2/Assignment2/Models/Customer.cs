using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment2.Models;

public class Customer
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "CustomerID must be a 4-digit number.")]
    public int CustomerID { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; }

    [StringLength(11)]
    [RegularExpression(@"^\d{3} \d{3} \d{3}$", ErrorMessage = "TFN must be in format: XXX XXX XXX")]
    public string TFN { get; set; }

    [StringLength(50)]
    public string Address { get; set; }

    [StringLength(40)]
    public string City { get; set; }

    [StringLength(3)]
    [RegularExpression(@"^(?:ACT|NSW|NT|QLD|SA|TAS|VIC|WA)$", ErrorMessage = "Invalid Australian state.")]
    public string State { get; set; }

    [StringLength(4), RegularExpression(@"^\d{4}$", ErrorMessage = "PostCode must be 4 digits.")]
    public string PostCode { get; set; }

    [StringLength(12), RegularExpression(@"^04\d{2} \d{3} \d{3}$", ErrorMessage = "Mobile Number must be in format: 04XX XXX XXX")]
    public string Mobile { get; set; }

    public virtual List<Account> Accounts { get; set; }

    [NotMapped]
    public virtual Login Login { get; set; }
}
