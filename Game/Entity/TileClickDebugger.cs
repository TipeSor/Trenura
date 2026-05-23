using System.Numerics;
using Engine;
using Raylib_cs;

public class TileClickDebugger : Entity
{
    private readonly Tilemap _tilemap;
    private readonly Color _color;
    private readonly float _borderThickness;
    private readonly int _zLayer;

    private Vector2 _tilePosition;
    private bool _hasTile;

    public TileClickDebugger(Tilemap tilemap, Color? color = null, float borderThickness = 2f, int zLayer = 20)
    {
        _tilemap = tilemap;
        _color = color ?? Color.White;
        _borderThickness = borderThickness;
        _zLayer = zLayer;
    }

    public override void Update(float delta)
    {
        base.Update(delta);

        Vector2? tileCenter = GetHoveredTileCenter();
        if (tileCenter == null)
        {
            _hasTile = false;
            return;
        }

        _tilePosition = tileCenter.Value - (_tilemap.CellSize * 0.5f);
        _hasTile = true;

        if (InputManager.IsMouseButtonPressed(MouseButton.Left))
            Console.WriteLine($"Tile center: {tileCenter.Value}");
    }

    public override void Draw()
    {
        base.Draw();

        if (!_hasTile)
            return;

        Renderer.DrawRectangleLines(
            new Rectangle(_tilePosition, _tilemap.CellSize),
            _borderThickness,
            _color,
            InstructionSource.Entity,
            _zLayer
        );
    }

    private Vector2? GetHoveredTileCenter()
    {
        Vector2 mousePosition = CameraManager.ScreenToWorld(InputManager.GetMousePosition());
        Vector2 topLeft = _tilemap.GetTopLeft();
        Vector2 localMousePosition = mousePosition - topLeft;

        int tileX = (int)MathF.Floor(localMousePosition.X / _tilemap.CellSize.X);
        int tileY = (int)MathF.Floor(localMousePosition.Y / _tilemap.CellSize.Y);

        if (tileX < 0 || tileX >= _tilemap.Width || tileY < 0 || tileY >= _tilemap.Height)
            return null;

        return topLeft + new Vector2(
            (tileX * _tilemap.CellSize.X) + (_tilemap.CellSize.X * 0.5f),
            (tileY * _tilemap.CellSize.Y) + (_tilemap.CellSize.Y * 0.5f)
        );
    }
}
