using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSparkNET.Dtos.Account
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string? ConfirmPassword { get; set; }
        public bool? RememberMe { get; set; }
    }
}