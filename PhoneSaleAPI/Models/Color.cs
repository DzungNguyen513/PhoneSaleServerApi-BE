using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class Color
    {
        public Color()
        {
            Products = new HashSet<Product>();
        }

        public string ColorName { get; set; } = null!;
        public decimal? ColorPrice { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
