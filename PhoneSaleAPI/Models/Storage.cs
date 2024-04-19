using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class Storage
    {
        public Storage()
        {
            BillDetails = new HashSet<BillDetail>();
            ProductDetails = new HashSet<ProductDetail>();
            ShoppingCartDetails = new HashSet<ShoppingCartDetail>();
        }

        public int StorageGb { get; set; }
        public int? StoragePrice { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<BillDetail> BillDetails { get; set; }
        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
        public virtual ICollection<ShoppingCartDetail> ShoppingCartDetails { get; set; }
    }
}
