using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using CodeSparkNET.Models.Enum;

namespace CodeSparkNET.Models
{
    /// <summary>
    /// Represents the main catalog settings and configuration.
    /// This is a singleton model - only one catalog exists in the system.    /// </summary>
    public class Catalog
    {
        [Key]
        public int Id { get; set; } = 1; // Singleton

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = "Каталог товаров";

        [MaxLength(1000)]
        public string? Description { get; set; }

        // Statistics
        public int TotalViews { get; set; } = 0;

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}