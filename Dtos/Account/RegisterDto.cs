using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSparkNET.Dtos.Account
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Имя пользователя обязательно.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "От 3 до 50 символов.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email обязателен.")]
        [EmailAddress(ErrorMessage = "Неверный формат email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль минимум 6 символов.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Необходимо согласиться с условиями.")]
        public bool ConfirmToS { get; set; }

        public bool ConfirmAd { get; set; }
    }
}