﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CourseSchedulingSystem.Data;
using CourseSchedulingSystem.Data.Models;
using CourseSchedulingSystem.Utilities;
using CourseSchedulingSystem.Utilities.Gijgo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Pages.Manage.Courses
{
    public class IndexModel : CoursesPageModel
    {
        public IndexModel(ApplicationDbContext context) : base(context)
        {
        }

        public IActionResult OnGet()
        {
            ViewData["DepartmentId"] = Context.Courses
                .Include(c => c.Department)
                .GroupBy(c => c.DepartmentId, (departmentId, courses) => new SelectListItem
            {
                Value = departmentId.ToString(),
                Text = courses.First().Department.Code
            });
            
            ViewData["CreditHours"] = Context.Courses.GroupBy(c => c.CreditHours, (ch, course) => new SelectListItem
            {
                Value = $"{ch}",
                Text = $"{ch}"
            });

            return Page();
        }

        public async Task<IActionResult> OnGetDataAsync(
            int? pageNumber, int? limit,
            string sortBy,
            string direction,
            [FromQuery(Name = "departments[]")] List<Guid> departments,
            string identifier,
            string title,
            [FromQuery(Name = "creditHours[]")] List<decimal> creditHours)
        {
            var query = Context.Courses
                .Include(c => c.Department)
                .Include(c => c.Subject)
                .ConditionalWhere(() => departments?.Count > 0, c => departments.Contains(c.DepartmentId))
                .ConditionalWhere(() => !string.IsNullOrWhiteSpace(identifier), c => (c.Subject.Code + c.Number).Contains(identifier))
                .ConditionalWhere(() => !string.IsNullOrWhiteSpace(title), c => c.Title.Contains(title))
                .ConditionalWhere(() => creditHours?.Count > 0, c => creditHours.Contains(c.CreditHours))
                .Select(c => new CourseRecord
                {
                    Id = c.Id,
                    Department = c.Department.Code,
                    Identifier = c.Subject.Code + c.Number,
                    Title = c.Title,
                    CreditHours = c.CreditHours,
                    Links = new RecordCrudLinks
                    {
                        Edit = Url.Page("Edit", new { id = c.Id }),
                        Details = Url.Page("Details", new { id = c.Id }),
                        Delete = Url.Page("Delete", new { id = c.Id })
                    }
                });

            var total = query.Count();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
            {
                if (direction.Trim().ToLower() == "asc")
                {
                    switch (sortBy.Trim().ToLower())
                    {
                        case "department":
                            query = query.OrderBy(q => q.Department).ThenBy(q => q.Identifier);
                            break;
                        case "identifier":
                            query = query.OrderBy(q => q.Identifier);
                            break;
                        case "title":
                            query = query.OrderBy(q => q.Title);
                            break;
                        case "creditHours":
                            query = query.OrderBy(q => q.CreditHours);
                            break;
                    }
                }
                else
                {
                    switch (sortBy.Trim().ToLower())
                    {
                        case "department":
                            query = query.OrderByDescending(q => q.Department).ThenBy(q => q.Identifier);
                            break;
                        case "identifier":
                            query = query.OrderByDescending(q => q.Identifier);
                            break;
                        case "title":
                            query = query.OrderByDescending(q => q.Title);
                            break;
                        case "creditHours":
                            query = query.OrderByDescending(q => q.CreditHours);
                            break;
                    }
                }
            }
            else
            {
                query = query.OrderBy(q => q.Department).ThenBy(q => q.Identifier);
            }

            List<CourseRecord> records;
            if (pageNumber.HasValue && limit.HasValue)
            {
                int start = (pageNumber.Value - 1) * limit.Value;
                records = await query.Skip(start).Take(limit.Value).ToListAsync();
            }
            else
            {
                records = await query.ToListAsync();
            }

            return new JsonResult(new DataGridResult<CourseRecord>
            {
                Records = records,
                Total = total
            });
        }

        public class CourseRecord : IRecord
        {
            public Guid Id { get; set; }
            public string Department { get; set; }
            public string Identifier { get; set; }
            public string Title { get; set; }
            public decimal CreditHours { get; set; }
            public RecordCrudLinks Links { get; set; }
        }
    }
}