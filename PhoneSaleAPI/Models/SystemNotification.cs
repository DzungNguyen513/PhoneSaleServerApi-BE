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

        public int NotificationId { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public string? NotificationType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<SystemNotificationRead> SystemNotificationReads { get; set; }
    }
}
