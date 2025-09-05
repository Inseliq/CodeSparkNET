using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Dtos.Profile;
using CodeSparkNET.Interfaces;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodeSparkNET.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<AppUser> _userManager;

        public ProfileService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> UpdatePersonalProfileAsync(string email, UpdatePersonalProfileDto model)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null && model == null)
                return null;

            user.UserName = model.UserName;
            user.Email = model.Email;

            return await _userManager.UpdateAsync(user);
        }
    }
}