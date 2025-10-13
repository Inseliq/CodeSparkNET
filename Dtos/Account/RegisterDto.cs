using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CodeSparkNET.Dtos.Account
{
    public class RegisterDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public bool ConfirmToS { get; set; }

        public bool ConfirmAd { get; set; }
    }
}