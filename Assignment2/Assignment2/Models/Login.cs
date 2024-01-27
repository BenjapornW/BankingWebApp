using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModelLibrary.Models;

public class Login
{
    [Column(TypeName = "char")]
    [StringLength(8), RegularExpression(@"^\d{8}$", ErrorMessage = "LoginID must be 8 digits.")]
    public string LoginID { get; set; }

    public int CustomerID { get; set; }
    public virtual Customer Customer { get; set; }

    [Column(TypeName = "char")]
    [Required, StringLength(94)]
    public string PasswordHash { get; set; }

}
