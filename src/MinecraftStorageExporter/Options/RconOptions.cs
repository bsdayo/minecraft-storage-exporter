using System.Net;

namespace MinecraftStorageExporter.Options;

public class RconOptions
{
    public IPAddress Host { get; set; } = IPAddress.Parse("127.0.0.1");

    public ushort Port { get; set; } = 25575;

    public string? Password { get; set; }
}