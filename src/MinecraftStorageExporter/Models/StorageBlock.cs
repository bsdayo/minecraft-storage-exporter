namespace MinecraftStorageExporter.Models;

public class StorageBlock
{
    public string id { get; set; } = "";

    public StorageItem[] Items { get; set; } = [];
}