using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class Vendor
    {
        public Vendor()
        {
            Products = new HashSet<Product>();
        }

        public string VendorId { get; set; } = null!;
        public string? VendorName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
