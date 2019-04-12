﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using System.Collections.Async;

namespace CourseSchedulingSystem.Pages.Manage.Rooms
{
    public class CreateModel : RoomPageModel
    {

        public CreateModel(CourseSchedulingSystem.Data.ApplicationDbContext context) : base(context)
        {
        }

        [BindProperty] public RoomInputModel RoomModel { get; set; }

        public IActionResult OnGet()
        {
            ViewData["BuildingId"] = new SelectList(_context.Building.Where(bd => bd.IsEnabled == true), "Id", "Code");
            return Page();
        }

        [BindProperty]
        public Room Room { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            ViewData["BuildingId"] = new SelectList(_context.Building.Where(bd => bd.IsEnabled == true), "Id", "Code");
            await _context.SaveChangesAsync();

            var room = new Room();

            var RoomCapabilityTypes = _context.RoomCapability
            .Where(at => RoomModel.RoomCapabilityIds.Contains(at.Id))
            .Select(at => new RoomRoomCapability
            {
                RoomId = Room.Id,
                RoomCapabilityId = at.Id
            });
            _context.RoomRoomCapability.AddRange(RoomCapabilityTypes);

            if (await TryUpdateModelAsync(room, "Room", r => r.Number, r => r.BuildingId))
            {
                await room.DbValidateAsync(_context).ForEachAsync(result =>
                {
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);
                });

                if (!ModelState.IsValid) return Page();

                _context.Room.Add(Room);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}