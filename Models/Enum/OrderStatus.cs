namespace CodeSparkNET.Models.Enum
{
    public enum OrderStatus
    {
        Pending = 1,    // Ожидает обработки
        Processing = 2, // В обработке
        Completed = 3,  // Завершен
        Cancelled = 4,  // Отменен
        Refunded = 5   // Возврат средств
    }
}