using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Dtos.Profile;
using CodeSparkNET.Interfaces;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CodeSparkNET.Services
{
    public class AccountService : IAccountService, IProfileService
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

        public async Task<IdentityResult> RegisterAsync(RegisterDto model, string loginLink)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return IdentityResult.Failed(new IdentityError { Description = "Пользователь с таким email уже зарегистрирован." });

            if (await _userManager.FindByEmailAsync(model.UserName) != null)
                return IdentityResult.Failed(new IdentityError { Description = "Пользователь с таким именем пользователя уже зарегистрирован." });

            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                EmailMarketingConsent = model.ConfirmAd
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
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

        public async Task<SignInResult> PasswordSignInAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return SignInResult.Failed;

            return await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
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

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto model)
        {
            // Find the user associated with the provided email
            var user = await _userManager.FindByEmailAsync(model.Email);
            // If user not found, return a generic failure
            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "Invalid request." });

            var decodedBytes = WebEncoders.Base64UrlDecode(model.Token);
            var decodedToken = Encoding.UTF8.GetString(decodedBytes);

            // Attempt reset user password
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);

            // If successful, update the Security Stamp to invalidate any active sessions or tokens
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                return result;
            }

            return result;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(ConfirmEmailDto model)
        {
            // Find the user associated with the provided email
            var user = await _userManager.FindByEmailAsync(model.Email);
            // If user not found, return a generic failure
            if (user is null)
                return IdentityResult.Failed(new IdentityError { Description = "Invalid request." });

            var decodedBytes = WebEncoders.Base64UrlDecode(model.Token);
            var decodedToken = Encoding.UTF8.GetString(decodedBytes);

            // Attempt to confirm the user's email
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            // If successful, update the Security Stamp to invalidate any active sessions or tokens
            if (result.Succeeded)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                return result;
            }

            return result;
        }

        public async Task<AppUser> GetUserAsync(ClaimsPrincipal user)
        {
            return await _userManager.GetUserAsync(user);
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


        public async Task<IdentityResult> ChangePasswordAsync(string email, ChangePasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return null;

            return await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        }

        public async Task<bool> SendEmailConfirmationLinkAsync(string email)
        {
            //Get user
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null || (await _userManager.IsEmailConfirmedAsync(user)))
                return false;

            //Generate unique token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            //Encoded token
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            //Constructing reset url link
            var baseUrl = _configuration["AppSettings:BaseUrl"]?.TrimEnd('/');
            var emailEscaped = System.Net.WebUtility.UrlEncode(user.Email);
            var confirmationLink = $"{baseUrl}/Profile/ConfirmEmail/?email={emailEscaped}&token={encodedToken}";

            await _emailService.SendEmailConfirmationAsync(user.Email!, user.UserName, confirmationLink);
            return true;
        }

        public async Task UpdateUserClaims(AppUser user)
        {
            await _signInManager.RefreshSignInAsync(user);
        }
    }
}