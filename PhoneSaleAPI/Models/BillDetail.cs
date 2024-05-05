using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class BillDetail
    {
        public string BillId { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        public string ColorName { get; set; } = null!;
        public int? StorageGb { get; set; }

		public int? Amount { get; set; }
        public int? Price { get; set; }
        public int? Discount { get; set; }
        public int? Total { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Bill Bill { get; set; } = null!;
        public virtual Color ColorNameNavigation { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
        public virtual Storage StorageGbNavigation { get; set; } = null!;
    }
}
