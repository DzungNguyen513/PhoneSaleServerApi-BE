using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class Storage
    {
        public Storage()
        {
            ProductDetails = new HashSet<ProductDetail>();
        }

        public int StorageGb { get; set; }
        public int? StoragePrice { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
    }
}
