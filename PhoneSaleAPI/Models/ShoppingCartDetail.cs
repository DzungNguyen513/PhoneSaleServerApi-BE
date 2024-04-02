using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class ShoppingCartDetail
    {
        public string ShoppingCartId { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        public int? Amount { get; set; }
        public int? Total { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual ShoppingCart ShoppingCart { get; set; } = null!;
    }
}
