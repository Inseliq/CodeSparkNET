using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CodeSparkNET.Dtos.Profile;
using CodeSparkNET.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeSparkNET.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly IProfileService _profileService;

        public ProfileController(ILogger<ProfileController> logger, IProfileService profileService)
        {
            _logger = logger;
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!ModelState.IsValid) return View();

            var email = User.FindFirstValue(ClaimTypes.Email);
            var personalInfo = await _profileService.GetPersonalInfoAsync(email);
            ViewBag.UserName = personalInfo.UserName;
            ViewBag.Email = personalInfo.Email;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileDto profileDto)
        {
            if (!ModelState.IsValid)
            {
                // restore ViewBag if needed
                ViewBag.UserName = profileDto?.UpdatePersonalProfileDto?.UserName;
                ViewBag.Email = profileDto?.UpdatePersonalProfileDto?.Email;
                ViewBag.Role = User?.FindFirstValue(ClaimTypes.Role);
                // explicitly return the Profile view (not "UpdateProfile")
                return View("Profile", profileDto);
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _profileService.UpdatePersonalProfileAsync(email, profileDto.UpdatePersonalProfileDto);

            if (result.Succeeded)
            {
                // PRG: redirect to GET Profile so updated data is loaded
                return RedirectToAction(nameof(Profile));
            }

            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);

            ViewBag.UserName = profileDto?.UpdatePersonalProfileDto?.UserName;
            ViewBag.Email = profileDto?.UpdatePersonalProfileDto?.Email;
            ViewBag.Role = User?.FindFirstValue(ClaimTypes.Role);

            return View("Profile", profileDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ProfileDto profileDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.UserName = profileDto?.UpdatePersonalProfileDto?.UserName;
                ViewBag.Email = profileDto?.UpdatePersonalProfileDto?.Email;
                ViewBag.Role = User?.FindFirstValue(ClaimTypes.Role);
                return View("Profile", profileDto);
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _profileService.ChangePasswordAsync(email, profileDto.ChangePasswordDto);

            if (result.Succeeded)
                return RedirectToAction(nameof(Profile));

            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);

            ViewBag.UserName = profileDto?.UpdatePersonalProfileDto?.UserName;
            ViewBag.Email = profileDto?.UpdatePersonalProfileDto?.Email;
            ViewBag.Role = User?.FindFirstValue(ClaimTypes.Role);

            return View("Profile", profileDto);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}