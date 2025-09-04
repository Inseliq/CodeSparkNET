using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Interfaces;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace CodeSparkNET.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AccountService(UserManager<AppUser> userManager, IEmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _emailService = emailService;
            _configuration = configuration;
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
            var result = await _userManager.ChangePasswordAsync(user, decodedToken, resetPasswordDto.Password);

            // If successful, update the Security Stamp to invalidate any active sessions or tokens
            if (result.Succeeded)
                return await _userManager.UpdateSecurityStampAsync(user);

            return result;
        }

        public async Task<bool> SendPasswordResetLinkAsync(string email)
        {
            //Get User
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || (!await _userManager.IsEmailConfirmedAsync(user)))
                return false;

            //Generate unique token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            //Encoded token
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            //Constructing reset url link
            var baseUrl = _configuration["AppSettings:BaseUrl"];
            var resetLink = $"{baseUrl}/Account/ResetPassword/?email={user.Email}&token={encodedToken}";

            await _emailService.SendResetPasswordEmailAsync(user.Email!, user.UserName, resetLink);

            return true;
        }
    }
}