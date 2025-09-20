using System.Globalization;
using System.Net;
using System.Threading.RateLimiting;
using CodeSparkNET.Data;
using CodeSparkNET.Interfaces;
using CodeSparkNET.Models;
using CodeSparkNET.Redis;
using CodeSparkNET.Repositories;
using CodeSparkNET.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;


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
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
}).AddErrorDescriber<IdentityErrorDescriber>()
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

// builder.Services.AddDistributedMemoryCache(); // in prod — Redis or SQL
builder.Services.AddDistributedMemoryCache(); // in debug

//Add Redis
// builder.Services.AddStackExchangeRedisCache(options =>
// {
//     options.Configuration = builder.Configuration.GetConnectionString("Redis");
//     options.InstanceName = "CodeSparkNET:";
// });

//Add Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IProfileService, AccountService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();

//Add repositories
builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

//Add Redis Service
// builder.Services.AddScoped<ICacheService, CacheService>();

//Add Redis singleton
// builder.Services.AddSingleton<ICacheProvider, CacheProvider>();

//Add keys
// var redis = ConnectionMultiplexer.Connect(builder.Configuration["REDIS_CONNECTION"]);
// builder.Services.AddDataProtection()
//     .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys")
//     .SetApplicationName("CodeSparkNET");

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
