using System.Diagnostics;
using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Interfaces;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeSparkNET.Controllers
{
    /// <summary>
    /// Controller responsible for handling account-related actions such as registration, login, and password management.
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        // private readonly ICacheService _cacheService;

        public AccountController(
            IAccountService accountService
            // ICacheService cacheService
            )
        {
            _accountService = accountService;
            // _cacheService = cacheService;
        }

        /// <summary>
        /// Show register page
        /// </summary>
        /// <returns>A view for the register page.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User?.Identity?.IsAuthenticated is true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="registerDto">The data transfer object containing the user's registration details.</param>
        /// <returns>A view for the registration page.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid) return View(registerDto);

            if (registerDto.ConfirmToS != true)
            {
                ModelState.AddModelError(string.Empty, "Вы не согласились с условиями использования.");
                return View(registerDto);
            }

            var loginLink = Url.Action("Login", "Account", null, Request.Scheme);

            var result = await _accountService.RegisterAsync(registerDto, loginLink);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);
                return View(registerDto);
            }

            // Cache the user after successful registration
            // await _cacheService.CacheUserAsync(registerDto.Email);

            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// Show login page
        /// </summary>
        /// <returns>A view for the login page.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User?.Identity?.IsAuthenticated is true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="loginDto">The data transfer object containing the user's login credentials.</param>
        /// <returns>A view for the login page.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid) return View(loginDto);

            var signResult = await _accountService.PasswordSignInAsync(loginDto);
            if (signResult.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Ваша учетная запись временно заблокирована. Попробуйте позже.");
                return View(loginDto);
            }
            if (!signResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Неверные данные для входа.");
                return View(loginDto);
            }

            // Cache the user after successful login
            // await _cacheService.CacheUserAsync(loginDto.Email);

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Logout user
        /// </summary>
        /// <returns>A redirect to the home page after logging out.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Clear cached user data on logout
            var email = User.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                // await _cacheService.ClearCachedUserAsync(email);
            }

            await _accountService.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Show forgot password page
        /// </summary>
        /// <returns>A view for the forgot password page.</returns>
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Send password reset link to user email
        /// </summary>
        /// <param name="model">The data transfer object containing the email for which the password reset link is requested.</param>
        /// <returns>A view for requesting a password reset.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return BadRequest(new { success = false, errors = modelErrors });
            }
            await _accountService.SendPasswordResetLinkAsync(model.Email);

            return Json(new { success = true, message = "Проверьте вашу почту." });
        }

        /// <summary>
        /// Show reset password page
        /// </summary>
        /// <param name="email">The email address associated with the account for which the password is being reset.</param>
        /// <param name="token">The token used to verify the password reset request.</param>
        /// <returns>A view for resetting the password with pre-filled email and token.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return BadRequest("Ошибка сброса пароля");

            return View(new ResetPasswordDto { Email = email, Token = token });
        }

        /// <summary>
        /// Reset user password
        /// </summary>
        /// <param name="model">The data transfer object containing the email, token, and new password.</param>
        /// <returns>A JSON response indicating the success or failure of the password reset operation.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                var modelErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                return BadRequest(new { success = false, errors = modelErrors });
            }

            var result = await _accountService.ResetPasswordAsync(model);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Пароль успешно восстановлен.";
                return RedirectToAction("Index", "Home");
            }

            var errors = result.Errors.Select(e => e.Description).ToArray();
            return BadRequest(new { success = false, errors });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}