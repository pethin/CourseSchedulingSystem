﻿using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CourseSchedulingSystem.Data.Models
{
    public class ScheduleType
    {
        private string _name;

        public ScheduleType()
        {
        }

        public ScheduleType(string name)
        {
            Name = name;
        }

        public Guid Id { get; set; }

        [Required]
        public string Name
        {
            get => _name;
            set
            {
                _name = value.Trim();
                NormalizedName = _name.ToUpper();
            }
        }

        public string NormalizedName { get; private set; }

        public virtual ICollection<CourseScheduleType> CourseScheduleTypes { get; set; }

        public System.Collections.Async.IAsyncEnumerable<ValidationResult> DbValidateAsync(
            ApplicationDbContext context
        )
        {
            return new AsyncEnumerable<ValidationResult>(async yield =>
            {
                // Check if any schedule type has the same name
                if (await context.ScheduleTypes
                    .Where(st => st.Id != Id)
                    .Where(st => st.NormalizedName == NormalizedName)
                    .AnyAsync())
                    await yield.ReturnAsync(
                        new ValidationResult($"A schedule type already exists with the name {Name}."));
            });
        }
    }
}