using System;
using System.Collections.Generic;

namespace Employee_TMS.Entities
{
    public partial class Report
    {
        public string? EmployeeId { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public TimeSpan? WorkedHours { get; set; }
        public int EmployeeSno { get; set; }

        public virtual Employee? Employee { get; set; }
    }
}
