using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeSparkNET.Models
{
    public class CourseDetail
    {
        [Key]
        [ForeignKey(nameof(Product))]
        public Guid ProductId { get; set; } // Primary key and foreign key to Product
        public Product Product { get; set; } = null!; // Navigation property to the Product
        public string FullDescription { get; set; } = null!; // Detailed description of the course

        public ICollection<Module>? Modules { get; set; } = new List<Module>(); // Navigation property for related modules
    }
}