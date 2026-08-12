using Autofac.Extensions.DependencyInjection;
using WS_ClinicService.Core.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllers(conf =>
{
    conf.EnableEndpointRouting = false;
    conf.Filters.Add(typeof(ApiExceptions));
}).AddNewtonsoftJson(x =>
{
    x.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
    x.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
}).ConfigureApiBehaviorOptions(y =>
{
    y.SuppressModelStateInvalidFilter = false;
    y.InvalidModelStateResponseFactory = c =>
    {
        return new UnprocessableEntityObjectResult(c.ModelState);
    };
});


builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
