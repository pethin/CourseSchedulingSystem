﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CourseSchedulingSystem.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public virtual ICollection<DepartmentUser> DepartmentUsers { get; set; }
    }
}
