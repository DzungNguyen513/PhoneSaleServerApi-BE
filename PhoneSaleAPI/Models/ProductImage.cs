using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class ProductImage
    {
        public string ProductImageId { get; set; } = null!;
        public string? ProductId { get; set; }
        public string? ImagePath { get; set; }
        public bool? IsPrimary { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Product? Product { get; set; }
    }
}
