using System;
using System.Collections.Generic;

namespace PhoneSaleAPI.Models
{
    public partial class Employee
    {
        public Employee()
        {
            Bills = new HashSet<Bill>();
        }

        public string EmployeeId { get; set; } = null!;
        public string? EmployeeName { get; set; }
        public string? PhoneNumber { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
    }
}
