using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SampleDotNetOTEL.ProxyService.ExternalServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient<BusinessServiceClient>(c =>
{
    var urls = builder.Configuration["BusinessServiceBaseUrl"]!.Split(';', StringSplitOptions.RemoveEmptyEntries);
    c.BaseAddress = new Uri(urls[Random.Shared.Next(urls.Length)]);
});

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

app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.UseDeveloperExceptionPage();
app.MapControllers();
app.Run();