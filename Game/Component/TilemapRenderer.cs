using System.Numerics;
using Engine;
using Raylib_cs;

class TilemapRenderer : Component
{
    private Tilemap? _tilemap;
    private Transform? _transform;

    public Color Tint { get; set; } = Color.White;

    public override void Load()
    {
        _tilemap = Entity?.GetComponent<Tilemap>();
        _transform = Entity?.GetComponent<Transform>();
    }

    public override void Draw()
    {
        if (_transform == null || _tilemap == null)
            return;

        if (!TextureManager.HasTexture(_tilemap.TextureName))
            return;

        Texture2D texture = TextureManager.GetTexture(_tilemap.TextureName);
        int columns = Math.Max(1, texture.Width / (int)_tilemap.CellSize.X);
        Vector2 topLeft = _tilemap.GetTopLeft();

        for (int y = 0; y < _tilemap.Cells.GetLength(0); y++)
        {
            for (int x = 0; x < _tilemap.Cells.GetLength(1); x++)
            {
                int cellIndex = _tilemap.Cells[y, x];
                if (cellIndex <= 0)
                    continue;

                Rectangle source = GetSourceRectangle(columns, cellIndex);
                Rectangle destination = new(
                    topLeft.X + (x * _tilemap.CellSize.X),
                    topLeft.Y + (y * _tilemap.CellSize.Y),
                    _tilemap.CellSize.X,
                    _tilemap.CellSize.Y
                );

                Renderer.DrawTexture(
                    texture,
                    source,
                    destination,
                    Vector2.Zero,
                    _transform.Rotation,
                    Tint,
                    InstructionSource.Entity,
                    _transform.ZLayer
                );
            }
        }
    }

    private Rectangle GetSourceRectangle(int columns, int cellIndex)
    {
        if (_tilemap == null)
            throw new InvalidOperationException("TilemapRenderer requires a Tilemap component.");

        int tileIndex = cellIndex - 1;
        int sourceX = tileIndex % columns;
        int sourceY = tileIndex / columns;

        return new Rectangle(
            sourceX * _tilemap.CellSize.X,
            sourceY * _tilemap.CellSize.Y,
            _tilemap.CellSize.X,
            _tilemap.CellSize.Y
        );
    }
}
