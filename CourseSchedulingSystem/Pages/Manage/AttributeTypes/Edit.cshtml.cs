﻿using System;
using System.Linq;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.AttributeTypes
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty] public AttributeType AttributeType { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            AttributeType = await _context.AttributeTypes.FirstOrDefaultAsync(m => m.Id == id);

            if (AttributeType == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var attributeTypeToUpdate = await _context.AttributeTypes.FindAsync(id);

            if (await TryUpdateModelAsync<AttributeType>(
                attributeTypeToUpdate,
                "AttributeType",
                at => at.Name))
            {
                // Check if any other subject has the same name
                if (await _context.AttributeTypes.AnyAsync(at =>
                    at.Id != attributeTypeToUpdate.Id && at.NormalizedName == attributeTypeToUpdate.NormalizedName))
                {
                    ModelState.AddModelError(string.Empty,
                        $"A subject already exists with the name {attributeTypeToUpdate.Name}.");
                }

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}