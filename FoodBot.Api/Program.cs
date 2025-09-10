using System.Text.Json.Serialization;
using FoodBot.Api.Auth;
using FoodBot.Application;
using FoodBot.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

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
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                
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
    .ConfigureInfrastructure()
    .ConfigureApplication()
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.AllowTrailingCommas = true;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();

app.UseCors(options => options
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Services.ApplyMigrations();

app.Run();