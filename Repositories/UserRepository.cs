using CodeSparkNET.Data;
using CodeSparkNET.Dtos.Account;
using CodeSparkNET.Dtos.User;
using CodeSparkNET.Interfaces;
using CodeSparkNET.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CodeSparkNET.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public UserRepository(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<AppUser> GetUserByEmailAsync(string email) => await _userManager.FindByEmailAsync(email);
        public async Task<AppUser> GetUserByIdAsync(string id) => await _userManager.FindByIdAsync(id);
        public async Task<AppUser> GetUserByUserNameAsync(string userName) => await _userManager.FindByNameAsync(userName);
        public Task<AppUser> GetUserAsync(ClaimsPrincipal user) => _userManager.GetUserAsync(user);
        public async Task<IdentityResult> CreateUserAsync(AppUser user, string password) => await _userManager.CreateAsync(user, password);
        public async Task<IdentityResult> UpdateUserAsync(AppUser user) => await _userManager.UpdateAsync(user);
        public async Task<IdentityResult> DeleteUserAsync(AppUser user) => await _userManager.DeleteAsync(user);
        public async Task<IdentityResult> AddToRoleAsync(AppUser user, string role) => await _userManager.AddToRoleAsync(user, role);
        public async Task<IList<string>> GetUserRolesAsync(AppUser user) => await _userManager.GetRolesAsync(user);
        public async Task<bool> IsEmailConfirmedAsync(AppUser user) => await _userManager.IsEmailConfirmedAsync(user);
        public async Task<string> GeneratePasswordResetTokenAsync(AppUser user) => await _userManager.GeneratePasswordResetTokenAsync(user);
        public async Task<IdentityResult> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword) => await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        public Task<IdentityResult> ResetPasswordAsync(AppUser user, string token, string newPassword) => throw new NotImplementedException();
        public async Task UpdateSecurityStampAsync(AppUser user) => await _userManager.UpdateSecurityStampAsync(user);
        public async Task<IdentityResult> ConfirmEmailAsync(AppUser user, string token) => await _userManager.ConfirmEmailAsync(user, token);
        public async Task<string> GenerateEmailConfirmationTokenAsync(AppUser user) => await _userManager.GenerateEmailConfirmationTokenAsync(user);
        public async Task<SignInResult> PasswordSignInAsync(LoginDto model)
        {
            var user = await GetUserByEmailAsync(model.Email);
            return await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
        }
        public async Task RefreshSignInAsync(AppUser user) => await _signInManager.RefreshSignInAsync(user);
        public async Task SignOutAsync() => await _signInManager.SignOutAsync();
        public async Task<IList<string>> GetUserRolesByEmailAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            
            return await _userManager.GetRolesAsync(user);
        }


    }
}
