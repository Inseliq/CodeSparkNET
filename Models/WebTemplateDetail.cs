using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSparkNET.Models
{
    public class WebTemplateDetail
    {
        [Key]
        [ForeignKey(nameof(Product))]
        public Guid ProductId { get; set; } // Primary key and foreign key to Product
        public Product Product { get; set; } = null!; // Navigation property to the Product

        public string FullDescription { get; set; } = null!; // Подробное описание шаблона
        public string Framework { get; set; } = null!; // Используемый фреймворк (React, Vue, HTML/CSS и т.д.)
        public string Technologies { get; set; } = null!; // Технологии (HTML, CSS, JS, Bootstrap и т.д.)
        public bool IsResponsive { get; set; } = true; // Адаптивный дизайн
        public string BrowserSupport { get; set; } = null!; // Поддержка браузеров

        // Файлы шаблона
        public string SourceCodeUrl { get; set; } = null!; // Ссылка на архив с исходным кодом
        public string DemoUrl { get; set; } = null!; // Ссылка на демо-версию
        public string DocumentationUrl { get; set; } = null!; // Ссылка на документацию
        public string? FigmaUrl { get; set; } // Ссылка на дизайн в Figma (если есть)

        // Характеристики
        public int PagesCount { get; set; } // Количество страниц в шаблоне
        public string ColorScheme { get; set; } = null!; // Цветовая схема
        public string FontsUsed { get; set; } = null!; // Используемые шрифты
        public bool HasDarkMode { get; set; } = false; // Поддержка темной темы
        public bool HasAnimations { get; set; } = false; // Наличие анимаций

        // Дополнительные возможности
        public string Features { get; set; } = null!; // Особенности шаблона (через запятую)
        public string? Dependencies { get; set; } // Зависимости (библиотеки, плагины)
        public string InstallationInstructions { get; set; } = null!; // Инструкции по установке
        public string? License { get; set; } // Лицензия использования

        // Связи
        public ICollection<WebTemplateOrder> Orders { get; set; } = new List<WebTemplateOrder>();
        public ICollection<WebTemplateReview> Reviews { get; set; } = new List<WebTemplateReview>();
    }
}