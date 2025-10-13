using System.ComponentModel.DataAnnotations;

namespace CodeSparkNET.ViewModels.Profile
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Обязательное поле.")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "Обязательное поле.")]
        [Compare("ConfirmPassword", ErrorMessage = "Пароли не совпадают.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Обязательное поле.")]
        public string ConfirmPassword { get; set; }
    }
}
