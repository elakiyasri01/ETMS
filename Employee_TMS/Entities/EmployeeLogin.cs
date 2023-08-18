
using System.ComponentModel.DataAnnotations;

namespace Employee_TMS.Entities
{
    public class EmployeeLogin
    {
        [Required(ErrorMessage = "Enter your User ID")]
        public string EmployeeId { get; set; } = null!;
        [Required(ErrorMessage = "Enter your Password")]
        public string EmployeePassword { get; set; } = null!;
        [Required]
        public string Token { get; set; } = null!;
    }
}


