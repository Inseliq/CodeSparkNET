using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Models.Enum;

namespace CodeSparkNET.Models
{
    public class DiplomaOrder
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; } = null!; // FK to AppUser
        public AppUser AppUser { get; set; } = null!;

        public Guid DiplomaId { get; set; } // FK to DiplomaDetail
        public DiplomaDetail Diploma { get; set; } = null!;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? CustomerNotes { get; set; } // Дополнительные пожелания от клиента
        public string? AdminNotes { get; set; } // Заметки администратора

        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; } // Сумма к оплате (может отличаться от базовой цены)

        public DateTime? CompletedAt { get; set; } // Дата выполнения заказа
    }
}