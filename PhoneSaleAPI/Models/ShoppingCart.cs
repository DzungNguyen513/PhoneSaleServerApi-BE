using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class ShoppingCart
    {
        public string ShoppingCartId { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        public string CustomerId { get; set; } = null!;
        public int? Amount { get; set; }
        public decimal? TotalCart { get; set; }
        public int? Status { get; set; }

        public virtual Customer Customer { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
