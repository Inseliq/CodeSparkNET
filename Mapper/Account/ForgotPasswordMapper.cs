using CodeSparkNET.Dtos.Account;
using CodeSparkNET.ViewModels.Account;

namespace CodeSparkNET.Mapper.Account
{
    public static class ForgotPasswordMappings
    {
        public static ForgotPasswordDto ToDto(this ForgotPasswordViewModel vm)
        {
            if (vm == null) return null;
            return new ForgotPasswordDto
            {
                Email = vm.Email?.Trim().ToLowerInvariant()
            };
        }
    }
}
