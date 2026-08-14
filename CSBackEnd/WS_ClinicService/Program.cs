using System.Text;
using ClinicServiceDAL;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
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
    x.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
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

services.AddAutoMapper(cfg => { }, typeof(ApiMappingProfile));

services.AddValidatorsFromAssembly(typeof(Program).Assembly);

services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();

services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
services.Configure<AuthOptions>(configuration.GetSection("Auth"));
services.AddSingleton<TokenService>();

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

services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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

app.MapGroup("v1").MapControllers();

app.Run();