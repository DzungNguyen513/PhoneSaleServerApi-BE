using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class ChatMessage
    {
        public string MessageId { get; set; } = null!;
        public string? SessionId { get; set; }
        public string? SentByAccountId { get; set; }
        public string? SentByCustomerId { get; set; }
        public string? MessageText { get; set; }
        public DateTime? SentAt { get; set; }

        public virtual Account? SentByAccount { get; set; }
        public virtual Customer? SentByCustomer { get; set; }
        public virtual ChatSession? Session { get; set; }
    }
}
