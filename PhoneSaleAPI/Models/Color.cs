using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class Color
    {
        public Color()
        {
            ProductImages = new HashSet<ProductImage>();
            Products = new HashSet<Product>();
        }
        public string ColorName { get; set; } = null!;
        public string? ColorImage { get; set; }
        public int? ColorPrice { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
