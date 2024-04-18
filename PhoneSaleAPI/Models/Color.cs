using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class Color
    {
        public Color()
        {
            BillDetails = new HashSet<BillDetail>();
            ProductDetails = new HashSet<ProductDetail>();
            ProductImages = new HashSet<ProductImage>();
            ShoppingCartDetails = new HashSet<ShoppingCartDetail>();
        }

        public string ColorName { get; set; } = null!;
        public string? ColorImage { get; set; }
        public int? ColorPrice { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<BillDetail> BillDetails { get; set; }
        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<ShoppingCartDetail> ShoppingCartDetails { get; set; }
    }
}
