using CodeSparkNET.Data;
using CodeSparkNET.Identity;
using CodeSparkNET.Infrastructure;
using CodeSparkNET.Interfaces.Repositories;
using CodeSparkNET.Interfaces.Services;
using CodeSparkNET.Models;
using CodeSparkNET.Redis;
using CodeSparkNET.Repositories;
using CodeSparkNET.Services;
using CodeSparkNET.Validators.Account;
using CodeSparkNET.Validators.AdminCourse;
using CodeSparkNET.Validators.Catalogs;
using CodeSparkNET.Validators.Profile;
using CodeSparkNET.ViewModels.Account;
using CodeSparkNET.ViewModels.AdminCourse;
using CodeSparkNET.ViewModels.Catalogs;
using CodeSparkNET.ViewModels.Profile;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using StackExchange.Redis;
using System.Globalization;
using System.Threading.RateLimiting;


var builder = WebApplication.CreateBuilder(args);

// resources for localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization() // localize views
    .AddDataAnnotationsLocalization() // localize data annotations
    .AddMvcOptions(options =>
    {
        var mb = options.ModelBindingMessageProvider;
        mb.SetAttemptedValueIsInvalidAccessor((x, name) => "Неверное значение.");
        mb.SetMissingBindRequiredValueAccessor(name => "Не указано обязательное значение.");
        mb.SetMissingKeyOrValueAccessor(() => "Отсутствует значение.");
        mb.SetUnknownValueIsInvalidAccessor(name => "Неверное значение.");
        mb.SetValueMustBeANumberAccessor(name => "Значение должно быть числом.");
        mb.SetValueIsInvalidAccessor(name => "Неверное значение.");
    });

builder.Services.AddHttpContextAccessor();

// Add DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Sql"));
});

// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
}).AddErrorDescriber<RussianIdentityErrorDescriber>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//Sessions
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

//Cookie Policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false; //true for consent with user
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
    // for production
    options.Secure = CookieSecurePolicy.Always;
});

//builder.Services.AddDistributedMemoryCache(); // in debug


builder.Services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(sp =>
{
    var conn = builder.Configuration["Redis:ConnectionString"];
    return StackExchange.Redis.ConnectionMultiplexer.Connect(conn);
});


//Add Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<ICourseService, CourseService>();

//Add repositories
builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

//Add Redis Service
builder.Services.AddScoped<ICacheService, CacheService>();

//Add Redis singleton
builder.Services.AddSingleton<ICacheProvider, CacheProvider>();

//Add Validators
builder.Services.AddFluentValidationAutoValidation(conf =>
{
    conf.DisableBuiltInModelValidation = true;
    conf.ValidationStrategy = SharpGrip.FluentValidation.AutoValidation.Mvc.Enums.ValidationStrategy.All;

    conf.EnableFormBindingSourceAutomaticValidation = true;
    conf.EnableQueryBindingSourceAutomaticValidation = true;
    conf.EnableBodyBindingSourceAutomaticValidation = true;

    conf.OverrideDefaultResultFactoryWith<MvcValidationResultFactory>();

});
//Account validators
builder.Services.AddTransient<MvcValidationResultFactory>();
builder.Services.AddScoped<IValidator<LoginViewModel>, LoginViewModelValidator>();
//builder.Services.AddScoped<IValidator<RegisterViewModel>, RegisterViewModelValidator>();
builder.Services.AddScoped<IValidator<ForgotPasswordViewModel>, ForgotPasswordViewModelValidator>();
builder.Services.AddScoped<IValidator<ResetPasswordViewModel>, ResetPasswordViewModelValidator>();

//AdminCourse validators
builder.Services.AddScoped<IValidator<AddLessonViewModel>, AddLessonViewModelValidator>();
builder.Services.AddScoped<IValidator<AddModuleViewModel>, AddModuleViewModelValidator>();
builder.Services.AddScoped<IValidator<CreateCourseViewModel>, CreateCourseViewModelValidator>();
builder.Services.AddScoped<IValidator<EditCourseViewModel>, EditCourseViewModelValidator>();
builder.Services.AddScoped<IValidator<UpdateLessonViewModel>, UpdateLessonViewModelValidator>();
builder.Services.AddScoped<IValidator<UpdateModuleViewModel>, UpdateModuleViewModelValidator>();

//Catalogs validator
builder.Services.AddScoped<IValidator<CatalogNamesViewModel>, CatalogNamesViewModelValidator>();
builder.Services.AddScoped<IValidator<CatalogProductDetailsViewModel>, CatalogProductDetailsViewModelValidator>();
builder.Services.AddScoped<IValidator<CatalogProductImageViewModel>, CatalogProductImageViewModelValidator>();
builder.Services.AddScoped<IValidator<CatalogProductsViewModel>, CatalogProductsViewModelValidator>();
builder.Services.AddScoped<IValidator<CatalogViewModel>, CatalogViewModelValidator>();

//Profile
builder.Services.AddScoped<IValidator<ChangePasswordViewModel>, ChangePasswordViewModelValidator>();
builder.Services.AddScoped<IValidator<PersonalProfileViewModel>, PersonalProfileViewModelValidator>();
builder.Services.AddScoped<IValidator<UpdatePersonalProfileViewModel>, UpdatePersonalProfileViewModelValidator>();

//Add keys
StackExchange.Redis.ConnectionMultiplexer? redis = null;
try
{
    var cs = builder.Configuration["Redis:ConnectionString"];
    if (!string.IsNullOrWhiteSpace(cs))
        redis = StackExchange.Redis.ConnectionMultiplexer.Connect(cs);
}
catch
{
}

if (redis != null)
{
    builder.Services.AddSingleton(redis);
    builder.Services.AddDataProtection()
           .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys")
           .SetApplicationName("CodeSparkNET");
}
else
{
    builder.Services.AddDataProtection()
           .SetApplicationName("CodeSparkNET");
}

//Custom Rate Limitter
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress.ToString(),
            factory: partion => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 40000,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
            }));

    options.OnRejected = (context, _) =>
    {
        if (context.HttpContext.Response.HasStarted)
            return new ValueTask();

        context.HttpContext.Response.StatusCode = 429;

        context.HttpContext.Response.Redirect("/Error/StatusCode/429");

        return new ValueTask();
    };
});

var app = builder.Build();

// Localization
var supportedCultures = new[] { new CultureInfo("ru"), new CultureInfo("en") };

var requestLocalizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("ru"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseRouting();

app.UseStatusCodePagesWithReExecute("/Error/StatusCode/{0}");

app.UseRateLimiter(); // Enable rate limitting middleware

app.UseStaticFiles();

app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
