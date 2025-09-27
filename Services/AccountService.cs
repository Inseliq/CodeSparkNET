using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Dtos.Profile;
using CodeSparkNET.Interfaces.Repositories;
using CodeSparkNET.Interfaces.Services;
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
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            IUserRepository userRepository,
            IEmailService emailService,
            IConfiguration configuration,
            IProductRepository productRepository,
            ILogger<AccountService> logger
            )
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _configuration = configuration;
            _productRepository = productRepository;
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
                    return IdentityResult.Failed(new IdentityError { Description = "Пользователь с такой почтой уже зарегистрирован." });

                if (await _userRepository.GetUserByUserNameAsync(model.UserName) != null)
                    return IdentityResult.Failed(new IdentityError { Description = "Пользователь с таким имененм уже заругистрирован." });

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
        
        public async Task RefrashSignInAsync(AppUser user)
        {
            try
            {
                await _userRepository.RefreshSignInAsync(user); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refrashing claims principal");
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
                    user.EmailConfirmedAt = DateTime.Now;
                    await _userRepository.UpdateUserAsync(user);
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
    
        public async Task<bool> AddCourseToUserAsync(string userId, string courseSlug)
        {
            try
            {
                return await _productRepository.AddCourseToUserAsync(userId, courseSlug);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding course {CourseSlug} to user {UserName}", courseSlug, userId);
                return false;
            }
        }
    
        public async Task<bool> IsCourseAlreadyEnrolled(string userId, string courseSlug)
        {
            try
            {
                return await _productRepository.IsCourseAlreadyEnrolledAsync(userId, courseSlug);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding course {CourseSlug} to user {UserName}", courseSlug, userId);
                return false;
            }
        }
    }
}
