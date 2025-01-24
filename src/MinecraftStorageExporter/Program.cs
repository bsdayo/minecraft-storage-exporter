using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinecraftStorageExporter.Options;
using MinecraftStorageExporter.Services;
using OpenTelemetry.Metrics;

var builder = new HostApplicationBuilder(args);

builder.Services.AddOptions<RconOptions>().BindConfiguration("Rcon");
builder.Services.AddOptions<SyncOptions>().BindConfiguration("Sync");
builder.Services.AddOptions<List<StorageOptions>>().BindConfiguration("Storage");

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddMeter(SyncService.Meter.Name)
        .AddPrometheusHttpListener(opt => opt.UriPrefixes = ["http://*:9464/"]));

builder.Services.AddSingleton<RconClient>();

builder.Services.AddHostedService<SyncService>();

var app = builder.Build();

app.Run();