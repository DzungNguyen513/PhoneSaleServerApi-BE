using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class Account
    {
        public string Username { get; set; } = null!;
        public string? Password { get; set; }
        public DateTime? LastLogin { get; set; }
        public int? Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
