using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Interfaces;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace CodeSparkNET.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AccountService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IEmailService emailService,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto dto, string loginLink)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return IdentityResult.Failed(new IdentityError { Description = "Пользователь с таким email уже зарегистрирован." });

            var user = new AppUser
            {
                UserName = dto.UserName,
                Email = dto.Email
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
                return createResult;

            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return IdentityResult.Failed(new IdentityError { Description = "Ошибка создания аккаунта." });
            }


            if (!string.IsNullOrEmpty(loginLink))
            {
                await _emailService.SendAccountCratedEmailAsync(user.Email!, user.UserName, loginLink);
            }

            return IdentityResult.Success;
        }

        public async Task<SignInResult> PasswordSignInAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return SignInResult.Failed;

            return await _signInManager.PasswordSignInAsync(user.UserName, dto.Password, dto.RememberMe, lockoutOnFailure: false);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }


        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            // Find the user associated with the provided email
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            // If user not found, return a generic failure
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "Invalid request." });

            var decodedBytes = WebEncoders.Base64UrlDecode(resetPasswordDto.Token);
            var decodedToken = Encoding.UTF8.GetString(decodedBytes);

            // Attempt reset user password
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.Password);

            // If successful, update the Security Stamp to invalidate any active sessions or tokens
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                return result;
            }

            return result;
        }

        public async Task<bool> SendPasswordResetLinkAsync(string email)
        {
            //Get user
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || (await _userManager.IsEmailConfirmedAsync(user))) //TODO: add inverse(add !) in email.IsConfirmed
                return false;

            //Generate unique token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            //Encoded token
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            //Constructing reset url link
            var baseUrl = _configuration["AppSettings:BaseUrl"]?.TrimEnd('/');
            var emailEscaped = System.Net.WebUtility.UrlEncode(user.Email);
            var resetLink = $"{baseUrl}/Account/ResetPassword/?email={emailEscaped}&token={encodedToken}";

            await _emailService.SendResetPasswordEmailAsync(user.Email!, user.UserName, resetLink);

            return true;
        }
    }
}