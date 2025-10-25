using CodeSparkNET.Application.Services.Cache;
using CodeSparkNET.Infrastructure.Repositories.Product;
using CodeSparkNET.Infrastructure.Repositories.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Microsoft.AspNetCore.DataProtection;
using CodeSparkNET.Infrastructure.Options;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using CodeSparkNET.Application.Services.Common.Email;
using CodeSparkNET.Application.Services.Common;
using CodeSparkNET.Application.Services;
using CodeSparkNET.Infrastructure.Redis;
using CodeSparkNET.Infrastructure.Repositories.Catalogs;
using CodeSparkNET.Application.Services.User;

namespace CodeSparkNET.Infrastructure
{
    public static class ServiceRegistry
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<AppDbContext>()
                    .AddRepositories()
                    .AddRedis(configuration)
                    .AddDataProtectionWithRedis(configuration)
                    .AddNotifications(configuration);

            services.AddTransient<IEmailService, EmailService>();

            return services;
        }

        private static IServiceCollection AddNotifications(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMailKit(configuration);

            return services;
        }

        private static IServiceCollection AddMailKit(this IServiceCollection services, IConfiguration configuration)
        {
            var smtp = configuration.GetSection(nameof(SmtpOptions)).Get<SmtpOptions>();

            services.AddMailKit(optionBuilder =>
            {
                optionBuilder.UseMailKit(new MailKitOptions
                {
                    Server = smtp!.Server,
                    Port = Convert.ToInt32(smtp!.Port),
                    Account = smtp!.Account,
                    Password = smtp!.Password,
                    SenderEmail = smtp!.SenderEmail,
                    SenderName = smtp!.SenderName,
                    Security = true
                });
            });

            return services;
        }

        private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            // Регистрируем IConnectionMultiplexer лениво через фабрику, чтобы не делать Connect сразу при старте контейнера.
            var redisConnString = configuration["Redis:ConnectionString"];
            if (!string.IsNullOrWhiteSpace(redisConnString))
            {
                services.AddSingleton<IConnectionMultiplexer>(sp =>
                {
                    return ConnectionMultiplexer.Connect(redisConnString);
                });
            }

            services.AddScoped<ICacheService, CacheService>();

            services.AddSingleton<ICacheProvider, CacheProvider>();

            return services;
        }

        public static IServiceCollection AddDataProtectionWithRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var sp = services.BuildServiceProvider();
            var redis = sp.GetService<IConnectionMultiplexer>();

            if (redis != null)
            {
                services.AddDataProtection()
                        .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys")
                        .SetApplicationName("CodeSparkNET");
            }
            else
            {
                services.AddDataProtection()
                        .SetApplicationName("CodeSparkNET");
            }

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<ICatalogRepository,  CatalogRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            return services;
        }
    }
}
