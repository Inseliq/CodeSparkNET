using System.ComponentModel.DataAnnotations;

namespace CodeSparkNET.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Введите email.")]
        [EmailAddress(ErrorMessage = "Введите корректный email.")]
        public string? Email { get; set; }
    }
}
