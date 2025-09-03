using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Interfaces;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;

namespace CodeSparkNET.Controllers
{
    [Route("[controller]")]
    public class Accountcontroller : Controller
    {
        private readonly ILogger<Accountcontroller> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public Accountcontroller(ILogger<Accountcontroller> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    ModelState.AddModelError("", "Ошибка создания аккаунта.");

                var user = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                };

                var regResult = await _userManager.CreateAsync(user, registerDto.Password);

                if (regResult.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "User");

                    if (roleResult.Succeeded)
                    {
                        new NewUserDto
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            Tokens = _tokenService.CreateToken(user)
                        };
                        return RedirectToAction("Index", "Home"); // TODO
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ошибка создания аккаунта. Попробуйте еще раз.");
                    }
                }
                ModelState.AddModelError("", "Неправильный логин или пароль");
                return View(registerDto);
            }
            catch (Exception e)
            {
                foreach (var error in e.Message)
                {
                    ModelState.AddModelError("", error.ToString());
                }
                return View(registerDto);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    ModelState.AddModelError("", "Ошибка входа.");

                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
                {
                    await _signInManager.SignInAsync(user, loginDto.RememberMe ?? false);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Неправильный логин или пароль");
                return View(loginDto);
            }
            catch (Exception e)
            {
                foreach (var error in e.Message)
                {
                    ModelState.AddModelError("", error.ToString());
                }
                return View(loginDto);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}