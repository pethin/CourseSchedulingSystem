﻿using System;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Departments
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }
        
        [BindProperty] public Department Department { get; set; }

        public bool InUse => Department.Courses.Any();

        public async Task<IActionResult> OnGetAsync()
        {
            Department = await _context.Departments
                .Include(d => d.DepartmentUsers)
                .ThenInclude(du => du.User)
                .Include(d => d.Courses)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (Department == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Department = await _context.Departments
                .Include(d => d.Courses)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (Department != null)
            {
                if (InUse) return RedirectToPage();

                _context.Departments.Remove(Department);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}