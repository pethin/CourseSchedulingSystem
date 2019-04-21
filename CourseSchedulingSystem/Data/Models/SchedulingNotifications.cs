using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseSchedulingSystem.Data.Models
{
    /// <summary>
    /// Scheduling notifications for course sections and scheduled meeting times.
    /// </summary>
    public class SchedulingNotifications
    {
        /// <summary>Gets or sets the warnings in the notification.</summary>
        [Required]
        public List<string> Warnings { get; set; } = new List<string>();
        
        /// <summary>Gets or sets the errors in the notification.</summary>
        [Required]
        public List<string> Errors { get; set; } = new List<string>();
    }
}