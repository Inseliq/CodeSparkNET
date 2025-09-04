using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CodeSparkNET.Dtos.Account
{
    public class UpdatePasswordDto
    {
        [Required(ErrorMessage = "Пароль обязателен.")]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ComparePassword { get; set; }
    }
}