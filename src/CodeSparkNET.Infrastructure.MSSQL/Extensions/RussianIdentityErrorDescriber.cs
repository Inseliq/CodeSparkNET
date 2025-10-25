using Microsoft.AspNetCore.Identity;

namespace CodeSparkNET.Infrastructure.Extensions
{
    public class RussianIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError() =>
           new IdentityError { Code = nameof(DefaultError), Description = "Произошла неизвестная ошибка." };

        public override IdentityError ConcurrencyFailure() =>
            new IdentityError { Code = nameof(ConcurrencyFailure), Description = "Ошибка конкурентного доступа к данным. Повторите операцию." };

        public override IdentityError PasswordMismatch() =>
            new IdentityError { Code = nameof(PasswordMismatch), Description = "Неверный пароль." };

        public override IdentityError InvalidToken() =>
            new IdentityError { Code = nameof(InvalidToken), Description = "Недействительный токен." };

        public override IdentityError LoginAlreadyAssociated() =>
            new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "Учетная запись с таким логином уже привязана." };

        public override IdentityError InvalidUserName(string userName) =>
            new IdentityError { Code = nameof(InvalidUserName), Description = $"Неверное имя пользователя: {userName}." };

        public override IdentityError InvalidEmail(string email) =>
            new IdentityError { Code = nameof(InvalidEmail), Description = $"Неверный формат email: {email}." };

        public override IdentityError DuplicateUserName(string userName) =>
            new IdentityError { Code = nameof(DuplicateUserName), Description = "Имя пользователя уже занято." };

        public override IdentityError DuplicateEmail(string email) =>
            new IdentityError { Code = nameof(DuplicateEmail), Description = "Пользователь с таким email уже существует." };

        public override IdentityError InvalidRoleName(string role) =>
            new IdentityError { Code = nameof(InvalidRoleName), Description = $"Недопустимое имя роли: {role}." };

        public override IdentityError DuplicateRoleName(string role) =>
            new IdentityError { Code = nameof(DuplicateRoleName), Description = "Роль с таким именем уже существует." };

        public override IdentityError UserAlreadyHasPassword() =>
            new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "У пользователя уже установлен пароль." };

        public override IdentityError UserLockoutNotEnabled() =>
            new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "Блокировка аккаунта не включена для этого пользователя." };

        public override IdentityError PasswordTooShort(int length) =>
            new IdentityError { Code = nameof(PasswordTooShort), Description = $"Пароль должен содержать не менее {length} символов." };

        public override IdentityError PasswordRequiresNonAlphanumeric() =>
            new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Пароль должен содержать хотя бы один неалфавитно-цифровой символ (например: !@#)." };

        public override IdentityError PasswordRequiresDigit() =>
            new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "Пароль должен содержать хотя бы одну цифру." };

        public override IdentityError PasswordRequiresLower() =>
            new IdentityError { Code = nameof(PasswordRequiresLower), Description = "Пароль должен содержать хотя бы одну строчную букву." };

        public override IdentityError PasswordRequiresUpper() =>
            new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "Пароль должен содержать хотя бы одну заглавную букву." };

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) =>
            new IdentityError { Code = nameof(PasswordRequiresUniqueChars), Description = $"Пароль должен содержать как минимум {uniqueChars} различных символов." };

        public override IdentityError RecoveryCodeRedemptionFailed() =>
            new IdentityError { Code = nameof(RecoveryCodeRedemptionFailed), Description = "Не удалось использовать код восстановления." };

    }
}