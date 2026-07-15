using System.Text;
using Bookstage.Api.Infrastructure.Data;
using Bookstage.Api.Infrastructure.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Bookstage.Api.Services;

var builder = WebApplication.CreateBuilder(args);
var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(defaultConnectionString) || defaultConnectionString.Contains("REPLACE_WITH_SUPABASE_DB_PASSWORD", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException("Set ConnectionStrings__DefaultConnection to your Supabase PostgreSQL connection string before starting the API.");
}

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Contains("REPLACE_WITH_A_LONG_RANDOM_SECRET_KEY", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException("Set Jwt__Key before starting the API.");
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new InvalidOperationException("Set Jwt__Issuer before starting the API.");
}

var jwtAudience = builder.Configuration["Jwt:Audience"];
if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException("Set Jwt__Audience before starting the API.");
}

static string[] GetAllowedOrigins(IConfiguration configuration)
{
    var sectionOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?.Where(origin => !string.IsNullOrWhiteSpace(origin))
        .Select(origin => origin.Trim())
        .ToArray();

    if (sectionOrigins is { Length: > 0 })
    {
        return sectionOrigins;
    }

    var csvOrigins = configuration["Cors:AllowedOrigins"];
    if (string.IsNullOrWhiteSpace(csvOrigins))
    {
        return Array.Empty<string>();
    }

    return csvOrigins
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Where(origin => !string.IsNullOrWhiteSpace(origin))
        .ToArray();
}

var allowedOrigins = GetAllowedOrigins(builder.Configuration);
if (!builder.Environment.IsDevelopment() && allowedOrigins.Length == 0)
{
    throw new InvalidOperationException("Set Cors__AllowedOrigins before starting the API in Production.");
}

var applyMigrationsOnStartup = builder.Configuration.GetValue("Database:ApplyMigrationsOnStartup", true);
var seedDataOnStartup = builder.Configuration.GetValue("Database:SeedDataOnStartup", builder.Environment.IsDevelopment());

var renderPort = builder.Configuration["PORT"] ?? Environment.GetEnvironmentVariable("PORT") ?? "10000";
builder.WebHost.UseUrls($"http://0.0.0.0:{renderPort}");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi();
}

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");

builder.Services.AddDbContext<BookstageDbContext>(options =>
{
    options.UseNpgsql(
        defaultConnectionString,
        npgsql => npgsql.EnableRetryOnFailure());
});

builder.Services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();
builder.Services.AddScoped<ITokenService, TokenService>();

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins);
        }
        else if (builder.Environment.IsDevelopment())
        {
            policy.WithOrigins("http://localhost:5173");
        }

        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

if (applyMigrationsOnStartup || seedDataOnStartup)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BookstageDbContext>();

        if (applyMigrationsOnStartup)
        {
            context.Database.Migrate();
        }

        if (seedDataOnStartup)
        {
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<object>>();
            await DataSeeder.SeedDataAsync(context, passwordHasher);
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogCritical(ex, "Failed to initialize the database during startup.");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowFrontend");

app.MapHealthChecks("/health", new() { ResponseWriter = static (context, report) =>
{
    context.Response.ContentType = "application/json";
    return context.Response.WriteAsync($"{{\"status\":\"{report.Status}\"}}");
} });

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();

app.Run();
