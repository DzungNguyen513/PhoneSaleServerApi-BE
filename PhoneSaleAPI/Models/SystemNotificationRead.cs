using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class SystemNotificationRead
    {
        public int ReadId { get; set; }
        public int? NotificationId { get; set; }
        public string? CustomerId { get; set; }
        public DateTime? ReadAt { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual SystemNotification? Notification { get; set; }
    }
}
