using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CodeSparkNET.Dtos.Profile
{
    public class UpdatePersonalProfileDto
    {
        [Required(ErrorMessage = "Введите имя пользователя.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Введите почту.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}