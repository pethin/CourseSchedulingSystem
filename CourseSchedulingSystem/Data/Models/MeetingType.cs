﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CourseSchedulingSystem.Data.Models
{
    public class MeetingType
    {
        private string _name;
        private string _code;
        
        public MeetingType()
        {
        }

        public MeetingType(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public Guid Id { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed.")]
        public string Code
        {
            get => _code;
            set => _code = value?.Trim().ToUpperInvariant();
        }

        [Required]
        public string Name
        {
            get => _name;
            set
            {
                _name = value?.Trim();
                NormalizedName = _name?.ToUpperInvariant();
            }
        }

        public string NormalizedName { get; private set; }
        
        public List<ScheduledMeetingTime> ScheduledMeetingTimes { get; set; }
    }
}
