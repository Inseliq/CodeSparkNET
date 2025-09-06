using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Models;

namespace CodeSparkNET.Dtos.Products
{
    public class CourseDto
    {
        public Product Product { get; set; }
        public CourseDetail CourseDetail { get; set; }
        public List<Module> Modules { get; set; }
        public int TotalLessons { get; set; }
        public bool IsUserEnrolled { get; set; }
        public int EnrolledStudents { get; set; }
    }
}