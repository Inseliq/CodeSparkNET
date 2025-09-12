using System.Security.Claims;
using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Dtos.Profile;
using CodeSparkNET.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeSparkNET.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly IProfileService _profileService;
        private readonly IAccountService _accountService;
        // private readonly ICacheService _cacheService;

        public ProfileController(
            ILogger<ProfileController> logger,
            IProfileService profileService,
            IAccountService accountService
            // ICacheService cacheService
            )
        {
            _logger = logger;
            _profileService = profileService;
            _accountService = accountService;
            // _cacheService = cacheService;
        }

        /// <summary>
        /// Show profile page
        /// </summary>
        /// <returns>Returns the profile view.</returns>
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            try
            {

                if (!ModelState.IsValid) return View();

                // var user = await _cacheService.GetCachedUserAsync(User.FindFirstValue(ClaimTypes.Email));
                var user = await _accountService.GetUserAsync(User);

                ViewBag.UserName = user.UserName;
                ViewBag.Email = user.Email;
                return View();
            }
            catch (Exception ex)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                _logger.LogError(ex, "Ошибка загрузки профиля у пользователя {email}", email);
                ModelState.AddModelError(string.Empty, "Ошибка загрузки профиля.");
                return View();
            }
        }

        /// <summary>
        /// Update personal profile data
        /// </summary>
        /// <param name="model">The data transfer object containing the updated personal profile information.</param>
        /// <returns>Returns the updated profile view with the result of the operation.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile([Bind(Prefix = "UpdatePersonalProfileDto")] UpdatePersonalProfileDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = true, message = "Ошибка изменения данных." });
                }

                // var user = await _cacheService.GetCachedUserAsync(User.FindFirstValue(ClaimTypes.Email));
                var user = await _accountService.GetUserAsync(User);
                var result = await _profileService.UpdatePersonalProfileAsync(user.Email, model);

                if (result.Succeeded)
                {
                    await _profileService.UpdateUserClaims(user);
                    // await _cacheService.CacheUserAsync(model.Email);
                    // PRG: redirect to GET Profile so updated data is loaded
                    return Json(new { success = true, message = "Профиль успешно обновлен." });
                }

                var modelErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return Json(new { success = false, errors = modelErrors });
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

        /// <summary>
        /// Send email confirmation link
        /// </summary>
        /// <returns>Returns a JSON result indicating the success or failure of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> SendEmailConfirmation()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var modelErrors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray();

                    return BadRequest(new { success = false, errors = modelErrors });
                }
                var user = await _accountService.GetUserAsync(User);

                await _profileService.SendEmailConfirmationLinkAsync(user.Email);
                return Json(new { success = true, message = "Проверьте вашу почту." });
            }
            catch (Exception ex)
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                _logger.LogError(ex, "Ошибка отправки ссылки подтверждения email у пользователя {email}", email);
                return Json(new { success = false, message = "Ошибка отправки ссылки подтверждения email." });
            }
        }

        /// <summary>
        /// Confirm email address
        /// </summary>
        /// <param name="email">The email address to confirm.</param>
        /// <param name="token">The token used to confirm the email address.</param>
        /// <returns>Returns a JSON result indicating the success or failure of the email confirmation.</returns>
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            try
            {
                var model = new ConfirmEmailDto { Email = email, Token = token };
                var result = await _accountService.ConfirmEmailAsync(model);
                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "Email успешно подтвержден." });
                }

                var errors = result.Errors.Select(e => e.Description).ToArray();
                return Json(new { success = false, message = "Ошибка обновления данных." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка подтверждения email {email}", email);
                return Json(new { success = false, message = "Ошибка подтверждения email." });
            }
        }
        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="model">The data transfer object containing the old and new passwords.</param>
        /// <returns>A JSON response indicating the success or failure of the password change operation.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([Bind(Prefix = "ChangePasswordDto")] ChangePasswordDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Ошибка смены пароля.");
                    return Json(new { success = false, message = "Ошибка смены пароля" });
                }

                // var user = await _cacheService.GetCachedUserAsync(User.FindFirstValue(ClaimTypes.Email));
                var user = await _accountService.GetUserAsync(User);
                var result = await _profileService.ChangePasswordAsync(user.Email, model);

                if (result.Succeeded)
                    return Json(new { success = true, message = "Пароль успешно изменен." });

                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);

                return Json(new { success = false, message = "Ошибка смены пароля." });
            }
            catch (Exception ex)
            {
                var user = await _accountService.GetUserAsync(User);
                _logger.LogError(ex, "Ошибка смены пароля у пользователя {email}", user.Email);
                return Json(new { success = false, message = "Ошибка смены пароля" });
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}