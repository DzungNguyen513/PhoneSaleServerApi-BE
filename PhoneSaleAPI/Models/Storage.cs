using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class Storage
    {
        public Storage()
        {
            Products = new HashSet<Product>();
        }

        public int StorageGb { get; set; }
        public int? StoragePrice { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
