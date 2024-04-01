using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Bills = new HashSet<Bill>();
            SystemNotificationReads = new HashSet<SystemNotificationRead>();
        }

        public string CustomerId { get; set; } = null!;
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public int? Status { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ShoppingCart? ShoppingCart { get; set; }
        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<SystemNotificationRead> SystemNotificationReads { get; set; }
    }
}
