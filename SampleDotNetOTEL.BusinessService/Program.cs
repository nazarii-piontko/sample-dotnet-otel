using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SampleDotNetOTEL.BusinessService.Controllers;
using SampleDotNetOTEL.BusinessService.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton(new ErrorResponsePolicy(builder.Configuration.GetValue<double>("ErrorRate")));
builder.Services.AddDbContext<WeatherDbContext>(b => b.UseNpgsql(builder.Configuration["ConnectionStrings:Default"]));
builder.Services.AddTransient<WeatherDbInitializer>();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(b =>
    {
        b.AddService(builder.Configuration["ServiceName"]!);
    })
    .WithTracing(b => b
        .AddAspNetCoreInstrumentation(o =>
        {
            o.Filter = ctx => ctx.Request.Path != "/metrics";
        })
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddOtlpExporter())
    .WithMetrics(b => b
        .AddAspNetCoreInstrumentation(o =>
        {
            o.Filter = (_, ctx) => ctx.Request.Path != "/metrics";
        })
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddPrometheusExporter())
    .StartWithHost();

var app = builder.Build();

using var scope = app.Services.CreateScope();
{
    var dbSeeder = scope.ServiceProvider.GetRequiredService<WeatherDbInitializer>();
    await dbSeeder.InitAsync();
}

app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.UseDeveloperExceptionPage();
app.MapControllers();
app.Run();
