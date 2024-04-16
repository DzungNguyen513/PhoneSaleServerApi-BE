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
        public DateTime? DateBill { get; set; }
        public string? DeliveryAddress { get; set; }
        public int? Status { get; set; }
        public string? Note { get; set; }
        public int? TotalBill { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
    }
}
