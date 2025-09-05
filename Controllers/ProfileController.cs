using System.Security.Claims;
using CodeSparkNET.Dtos.Profile;
using CodeSparkNET.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> UpdateProfile([Bind(Prefix = "UpdatePersonalProfileDto")] UpdatePersonalProfileDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // restore ViewBag if needed
                    ViewBag.UserName = model?.UserName;
                    ViewBag.Email = model?.Email;
                    ViewBag.Role = User?.FindFirstValue(ClaimTypes.Role);
                    // explicitly return the Profile view (not "UpdateProfile")
                    return View("Profile", model);
                }

                var email = User.FindFirstValue(ClaimTypes.Email);
                var result = await _profileService.UpdatePersonalProfileAsync(email, model);

                if (result.Succeeded)
                {
                    // PRG: redirect to GET Profile so updated data is loaded
                    return RedirectToAction(nameof(Profile));
                }

                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);

                ViewBag.UserName = model?.UserName;
                ViewBag.Email = model?.Email;
                ViewBag.Role = User?.FindFirstValue(ClaimTypes.Role);

                return View("Profile", model);
            }
            catch (Exception ex)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                _logger.LogError(ex, "Ошибка обновления персональных данных в профиле у пользователя {email}", email);
                return View("Profile", model);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([Bind(Prefix = "ChangePasswordDto")] ChangePasswordDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Profile));
                }

                var email = User.FindFirstValue(ClaimTypes.Email);
                var result = await _profileService.ChangePasswordAsync(email, model);

                if (result.Succeeded)
                    return RedirectToAction(nameof(Profile));

                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);

                return RedirectToAction(nameof(Profile), model);
            }
            catch (Exception ex)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                _logger.LogError(ex, "Ошибка смены пароля у пользователя {email}", email);
                return RedirectToAction(nameof(Profile), model);
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}