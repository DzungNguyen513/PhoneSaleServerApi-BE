using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class ChatMessage
    {
        public string MessageId { get; set; } = null!;
        public string? SessionId { get; set; }
        public string? AccountId { get; set; }
        public string? CustomerId { get; set; }
        public string? MessageText { get; set; }
        public DateTime? SentAt { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual ChatSession? Session { get; set; }
    }
}
