using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class SystemNotification
    {
        public SystemNotification()
        {
            SystemNotificationReads = new HashSet<SystemNotificationRead>();
        }

        public string NotificationId { get; set; } = null!;
        public string? NotificationName { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Message { get; set; }
        public string? NotificationType { get; set; }
        public string? CustomerId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsActive { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual ICollection<SystemNotificationRead> SystemNotificationReads { get; set; }
    }
}
