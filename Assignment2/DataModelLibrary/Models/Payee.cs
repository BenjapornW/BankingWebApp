using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModelLibrary.Models
{
	public class Payee
	{
		public int PayeeID { get; set; }

		[StringLength(50)]
		public string Name { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(3)]
        [RegularExpression(@"^(?:ACT|NSW|NT|QLD|SA|TAS|VIC|WA)$", ErrorMessage = "Invalid Australian state.")]
        public string State { get; set; }

        [StringLength(4), RegularExpression(@"^\d{4}$", ErrorMessage = "PostCode must be 4 digits.")]
        public string PostCode { get; set; }

        [StringLength(14), RegularExpression(@"^\(0\d\)\s\d{4}\s\d{4}$", ErrorMessage = "Phone Number must be in format: (0X) XXXX XXXX")]
        public string Phone { get; set; }

        public virtual List<BillPay> BillPays { get; set; }
    }
}

