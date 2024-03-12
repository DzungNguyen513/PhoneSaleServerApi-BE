using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public string CategoryId { get; set; } = null!;
        public string? CategoryName { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
