using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Employee_TMS.Entities
{
    public partial class Employee
    {
        public Employee()
        {
            Reports = new HashSet<Report>();
        }
        [Required(ErrorMessage = "Enter your EmployeeID")]
        public string EmployeeId { get; set; } 
        [Required(ErrorMessage = "Enter your Password")]
        public string? EmployeePassword { get; set; }
        public string FullName { get; set; } 
        public string Department { get; set; }
        public string? SecurityQuestion { get; set; }
        public string? SecurityAnswer { get; set; }
        public DateTime? Passwordchanged { get; set; }
        public string? Designation { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? ReportingTo { get; set; }
        public byte[]? ProfileImage { get; set; }
        public string? ProfileImageName { get; set; }

        public virtual ICollection<Report> Reports { get; set; }
    }
}
