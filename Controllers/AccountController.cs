using System.Diagnostics;
using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Interfaces;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CodeSparkNET.Controllers
{
    public class AccountController : Controller
    {
        private ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;

        public AccountController(
            IAccountService accountService
            )
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
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

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
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

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountService.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
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

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return BadRequest("Ошибка сброса пароля");

            return View(new ResetPasswordDto { Email = email, Token = token });
        }

        [HttpPost]
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
                return Json(new { success = true, message = "Пароль успешно восстановлен." });
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