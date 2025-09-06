using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CodeSparkNET.Models
{
    /// <summary>
    /// Represents a category for organizing products in the catalog.
    /// </summary>
    [Index(nameof(Slug), IsUnique = true)]
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!; // Название категории

        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = null!; // URL-friendly версия названия

        [MaxLength(500)]
        public string? Description { get; set; } // Описание категории

        public string? ImageUrl { get; set; } // Изображение категории

        public string? IconUrl { get; set; } // Иконка категории

        // Иерархия категорий
        public Guid? ParentCategoryId { get; set; } // Родительская категория
        [ForeignKey(nameof(ParentCategoryId))]
        public Category? ParentCategory { get; set; }

        // Порядок сортировки
        public int SortOrder { get; set; } = 0;

        // Статус
        public bool IsActive { get; set; } = true;
        public bool IsVisible { get; set; } = true; // Видима ли в каталоге

        // Метаданные для SEO
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }

        // Даты
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Навигационные свойства
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    }
}