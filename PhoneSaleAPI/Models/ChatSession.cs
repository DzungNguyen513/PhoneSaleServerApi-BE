using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class ChatSession
    {
        public ChatSession()
        {
            ChatMessages = new HashSet<ChatMessage>();
        }

        public string SessionId { get; set; } = null!;
        public string? SessionName { get; set; }
        public string? CustomerId { get; set; }
        public string? Username { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public bool? IsActive { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Account? UsernameNavigation { get; set; }
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
    }
}
