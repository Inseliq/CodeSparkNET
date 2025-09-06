using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSparkNET.Models
{
    public class Lesson
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ModuleId { get; set; }
        public Module Module { get; set; } = null!;

        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int Order { get; set; }
    }
}