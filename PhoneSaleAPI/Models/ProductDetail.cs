using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class ProductDetail
    {
<<<<<<< HEAD
        public string ProductId { get; set; } = null!;
        public string ColorName { get; set; } = null!;
        public int StorageGb { get; set; }
        public int? Amount { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Color ColorNameNavigation { get; set; } = null!;
        public virtual Storage StorageGbNavigation { get; set; } = null!;
=======
        public string ProductDetailId { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        public int? StorageGb { get; set; }
        public string? ColorName { get; set; }

        public virtual Color? ColorNameNavigation { get; set; }
        public virtual Product Product { get; set; } = null!;
        public virtual Storage? StorageGbNavigation { get; set; }
>>>>>>> 1f5b15b24183bf0102df0552a617a4d6d35ad5e1
    }
}
