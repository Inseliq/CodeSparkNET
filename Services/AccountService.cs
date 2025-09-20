using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Dtos.Profile;
using CodeSparkNET.Interfaces;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;

namespace CodeSparkNET.Services
{
    /// <summary>
    /// Service responsible for handling user accounts (registration, authentication, profile, password reset, email confirmation).
    /// </summary>
    public class AccountService : IAccountService, IProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            IUserRepository userRepository,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<AccountService> logger
            )
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user with email, username, and password.
        /// </summary>
        public async Task<IdentityResult> RegisterAsync(RegisterDto model, string loginLink)
        {
            try
            {
                if (await _userRepository.GetUserByEmailAsync(model.Email) != null)
                    return IdentityResult.Failed(new IdentityError { Description = "A user with this email already exists." });

                if (await _userRepository.GetUserByUserNameAsync(model.UserName) != null)
                    return IdentityResult.Failed(new IdentityError { Description = "A user with this username already exists." });

                var user = new AppUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    EmailMarketingConsent = model.ConfirmAd,
                    EmailAddAt = DateTime.UtcNow,
                    EmailChangedAt = DateTime.MinValue,
                    EmailConfirmedAt = DateTime.MinValue
                };

                var createResult = await _userRepository.CreateUserAsync(user, model.Password);
                if (!createResult.Succeeded)
                    return createResult;

                var roleResult = await _userRepository.AddToRoleAsync(user, "User");
                if (!roleResult.Succeeded)
                {
                    await _userRepository.DeleteUserAsync(user);
                    return IdentityResult.Failed(new IdentityError { Description = "Account creation failed." });
                }

                if (!string.IsNullOrEmpty(loginLink))
                {
                    await _emailService.SendAccountCratedEmailAsync(user.Email!, user.UserName, loginLink);
                }

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering user {Email}", model.Email);
                throw;
            }
        }

        /// <summary>
        /// Signs in a user with email and password.
        /// </summary>
        public async Task<SignInResult> PasswordSignInAsync(LoginDto model)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(model.Email);
                if (user == null)
                    return SignInResult.Failed;

                return await _userRepository.PasswordSignInAsync(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while signing in user {Email}", model.Email);
                throw;
            }
        }

        /// <summary>
        /// Signs out the currently logged-in user.
        /// </summary>
        public async Task SignOutAsync()
        {
            try
            {
                await _userRepository.SignOutAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while signing out.");
                throw;
            }
        }

        /// <summary>
        /// Returns roles of the user.
        /// </summary>
        public async Task<IList<string>> GetRolesAsync(AppUser user)
        {
            try
            {
                return await _userRepository.GetUserRolesAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting roles for user {UserName}", user.UserName);
                throw;
            }
        }

        /// <summary>
        /// Sends a password reset link to the user's email.
        /// </summary>
        public async Task<bool> SendPasswordResetLinkAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user == null || (!await _userRepository.IsEmailConfirmedAsync(user)))
                    return false;

                var token = await _userRepository.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var baseUrl = _configuration["AppSettings:BaseUrl"]?.TrimEnd('/');
                var emailEscaped = System.Net.WebUtility.UrlEncode(user.Email);
                var resetLink = $"{baseUrl}/Account/ResetPassword/?email={emailEscaped}&token={encodedToken}";

                await _emailService.SendResetPasswordEmailAsync(user.Email!, user.UserName, resetLink);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending password reset link to {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Resets the user's password.
        /// </summary>
        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto model)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(model.Email);
                if (user is null)
                    return IdentityResult.Failed(new IdentityError { Description = "Invalid request." });

                var decodedBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedBytes);

                var result = await _userRepository.ResetPasswordAsync(user, decodedToken, model.Password);

                if (result.Succeeded)
                {
                    await _userRepository.UpdateSecurityStampAsync(user);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while resetting password for {Email}", model.Email);
                throw;
            }
        }

        /// <summary>
        /// Confirms the user's email using a token.
        /// </summary>
        public async Task<IdentityResult> ConfirmEmailAsync(ConfirmEmailDto model)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(model.Email);
                if (user is null)
                    return IdentityResult.Failed(new IdentityError { Description = "Invalid request." });

                var decodedBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedBytes);

                var result = await _userRepository.ConfirmEmailAsync(user, decodedToken);

                if (result.Succeeded)
                {
                    await _userRepository.UpdateSecurityStampAsync(user);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while confirming email for {Email}", model.Email);
                throw;
            }
        }

        /// <summary>
        /// Gets user from ClaimsPrincipal.
        /// </summary>
        public async Task<AppUser> GetUserAsync(ClaimsPrincipal user)
        {
            try
            {
                return await _userRepository.GetUserAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting user from ClaimsPrincipal");
                throw;
            }
        }

        /// <summary>
        /// Updates user's personal profile.
        /// </summary>
        public async Task<IdentityResult> UpdatePersonalProfileAsync(string email, UpdatePersonalProfileDto model)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user == null && model == null)
                    return null;

                user.UserName = model.UserName;
                user.Email = model.Email;

                return await _userRepository.UpdateUserAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating personal profile for {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Changes the user's password.
        /// </summary>
        public async Task<IdentityResult> ChangePasswordAsync(string email, ChangePasswordDto model)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user == null)
                    return null;

                return await _userRepository.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while changing password for {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Sends email confirmation link.
        /// </summary>
        public async Task<bool> SendEmailConfirmationLinkAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user is null || (await _userRepository.IsEmailConfirmedAsync(user)))
                    return false;

                var token = await _userRepository.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var baseUrl = _configuration["AppSettings:BaseUrl"]?.TrimEnd('/');
                var emailEscaped = System.Net.WebUtility.UrlEncode(user.Email);
                var confirmationLink = $"{baseUrl}/Profile/ConfirmEmail/?email={emailEscaped}&token={encodedToken}";

                await _emailService.SendEmailConfirmationAsync(user.Email!, user.UserName, confirmationLink);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while sending email confirmation link to {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Updates claims for the user (refreshes sign-in).
        /// </summary>
        public async Task UpdateUserClaims(AppUser user)
        {
            try
            {
                await _userRepository.RefreshSignInAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating claims for {UserName}", user.UserName);
                throw;
            }
        }

        /// <summary>
        /// Checks if a user exists by email.
        /// </summary>
        public async Task<bool> UserWithEmailExistsAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(email);
                return user != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking if user exists with email {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Checks if a user exists by username.
        /// </summary>
        public async Task<bool> UserWithUserNameExistsAsync(string userName)
        {
            try
            {
                var user = await _userRepository.GetUserByUserNameAsync(userName);
                return user != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking if user exists with username {UserName}", userName);
                throw;
            }
        }
    }
}
