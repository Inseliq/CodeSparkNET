using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Models.Enum;

namespace CodeSparkNET.Models
{
    public class DiplomaDetail
    {
        [Key]
        [ForeignKey(nameof(Product))]
        public Guid ProductId { get; set; } // Primary key and foreign key to Product
        public Product Product { get; set; } = null!; // Navigation property to the Product

        public string FullDescription { get; set; } = null!; // Подробное описание работы
        public string Subject { get; set; } = null!; // Предмет/дисциплина
        public DiplomaType DiplomaType { get; set; } // Курсовая или дипломная
        public string Specialization { get; set; } = null!; // Специальность
        public string Keywords { get; set; } = null!; // Ключевые слова (через запятую)

        // Файлы работы
        public string? DocumentUrl { get; set; } // Ссылка на основной документ (.docx/.pdf)
        public string? PresentationUrl { get; set; } // Ссылка на презентацию (.pptx/.pdf)
        public string? SourceCodeUrl { get; set; } // Ссылка на исходный код (если есть)

        // Дополнительная информация
        public string? Bibliography { get; set; } // Список литературы/источников
        public string? Requirements { get; set; } // Особые требования или примечания
        public DateTime CreatedYear { get; set; } // Год создания работы
        public bool HasPlagiarismCheck { get; set; } = false; // Проходила ли проверку на плагиат
        public decimal? PlagiarismPercentage { get; set; } // Процент уникальности

        // Связи
        public ICollection<DiplomaOrder> Orders { get; set; } = new List<DiplomaOrder>();
    }
}