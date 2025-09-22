using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Interfaces.Repositories;
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

        /// <summary>
        /// Gets a user by their email address.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <returns>The user with the specified email, or null if not found.</returns>
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

        /// <summary>
        /// Gets a user by their unique identifier.
        /// </summary>
        /// <param name="id">The user's unique identifier.</param>
        /// <returns>The user with the specified ID, or null if not found.</returns>
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

        /// <summary>
        /// Gets a user by their username.
        /// </summary>
        /// <param name="userName">The user's username.</param>
        /// <returns>The user with the specified username, or null if not found.</returns>
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

        /// <summary>
        /// Gets a user from a ClaimsPrincipal.
        /// </summary>
        /// <param name="user">The ClaimsPrincipal representing the user.</param>
        /// <returns>The user associated with the ClaimsPrincipal, or null if not found.</returns>
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

        /// <summary>
        /// Creates a new user with the specified password.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="password">The password for the user.</param>
        /// <returns>The result of the creation operation.</returns>
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

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>The result of the update operation.</returns>
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

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <returns>The result of the delete operation.</returns>
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

        /// <summary>
        /// Adds a user to a specified role.
        /// </summary>
        /// <param name="user">The user to add to the role.</param>
        /// <param name="role">The role to add the user to.</param>
        /// <returns>The result of the add-to-role operation.</returns>
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

        /// <summary>
        /// Gets the roles assigned to a user.
        /// </summary>
        /// <param name="user">The user whose roles to retrieve.</param>
        /// <returns>A list of role names assigned to the user.</returns>
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

        /// <summary>
        /// Checks if a user's email is confirmed.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <returns>True if the email is confirmed; otherwise, false.</returns>
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

        /// <summary>
        /// Generates a password reset token for a user.
        /// </summary>
        /// <param name="user">The user to generate the token for.</param>
        /// <returns>The password reset token.</returns>
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

        /// <summary>
        /// Changes a user's password.
        /// </summary>
        /// <param name="user">The user whose password to change.</param>
        /// <param name="currentPassword">The current password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>The result of the password change operation.</returns>
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

        /// <summary>
        /// Resets a user's password using a token. (Not implemented)
        /// </summary>
        /// <param name="user">The user whose password to reset.</param>
        /// <param name="token">The password reset token.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>The result of the password reset operation.</returns>
        public Task<IdentityResult> ResetPasswordAsync(AppUser user, string token, string newPassword)
        {
            _logger.LogWarning("Method ResetPasswordAsync is not implemented yet.");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the security stamp for a user.
        /// </summary>
        /// <param name="user">The user whose security stamp to update.</param>
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

        /// <summary>
        /// Confirms a user's email using a token.
        /// </summary>
        /// <param name="user">The user whose email to confirm.</param>
        /// <param name="token">The email confirmation token.</param>
        /// <returns>The result of the email confirmation operation.</returns>
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

        /// <summary>
        /// Generates an email confirmation token for a user.
        /// </summary>
        /// <param name="user">The user to generate the token for.</param>
        /// <returns>The email confirmation token.</returns>
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

        /// <summary>
        /// Signs in a user using their email and password.
        /// </summary>
        /// <param name="model">The login data transfer object containing email, password, and remember me flag.</param>
        /// <returns>The result of the sign-in operation.</returns>
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

        /// <summary>
        /// Refreshes the sign-in session for a user.
        /// </summary>
        /// <param name="user">The user whose sign-in session to refresh.</param>
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

        /// <summary>
        /// Signs out the current user.
        /// </summary>
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

        /// <summary>
        /// Gets the roles assigned to a user by their email address.
        /// </summary>
        /// <param name="userEmail">The user's email address.</param>
        /// <returns>A list of role names assigned to the user.</returns>
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
