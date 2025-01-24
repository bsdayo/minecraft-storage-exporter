using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using CoreRCON;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinecraftStorageExporter.Models;
using MinecraftStorageExporter.Options;

namespace MinecraftStorageExporter.Services;

public partial class RconClient(IOptions<RconOptions> options, ILogger<RconClient> logger)
{
    private readonly RCON _rcon = new(
        Dns.GetHostAddresses(options.Value.Host)[0],
        options.Value.Port,
        options.Value.Password);

    public async Task<StorageBlock> GetStorageBlockAsync(int x, int y, int z)
    {
        var command = $"data get block {x} {y} {z}";
        logger.LogDebug("Sending command: {Command}", command);
        var response = await _rcon.SendCommandAsync(command);

        // Remove unrelated message
        var snbt = SnbtRegex.Replace(response, "");

        // Quote property names
        var quoted = QuotePropsRegex.Replace(snbt, "\"$1\"");

        // Remove number suffix (eg. 12b)
        var json = RemoveNumberSuffixRegex.Replace(quoted, "$1");

        return JsonSerializer.Deserialize<StorageBlock>(json, JsonSerializerOptions.Web)!;
    }

    [GeneratedRegex(".*has the following block data: (?={.*})")]
    private partial Regex SnbtRegex { get; }

    [GeneratedRegex(@"([a-zA-Z]+)(?=:\s)")]
    private partial Regex QuotePropsRegex { get; }

    [GeneratedRegex(@"(\d+)[a-zA-Z]")]
    private partial Regex RemoveNumberSuffixRegex { get; }
}