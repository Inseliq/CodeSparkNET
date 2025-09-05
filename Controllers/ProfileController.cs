using System.Security.Claims;
using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Dtos.Profile;
using CodeSparkNET.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CodeSparkNET.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly IProfileService _profileService;
        private readonly IAccountService _accountService;

        public ProfileController(ILogger<ProfileController> logger, IProfileService profileService, IAccountService accountService)
        {
            _logger = logger;
            _profileService = profileService;
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {

            if (!ModelState.IsValid) return View();

            var user = await _accountService.GetUserAsync(User);

            ViewBag.UserName = user.UserName;
            ViewBag.Email = user.Email;
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
                    var vmInvalid = new ProfileDto
                    {
                        UpdatePersonalProfileDto = model,
                        ChangePasswordDto = new ChangePasswordDto()
                    };

                    ViewBag.UserName = model?.UserName;
                    ViewBag.Email = model?.Email;
                    ViewBag.Role = User?.FindFirstValue(ClaimTypes.Role);

                    return View("Profile", vmInvalid);
                }

                var user = await _accountService.GetUserAsync(User);
                var result = await _profileService.UpdatePersonalProfileAsync(user.Email, model);

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

                return View("Profile", model); //TODO: return Json to show modal or text about result
            }
            catch (Exception ex)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                _logger.LogError(ex, "Ошибка обновления персональных данных в профиле у пользователя {email}", email);

                var vmException = new ProfileDto
                {
                    UpdatePersonalProfileDto = model ?? new UpdatePersonalProfileDto(),
                    ChangePasswordDto = new ChangePasswordDto()
                };

                ViewBag.UserName = model?.UserName;
                ViewBag.Email = model?.Email;
                ViewBag.Role = User?.FindFirstValue(ClaimTypes.Role);

                return View("Profile", vmException);
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

                var user = await _accountService.GetUserAsync(User);
                var result = await _accountService.ChangePasswordAsync(user.Email, model);

                if (result.Succeeded)
                    return RedirectToAction(nameof(Profile));

                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);

                return RedirectToAction(nameof(Profile), model); //TODO: return Json to show modal or text about result
            }
            catch (Exception ex)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                _logger.LogError(ex, "Ошибка смены пароля у пользователя {email}", email);
                return RedirectToAction(nameof(Profile), model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmailConfirmation(SendEmailConfirmationDto model)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return BadRequest(new { success = false, errors = modelErrors });
            }

            await _accountService.SendEmailConfirmationLinkAsync(model.Email);
            return Json(new { success = true, message = "Проверьте вашу почту." });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {

            var model = new ConfirmEmailDto { Email = email, Token = token };
            var result = await _accountService.ConfirmEmailAsync(model);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Email успешно подтвержден." });
            }

            var errors = result.Errors.Select(e => e.Description).ToArray();
            return Json(new { success = false, message = errors });
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}