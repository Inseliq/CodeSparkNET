using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CodeSparkNET.Dtos.Account
{
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "Введите имя пользователя.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Введите почту.")]
        public string Email { get; set; }
    }
}