using ClinicServiceDAL;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;
using System.Text;
using WS_ClinicService.Core.Auth;
using WS_ClinicService.Core.Extensions;
using WS_ClinicService.Core.Filters;
using WS_ClinicService.Mapping;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers(conf =>
{
    conf.Filters.Add(typeof(ApiExceptions));
    conf.Filters.Add(typeof(ValidationFilter));
}).AddNewtonsoftJson(x =>
{
    x.SerializerSettings.Converters.Add(new StringEnumConverter());
    x.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    x.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
}).ConfigureApiBehaviorOptions(y =>
{
    y.SuppressModelStateInvalidFilter = false;
    y.InvalidModelStateResponseFactory = c =>
    {
        return new UnprocessableEntityObjectResult(c.ModelState);
    };
});

services.AddClinicDAL(configuration);

services.AddAutoMapper(cfg => { }, typeof(ClinicMappingProfile));

services.AddValidatorsFromAssembly(typeof(Program).Assembly);

services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();

if (string.IsNullOrWhiteSpace(jwtOptions.Secret) || Encoding.UTF8.GetByteCount(jwtOptions.Secret) < 32)
{
    throw new InvalidOperationException("Jwt:Secret must be configured and contain at least 32 bytes.");
}

if (string.IsNullOrWhiteSpace(jwtOptions.Issuer) || string.IsNullOrWhiteSpace(jwtOptions.Audience))
{
    throw new InvalidOperationException("Jwt:Issuer and Jwt:Audience must be configured.");
}

if (jwtOptions.ExpiresMinutes is <= 0 or > 60)
{
    throw new InvalidOperationException("Jwt:ExpiresMinutes must be between 1 and 60.");
}

services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
services.Configure<AuthBootstrapOptions>(configuration.GetSection("Auth:Bootstrap"));
services.AddSingleton<TokenService>();
services.AddSingleton<IPasswordHasher<ClinicServiceContext.Entities.PersonSnapshot>, PasswordHasher<ClinicServiceContext.Entities.PersonSnapshot>>();
services.AddScoped<DatabaseAuthenticationService>();
services.AddScoped<AuthBootstrapper>();

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
        };
    });

services.AddAuthorization();

services.AddRateLimiter(options =>
{
    options.AddPolicy("login", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));
});

services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapOpenApi("/openapi/{documentName}.yaml");

    app.MapGet("/", () => Results.Redirect("/openapi/v1.yaml"))
        .ExcludeFromDescription();
}

app.MapScalarApiReference(options => options.WithTitle("Clinic Service API"));

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();


if (configuration["Database:AutoMigrate"] == "true")
{
    using var scope = app.Services.CreateScope();
    var provider = (configuration["Database:Provider"] ?? "sqlite").ToLowerInvariant();
    DbContext migrateContext = provider switch
    {
        "mssql" => scope.ServiceProvider.GetRequiredService<MssqlClinicDbContext>(),
        "pgsql" => scope.ServiceProvider.GetRequiredService<PgsqlClinicDbContext>(),
        _ => scope.ServiceProvider.GetRequiredService<SqliteClinicDbContext>()
    };
    migrateContext.Database.Migrate();
}

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<AuthBootstrapper>().SeedAsync();
}

app.MapGroup("v1").MapControllers();

app.Run();