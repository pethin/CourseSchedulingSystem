﻿using System;
using System.Collections.Async;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.InstructionalMethods
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute] public Guid Id { get; set; }
        
        [BindProperty] public InstructionalMethod InstructionalMethod { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            InstructionalMethod = await _context.InstructionalMethods.FirstOrDefaultAsync(m => m.Id == Id);

            if (InstructionalMethod == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var instructionalMethod = await _context.InstructionalMethods.FindAsync(Id);

            if (await TryUpdateModelAsync(
                instructionalMethod,
                "InstructionalMethod",
                im => im.Code,
                im => im.Name,
                im => im.IsRoomRequired))
            {
                await instructionalMethod.DbValidateAsync(_context).AddErrorsToModelState(ModelState);

                if (!ModelState.IsValid) return Page();

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}