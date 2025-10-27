using CodeSparkNET.Application.Services.Cache;
using CodeSparkNET.Application.Services.Catalogs;
using CodeSparkNET.Application.Services.Common.Email;
using CodeSparkNET.Application.Services.Courses;
using CodeSparkNET.Application.Services.Templates;
using CodeSparkNET.Application.Services.User;
using Microsoft.Extensions.DependencyInjection;

namespace CodeSparkNET.Application
{
    public static class ServiceRegistry
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddServices();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddCache()
                    .AddCatalog()
                    .AddProducts()
                    .AddUser();

            return services;
        }

        private static IServiceCollection AddCache(this IServiceCollection services)
        {
            services.AddScoped<ICacheService, CacheService>();

            return services;
        }

        private static IServiceCollection AddCatalog(this IServiceCollection services)
        {
            services.AddScoped<ICatalogService, CatalogService>();

            return services;
        }

        private static IServiceCollection AddProducts(this IServiceCollection services)
        {
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ITemplateService, TemplateService>();

            return services;
        }

        private static IServiceCollection AddUser(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProfileService, ProfileService>();

            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
