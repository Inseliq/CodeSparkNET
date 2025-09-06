using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Models.Enum;

namespace CodeSparkNET.Models
{
    public class WebTemplateOrder
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; } = null!; // FK to AppUser
        public AppUser AppUser { get; set; } = null!;

        public Guid TemplateId { get; set; } // FK to WebTemplateDetail
        public WebTemplateDetail Template { get; set; } = null!;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? CustomerNotes { get; set; }
        public string? AdminNotes { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}