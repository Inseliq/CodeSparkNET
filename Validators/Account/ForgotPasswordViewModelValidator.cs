using CodeSparkNET.ViewModels.Account;
using FluentValidation;

namespace CodeSparkNET.Validators.Account
{
    public class ForgotPasswordViewModelValidator : AbstractValidator<ForgotPasswordViewModel>
    {
        public ForgotPasswordViewModelValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Введите email.")
                .EmailAddress().WithMessage("Введите корректный email.");
        }
    }
}
