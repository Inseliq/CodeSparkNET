using System.ComponentModel.DataAnnotations;

namespace CodeSparkNET.ViewModels.Profile
{
    public class UpdatePersonalProfileViewModel
    {
        [Required(ErrorMessage = "Введите имя пользователя.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Введите почту.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
