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
        public decimal? StoragePrice { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
