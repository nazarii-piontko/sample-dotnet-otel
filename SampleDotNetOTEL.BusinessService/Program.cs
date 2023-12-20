using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SampleDotNetOTEL.BusinessService.Controllers;
using SampleDotNetOTEL.BusinessService.Persistence;
using SampleDotNetOTEL.BusinessService.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.Logging
    .AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;

        var resBuilder = ResourceBuilder.CreateDefault();
        var serviceName = builder.Configuration["ServiceName"]!;
        resBuilder.AddService(serviceName);
        options.SetResourceBuilder(resBuilder);

        options.AddOtlpExporter();
    });

builder.Services.AddControllers();

builder.Services.AddHttpLogging(o => o.LoggingFields = HttpLoggingFields.All);

builder.Services.AddSingleton(new ErrorResponsePolicy(builder.Configuration.GetValue<double>("ErrorRate")));

builder.Services.AddDbContext<WeatherDbContext>(b => b.UseNpgsql(builder.Configuration["ConnectionStrings:Default"]));
builder.Services.AddTransient<WeatherDbInitializer>();

builder.Services.AddHostedService<MessagesProcessingBackgroundService>();

builder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(options =>
{
    // Filter out instrumentation of the Prometheus scraping endpoint.
    options.Filter = ctx => ctx.Request.Path != "/metrics";
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(b =>
    {
        b.AddService(builder.Configuration["ServiceName"]!);
    })
    .WithTracing(b => b
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddSource(MessagesProcessingBackgroundService.TraceActivityName)
        .AddOtlpExporter())
    .WithMetrics(b => b
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddPrometheusExporter());

var app = builder.Build();

using var scope = app.Services.CreateScope();
{
    var dbSeeder = scope.ServiceProvider.GetRequiredService<WeatherDbInitializer>();
    await dbSeeder.InitAsync();
}

app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.UseHttpLogging();
app.UseDeveloperExceptionPage();
app.MapControllers();
app.Run();
