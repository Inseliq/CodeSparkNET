using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSparkNET.Dtos.Profile
{
    public class ChangePasswordDto
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