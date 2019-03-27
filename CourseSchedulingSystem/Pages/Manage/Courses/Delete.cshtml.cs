﻿using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Courses
{
    public class DeleteModel : CoursesPageModel
    {
        public DeleteModel(ApplicationDbContext context) : base(context)
        {
        }

        [BindProperty] public Course Course { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            Course = await Context.Courses
                .Include(c => c.Department)
                .Include(c => c.Subject)
                .Include(c => c.CourseScheduleTypes)
                .ThenInclude(cst => cst.ScheduleType)
                .Include(c => c.CourseAttributeTypes)
                .ThenInclude(cat => cat.AttributeType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Course == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null) return NotFound();

            Course = await Context.Courses.FindAsync(id);

            if (Course != null)
            {
                Context.Courses.Remove(Course);
                await Context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}