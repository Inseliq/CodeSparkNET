using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSparkNET.Models
{
    public class Module
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CourseId { get; set; }
        public CourseDetail Course { get; set; } = null!;

        public string Title { get; set; } = null!;
        public int Order { get; set; } // порядок модуля в курсе

        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public ICollection<CourseAssignment> Assignments { get; set; } = new List<CourseAssignment>();
    }
}