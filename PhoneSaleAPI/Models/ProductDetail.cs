using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class ProductDetail
    {
        public string ProductDetailId { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        public int? StorageGb { get; set; }
        public string? ColorName { get; set; }

        public virtual Color? ColorNameNavigation { get; set; }
        public virtual Product Product { get; set; } = null!;
        public virtual Storage? StorageGbNavigation { get; set; }
    }
}
