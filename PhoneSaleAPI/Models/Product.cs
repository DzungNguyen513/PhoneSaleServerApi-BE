using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class Product
    {
        public Product()
        {
            BillDetails = new HashSet<BillDetail>();
            ShoppingCarts = new HashSet<ShoppingCart>();
        }

        public string ProductId { get; set; } = null!;
        public string? ProductName { get; set; }
        public int? Amount { get; set; }
        public decimal? Price { get; set; }
        public string? CategoryId { get; set; }
        public string? VendorId { get; set; }
        public string? Detail { get; set; }
        public string? Img { get; set; }

        public virtual Category? Category { get; set; }
        public virtual Vendor? Vendor { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }
    }
}
