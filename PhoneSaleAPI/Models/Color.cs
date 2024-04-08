using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class Color
    {
        public Color()
        {
            ProductDetails = new HashSet<ProductDetail>();
            ProductImages = new HashSet<ProductImage>();
        }

        public string ColorName { get; set; } = null!;
        public string? ColorImage { get; set; }
        public int? ColorPrice { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
    }
}
