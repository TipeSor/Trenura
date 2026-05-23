using Engine;

class TilemapManager
{
    private static readonly Dictionary<string, Func<int[,]>> _tilemaps = [];

    public static void AddTilemap(string name, string path)
    {
        _tilemaps[name] = () => Tilemap.LoadCells(path);
    }

    public static void AddEmbeddedTilemap(string name, string resourceNameOrPath)
    {
        _tilemaps[name] = () =>
        {
            byte[] bytes = Embedded.ReadAsset(resourceNameOrPath);
            using MemoryStream ms = new(bytes);
            return Tilemap.LoadCells(ms);
        };
    }

    public static bool HasTilemap(string name) => _tilemaps.ContainsKey(name);

    public static int[,] GetTilemap(string name)
    {
        if (!_tilemaps.TryGetValue(name, out Func<int[,]>? loader))
            throw new ArgumentException($"Tilemap not found : {name}");

        return loader();
    }
}
