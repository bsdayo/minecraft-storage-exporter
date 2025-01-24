namespace MinecraftStorageExporter.Options;

public class SyncOptions
{
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(5);
}