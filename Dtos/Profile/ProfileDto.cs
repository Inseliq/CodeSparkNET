using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CodeSparkNET.Dtos.Profile
{
    public class ProfileDto
    {
        public UpdatePersonalProfileDto UpdatePersonalProfileDto { get; set; }
        public ChangePasswordDto ChangePasswordDto { get; set; }
    }
}