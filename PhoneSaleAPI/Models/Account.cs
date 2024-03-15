using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class Account
    {
        public string Username { get; set; } = null!;
        public string? Password { get; set; }
    }
}
