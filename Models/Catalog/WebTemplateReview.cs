using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSparkNET.Models
{
    public class WebTemplateReview
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; } = null!; // FK to AppUser
        public AppUser AppUser { get; set; } = null!;

        public Guid TemplateId { get; set; } // FK to WebTemplateDetail
        public WebTemplateDetail Template { get; set; } = null!;

        public int Rating { get; set; } // Оценка от 1 до 5
        public string? ReviewText { get; set; } // Текст отзыва
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublished { get; set; } = true; // Опубликован ли отзыв
    }
}