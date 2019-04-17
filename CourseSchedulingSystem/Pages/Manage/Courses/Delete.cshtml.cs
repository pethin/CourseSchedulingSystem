﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        [FromRoute] public Guid Id { get; set; }
        
        [BindProperty] public Course Course { get; set; }
        
        public IEnumerable<Instructor> Instructors { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Course = await Context.Courses
                .Include(c => c.Department)
                .Include(c => c.Subject)
                .Include(c => c.CourseScheduleTypes)
                .ThenInclude(cst => cst.ScheduleType)
                .Include(c => c.CourseCourseAttributes)
                .ThenInclude(cat => cat.CourseAttribute)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (Course == null) return NotFound();
            
            Instructors = Context.Instructors
                .Where(i => i.ScheduledMeetingTimeInstructors.Any(smti =>
                    smti.ScheduledMeetingTime.CourseSection.CourseId == Id));
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Course = await Context.Courses.FindAsync(Id);

            if (Course != null)
            {
                Context.Courses.Remove(Course);
                await Context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}