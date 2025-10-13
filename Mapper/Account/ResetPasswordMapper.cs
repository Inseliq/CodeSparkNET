using CodeSparkNET.Dtos.Account;
using CodeSparkNET.ViewModels.Account;

namespace CodeSparkNET.Mapper.Account
{
    public static class ResetPasswordMapper
    {
        public static ResetPasswordDto ToDto(this ResetPasswordViewModel model)
        {
            if (model == null) return null;
            return new ResetPasswordDto
            {
                Email = model.Email.Trim().ToLowerInvariant(),
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                Token = model.Token
            };
        }
    }
}
