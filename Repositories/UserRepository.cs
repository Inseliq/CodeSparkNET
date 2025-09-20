using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Interfaces;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CodeSparkNET.Repositories
{
    /// <summary>
    /// Repository for working with users and authentication/authorization.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<UserRepository> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<AppUser> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _userManager.FindByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting user by email: {Email}", email);
                throw;
            }
        }

        public async Task<AppUser> GetUserByIdAsync(string id)
        {
            try
            {
                return await _userManager.FindByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting user by Id: {Id}", id);
                throw;
            }
        }

        public async Task<AppUser> GetUserByUserNameAsync(string userName)
        {
            try
            {
                return await _userManager.FindByNameAsync(userName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting user by UserName: {UserName}", userName);
                throw;
            }
        }

        public Task<AppUser> GetUserAsync(ClaimsPrincipal user)
        {
            try
            {
                return _userManager.GetUserAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting user from ClaimsPrincipal");
                throw;
            }
        }

        public async Task<IdentityResult> CreateUserAsync(AppUser user, string password)
        {
            try
            {
                return await _userManager.CreateAsync(user, password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating user {UserName}", user.UserName);
                throw;
            }
        }

        public async Task<IdentityResult> UpdateUserAsync(AppUser user)
        {
            try
            {
                return await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating user {UserName}", user.UserName);
                throw;
            }
        }

        public async Task<IdentityResult> DeleteUserAsync(AppUser user)
        {
            try
            {
                return await _userManager.DeleteAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting user {UserName}", user.UserName);
                throw;
            }
        }

        public async Task<IdentityResult> AddToRoleAsync(AppUser user, string role)
        {
            try
            {
                return await _userManager.AddToRoleAsync(user, role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding role {Role} to user {UserName}", role, user.UserName);
                throw;
            }
        }

        public async Task<IList<string>> GetUserRolesAsync(AppUser user)
        {
            try
            {
                return await _userManager.GetRolesAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting roles for user {UserName}", user.UserName);
                throw;
            }
        }

        public async Task<bool> IsEmailConfirmedAsync(AppUser user)
        {
            try
            {
                return await _userManager.IsEmailConfirmedAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking email confirmation for user {UserName}", user.UserName);
                throw;
            }
        }

        public async Task<string> GeneratePasswordResetTokenAsync(AppUser user)
        {
            try
            {
                return await _userManager.GeneratePasswordResetTokenAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while generating password reset token for user {UserName}", user.UserName);
                throw;
            }
        }

        public async Task<IdentityResult> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword)
        {
            try
            {
                return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while changing password for user {UserName}", user.UserName);
                throw;
            }
        }

        public Task<IdentityResult> ResetPasswordAsync(AppUser user, string token, string newPassword)
        {
            _logger.LogWarning("Method ResetPasswordAsync is not implemented yet.");
            throw new NotImplementedException();
        }

        public async Task UpdateSecurityStampAsync(AppUser user)
        {
            try
            {
                await _userManager.UpdateSecurityStampAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating security stamp for user {UserName}", user.UserName);
                throw;
            }
        }

        public async Task<IdentityResult> ConfirmEmailAsync(AppUser user, string token)
        {
            try
            {
                return await _userManager.ConfirmEmailAsync(user, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while confirming email for user {UserName}", user.UserName);
                throw;
            }
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(AppUser user)
        {
            try
            {
                return await _userManager.GenerateEmailConfirmationTokenAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while generating email confirmation token for user {UserName}", user.UserName);
                throw;
            }
        }

        public async Task<SignInResult> PasswordSignInAsync(LoginDto model)
        {
            try
            {
                var user = await GetUserByEmailAsync(model.Email);
                return await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while signing in user {Email}", model.Email);
                throw;
            }
        }

        public async Task RefreshSignInAsync(AppUser user)
        {
            try
            {
                await _signInManager.RefreshSignInAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while refreshing sign-in for user {UserName}", user.UserName);
                throw;
            }
        }

        public async Task SignOutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while signing out user.");
                throw;
            }
        }

        public async Task<IList<string>> GetUserRolesByEmailAsync(string userEmail)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                return await _userManager.GetRolesAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting roles by email: {Email}", userEmail);
                throw;
            }
        }
    }
}
