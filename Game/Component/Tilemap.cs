using System.Numerics;
using System.Xml.Linq;
using Engine;

public class Tilemap : Component
{
    private TilemapCollider? _tilemapCollider;

    public string TextureName { get; set; }
    public Vector2 CellSize { get; set; }
    public int[,] Cells { get; }
    public bool HasCollider => _tilemapCollider != null;
    public int Width => Cells.GetLength(1);
    public int Height => Cells.GetLength(0);
    public Vector2 Size => new(Width * CellSize.X, Height * CellSize.Y);
    public Transform? Transform => Entity?.GetComponent<Transform>();

    public Tilemap(string textureName, string tilemapName, Vector2 cellSize)
        : this(textureName, cellSize, TilemapManager.GetTilemap(tilemapName)) { }

    public Tilemap(string textureName, Vector2 cellSize, int[,] cells)
    {
        TextureName = textureName;
        CellSize = cellSize;
        Cells = cells;
    }

    public override void Load()
    {
        _tilemapCollider = Entity?.GetComponent<TilemapCollider>();
    }

    public int GetCell(int x, int y)
    {
        if (y < 0 || y >= Cells.GetLength(0) || x < 0 || x >= Cells.GetLength(1))
            return 0;

        return Cells[y, x];
    }

    public void SetCell(int x, int y, int cellIndex)
    {
        if (y < 0 || y >= Cells.GetLength(0) || x < 0 || x >= Cells.GetLength(1))
            return;

        Cells[y, x] = cellIndex;
        _tilemapCollider?.Rebuild();
    }

    public Vector2 GetTopLeft()
    {
        return (Transform?.Position ?? Vector2.Zero) - (Size * 0.5f);
    }

    public static int[,] CreateCells(int width, int height, int defaultCell = 0)
    {
        int[,] cells = new int[height, width];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                cells[y, x] = defaultCell;

        return cells;
    }

    public static int[,] LoadCells(string path)
    {
        using FileStream file = File.OpenRead(path);
        return LoadCells(file);
    }

    public static int[,] LoadCells(Stream stream)
    {
        XElement data = XDocument.Load(stream).Descendants("data").First();
        string csv = data.Value;

        string[] rows = csv
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        int height = rows.Length;
        int width = rows[0]
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Length;

        int[,] cells = new int[height, width];

        for (int y = 0; y < height; y++)
        {
            string[] values = rows[y]
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            for (int x = 0; x < width; x++)
                cells[y, x] = int.Parse(values[x]);
        }

        return cells;
    }
}
