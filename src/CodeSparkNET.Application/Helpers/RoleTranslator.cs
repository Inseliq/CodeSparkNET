namespace CodeSparkNET.Application.Helpers
{
    public static class RoleTranslatorExtensions
    {
        private static readonly Dictionary<string, string> _map = new()
        {
            ["User"]  = "Пользователь",
            ["Admin"] = "Администратор",
            ["Prime"] = "Премиум",
        };

        public static string ToRussian(this string? role)
            => string.IsNullOrWhiteSpace(role) ? string.Empty
               : (_map.TryGetValue(role!, out var rus) ? rus : role!);

        public static IEnumerable<string> ToRussian(this IEnumerable<string>? roles)
            => (roles ?? Enumerable.Empty<string>()).Select(r => r.ToRussian());

        public static List<string> ToRussianList(this IEnumerable<string>? roles)
            => roles.ToRussian().ToList();

        public static List<string> ToRussian(this List<string>? roles)
            => (roles ?? new List<string>()).Select(r => r.ToRussian()).ToList();

        public static string ToRussianString(this IList<string> rolesTask, string separator = ", ")
        {
            var roles = (rolesTask ?? (IList<string>)Array.Empty<string>());
            return string.Join(separator, (roles ?? Enumerable.Empty<string>()).Select(r => r.ToRussian()));
        }

        public static async Task<List<string>> ToRussianListAsync(this Task<IList<string>> rolesTask)
        {
            var roles = await (rolesTask ?? Task.FromResult((IList<string>)Array.Empty<string>()));
            return (roles ?? Enumerable.Empty<string>()).Select(r => r.ToRussian()).ToList();
        }

        public static async Task<List<string>> ToRussianListAsync(this Task<IEnumerable<string>> rolesTask)
        {
            var roles = await (rolesTask ?? Task.FromResult(Enumerable.Empty<string>()));
            return (roles ?? Enumerable.Empty<string>()).Select(r => r.ToRussian()).ToList();
        }

        public static async Task<List<string>> ToRussianListAsync(this Task<List<string>> rolesTask)
        {
            var roles = await (rolesTask ?? Task.FromResult(new List<string>()));
            return (roles ?? new List<string>()).Select(r => r.ToRussian()).ToList();
        }

        public static async Task<string> ToRussianStringAsync(this Task<IList<string>> rolesTask, string separator = ", ")
        {
            var roles = await (rolesTask ?? Task.FromResult((IList<string>)Array.Empty<string>()));
            return string.Join(separator, (roles ?? Enumerable.Empty<string>()).Select(r => r.ToRussian()));
        }

        public static async Task<string> ToRussianStringAsync(this Task<IEnumerable<string>> rolesTask, string separator = ", ")
        {
            var roles = await (rolesTask ?? Task.FromResult(Enumerable.Empty<string>()));
            return string.Join(separator, (roles ?? Enumerable.Empty<string>()).Select(r => r.ToRussian()));
        }

        public static async Task<string> ToRussianStringAsync(this Task<List<string>> rolesTask, string separator = ", ")
        {
            var roles = await (rolesTask ?? Task.FromResult(new List<string>()));
            return string.Join(separator, (roles ?? new List<string>()).Select(r => r.ToRussian()));
        }
    }
}
