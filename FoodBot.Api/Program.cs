using System.Text.Json.Serialization;
using DiscordService;
using FoodBot.Api.Auth;
using FoodBot.Application;
using FoodBot.Application.Common;
using FoodBot.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
var isDevelopment = builder.Environment.EnvironmentName == "Development";

builder.Services.Configure<AppOptions>(builder.Configuration);
builder.Services.Configure<DiscordOptions>(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserProvider, UserProvider>();

builder.Services.AddTransient<ApiKeyValidator>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = FoodBotAuthScheme.DefaultScheme;
        options.DefaultChallengeScheme = FoodBotAuthScheme.DefaultScheme;
    })
    .AddPolicyScheme(FoodBotAuthScheme.DefaultScheme, FoodBotAuthScheme.DisplayName, policy =>
    {
        policy.ForwardDefaultSelector = context =>
        {
            var authHeader = context.Request.Headers.Authorization.ToString();
            var apiKeyHeader = context.Request.Headers[FoodBotAuthScheme.ApiKeySchemeHeader].ToString();

            if (!string.IsNullOrWhiteSpace(apiKeyHeader) || !string.IsNullOrWhiteSpace(authHeader) &&
                authHeader.StartsWith(FoodBotAuthScheme.ApiKeyPrefix, StringComparison.OrdinalIgnoreCase))
                return FoodBotAuthScheme.ApiKeyScheme;

            return CookieAuthenticationDefaults.AuthenticationScheme;
        };
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = null;
        options.ReturnUrlParameter = "return-url";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;

        options.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = isDevelopment ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
        };
    })
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
        FoodBotAuthScheme.ApiKeyScheme,
        options =>
        {
            options.DefaultScheme = FoodBotAuthScheme.ApiKeyScheme;
            options.ApiKeySchemeHeader = FoodBotAuthScheme.ApiKeySchemeHeader;
            options.ApiKeyPrefix = FoodBotAuthScheme.ApiKeyPrefix;
        })
    ;

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services
    .AddInfrastructure()
    .AddApplication()
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.AllowTrailingCommas = true;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();
var allowedOrigins = app.Services.GetRequiredService<IConfiguration>().GetSection("AllowedOrigins").Get<string[]>() ?? [];
app.UseCors(options => options
    .WithOrigins(allowedOrigins)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Services.ApplyMigrations();

app.Run();