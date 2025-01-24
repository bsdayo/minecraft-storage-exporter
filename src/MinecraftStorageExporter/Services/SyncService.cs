using System.Diagnostics.Metrics;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinecraftStorageExporter.Options;

namespace MinecraftStorageExporter.Services;

public sealed class SyncService(
    RconClient client,
    IOptions<SyncOptions> syncOptions,
    IOptions<List<StorageOptions>> storageOptions,
    ILogger<SyncService> logger) : BackgroundService
{
    public static Meter Meter { get; } = new("MinecraftStorageExporter",
        typeof(Program).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version);

    private readonly Gauge<int> _gauge = Meter.CreateGauge<int>(
        name: "minecraft.storage.items",
        description: "Number of items in storage");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Start syncing storage data");

            foreach (var storage in storageOptions.Value)
            {
                logger.LogInformation("Syncing storage {Storage}", storage.Name);

                var sumDict = new Dictionary<string, int>();

                foreach (var block in storage.Blocks)
                {
                    if (block.Split().Select(int.Parse).ToArray() is not [var x, var y, var z])
                        throw new ArgumentException("Block coordinates must be in the format \"x y z\"");

                    logger.LogInformation("Syncing storage {Storage} block {X} {Y} {Z}", storage.Name, x, y, z);
                    var data = await client.GetStorageBlockAsync(x, y, z);

                    // Sum up the count of each item
                    foreach (var item in data.Items)
                    {
                        sumDict.TryAdd(item.Id, 0);
                        sumDict[item.Id] += item.Count;
                    }
                }

                // Record the count of each item
                foreach (var (itemId, count) in sumDict)
                {
                    _gauge.Record(count,
                        new KeyValuePair<string, object?>("storage", storage.Name),
                        new KeyValuePair<string, object?>("item", itemId));
                }
            }

            logger.LogInformation("End syncing storage data");
            await Task.Delay(syncOptions.Value.Interval, stoppingToken);
        }
    }
}