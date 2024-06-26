﻿using System;
using System.Collections.Generic;
using PhoneSaleAPI.DTO.Bill;

namespace PhoneSaleAPI.Models
{
    public partial class Bill
    {
        public Bill()
        {
            BillDetails = new HashSet<BillDetail>();
        }

        public string BillId { get; set; } = null!;
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? CustomerPhone { get; set; }
        public DateTime? DateBill { get; set; }
        public BillStatus Status { get; set; }
        public string? Note { get; set; }
        public int? TotalBill { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
    }
}
