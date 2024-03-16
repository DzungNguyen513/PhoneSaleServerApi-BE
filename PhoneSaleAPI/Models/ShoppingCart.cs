using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class ShoppingCart
    {
        public ShoppingCart()
        {
            ShoppingCartDetails = new HashSet<ShoppingCartDetail>();
        }

        public string ShoppingCartId { get; set; } = null!;
        public string CustomerId { get; set; } = null!;
        public int? TotalCart { get; set; }
        public int? Status { get; set; }

        public virtual Customer Customer { get; set; } = null!;
        public virtual ICollection<ShoppingCartDetail> ShoppingCartDetails { get; set; }
    }
}
