using System.ComponentModel.DataAnnotations;

namespace CodeSparkNET.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Введите email.")]
        [EmailAddress(ErrorMessage = "Введите корректный email.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен.")]
        [DataType(DataType.Password, ErrorMessage = "Введите корректный пароль")]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password, ErrorMessage = "Введите корректный пароль")]
        [Compare("Password", ErrorMessage = "Введенные пароли не совпадают.")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Токен для восстановления пароля обязателен.")]
        public string? Token { get; set; }
    }
}
