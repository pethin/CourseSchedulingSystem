﻿using System.Collections.Async;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CourseSchedulingSystem.Pages.Manage.Terms
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public Term Term { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var term = new Term();

            if (await TryUpdateModelAsync(
                term,
                "Term",
                t => t.Name, t => t.StartDate, t => t.EndDate))
            {
                await term.DbValidateAsync(_context).ForEachAsync(result =>
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                });

                if (!ModelState.IsValid) return Page();

                _context.Terms.Add(term);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}