using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSparkNET.Models
{
    /// <summary>
    /// Represents an assignment related to a course.
    /// </summary>
    public class CourseAssignment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ModuleId { get; set; }
        public Module Module { get; set; } = null!; // Navigation property to Module
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime DeadLine { get; set; }
        public ICollection<UserAssignment> UserAssignments { get; set; } = new List<UserAssignment>();
    }
}