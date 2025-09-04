using System.Diagnostics;
using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Interfaces;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeSparkNET.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        private readonly IAccountService _accountService;

        public AccountController(ILogger<AccountController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IAccountService accountService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
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
            try
            {
                if (registerDto.ConfirmToS != true)
                {
                    ModelState.AddModelError(string.Empty, "Вы не согласились с условиями использования.");
                    return View(registerDto);
                }

                if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
                {
                    ModelState.AddModelError(string.Empty, "Пользователь с таким email уже зарегистрирован.");
                    return View(registerDto);
                }

                var user = new AppUser { UserName = registerDto.UserName, Email = registerDto.Email };
                var createResult = await _userManager.CreateAsync(user, registerDto.Password);

                if (!createResult.Succeeded)
                {
                    foreach (var err in createResult.Errors) ModelState.AddModelError(string.Empty, err.Description);
                    return View(registerDto);
                }

                await _userManager.AddToRoleAsync(user, "User");

                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Ошибка при регистрации пользователя {UserName}", registerDto?.UserName);
                ModelState.AddModelError(string.Empty, "Произошла ошибка при обработке регистрации. Попробуйте позже.");
                return View(registerDto);
            }
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
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден.");
                    return View(loginDto);
                }

                var signResult = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

                if (signResult.IsLockedOut)
                {
                    _logger?.LogWarning("Пользователь{UserId} заблокирован.", user.Id);
                    ModelState.AddModelError(string.Empty, "Ваша учетная запись временно заблокирована. Попробуйте позже.");
                    return View(loginDto);
                }

                if (!signResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Неверные данные для входа.");
                    return View(loginDto);
                }

                await _signInManager.SignInAsync(user, loginDto.RememberMe == true ? true : false);

                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при попытке входа пользователя {Login}", loginDto?.Email);
                ModelState.AddModelError(string.Empty, "Произошла ошибка при входе. Попробуйте позже.");
                return View(loginDto);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
                return View(forgotPasswordDto);

            await _accountService.SendPasswordResetLinkAsync(forgotPasswordDto.Email);

            return Ok();
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return BadRequest("Ошибка сброса пароля");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}