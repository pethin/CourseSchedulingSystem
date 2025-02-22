﻿using System;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.CourseAttributes
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }
        
        public CourseAttribute CourseAttribute { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            CourseAttribute = await _context.CourseAttributes
                .Include(ca => ca.CourseCourseAttributes)
                .ThenInclude(cca => cca.Course)
                .ThenInclude(c => c.Subject)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (CourseAttribute == null) return NotFound();
            return Page();
        }
    }
}