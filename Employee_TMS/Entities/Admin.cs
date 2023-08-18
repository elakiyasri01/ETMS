using System;
using System.Collections.Generic;

namespace Employee_TMS.Entities
{
    public partial class Admin
    {
        public string AdminId { get; set; } = null!;
        public string? AdminPassword { get; set; }
    }
}
