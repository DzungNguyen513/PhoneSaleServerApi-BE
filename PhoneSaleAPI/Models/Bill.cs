using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class Bill
    {
        public Bill()
        {
            BillDetails = new HashSet<BillDetail>();
        }

        public string BillId { get; set; } = null!;
        public string? CustomerId { get; set; }
        public string? EmployeeId { get; set; }
        public int? Amount { get; set; }
        public DateTime? DateBill { get; set; }
        public int? Status { get; set; }
        public decimal? TotalBill { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Employee? Employee { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
    }
}
