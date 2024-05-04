namespace PhoneSaleAPI.DTO.SystemNotification
{
    public class SystemNotificationDTO
    {
        public string NotificationName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public string NotificationType { get; set; }
        public string? CustomerId { get; set; }
        public bool IsActive { get; set; }
    }

    public class SystemNotificationResponseDTO
    {
        public string NotificationID { get; set; }
        public string NotificationName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public string NotificationType { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; }
    }
}
