using System.Net;

namespace MinecraftStorageExporter.Options;

public class RconOptions
{
    public string Host { get; set; } = "localhost";

    public ushort Port { get; set; } = 25575;

    public string? Password { get; set; }
}