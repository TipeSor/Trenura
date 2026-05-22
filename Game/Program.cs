using System.Numerics;
using Engine;
using Raylib_cs;

Window win = new Window(600, 800, "Cool Game", Color.Red);

win.AddScene(new MainMenu());

win.Run();

class MainMenu : Scene
{
    public MainMenu()
    {
        AddEntity(new Player());
    }
}

class Player : Entity
{
    public Transform transform;

    public Player()
    {
        transform = AddComponent(new Transform(Vector2.Zero, Vector2.One, zLayer: 12));
        AddComponent(new PlayerController(100));
        AddComponent(new RectRenderer(Vector2.One * 16, Color.Blue));
    }
}

class TileMap : Entity
{
    public Transform transform;

    public TileMap()
    {
        // AddComponent(new TilemapRenderer);
        // AddComponent(new TilemapCollider);
    }
}

class PlayerController : Component
{
    Dictionary<ControlKey, KeyboardKey> _keys;

    public int Speed { get; set; }
    public bool IsMoving { get; protected set; }
    public Vector2 Direction { get; protected set; }

    Transform? transform = null!;

    public PlayerController(int speed)
    {
        Speed = speed;

        _keys = new()
        {
            { ControlKey.Up, KeyboardKey.W },
            { ControlKey.Down, KeyboardKey.S },
            { ControlKey.Left, KeyboardKey.A },
            { ControlKey.Right, KeyboardKey.D }
        };
    }

    public KeyboardKey GetKey(ControlKey key) => _keys[key];
    public void SetKey(ControlKey key, KeyboardKey value) => _keys[key] = value;

    public override void Load()
    {
        transform = Entity?.GetComponent<Transform>();
    }

    public override void Update(float delta)
    {
        base.Update(delta);

        if (transform == null)
            return;

        Vector2 movement = GetMovement();

        if (movement == Vector2.Zero)
        {
            IsMoving = false;
            Direction = Vector2.Zero;
            return;
        }

        IsMoving = true;
        Direction = Vector2.Normalize(movement);
        Vector2 moveDelta = Direction * Speed * delta;

        transform.LocalPosition += moveDelta;
    }

    private Vector2 GetMovement()
    {
        int dirX = 0, dirY = 0;
        if (InputManager.IsKeyDown(_keys[ControlKey.Up])) dirY--;
        if (InputManager.IsKeyDown(_keys[ControlKey.Left])) dirX--;
        if (InputManager.IsKeyDown(_keys[ControlKey.Down])) dirY++;
        if (InputManager.IsKeyDown(_keys[ControlKey.Right])) dirX++;
        return new Vector2(dirX, dirY);
    }
}

public enum ControlKey
{
    Up,
    Down,
    Left,
    Right
}

public class RectRenderer : Component
{
    public Vector2 Size { get; set; }
    public Color Color { get; set; }

    Transform? transform;

    int ZLayerOffset;

    public RectRenderer(Vector2 size, Color color, int zLayerOffset = 0)
    {
        Size = size;
        Color = color;

        ZLayerOffset = zLayerOffset;
    }

    public override void Load()
    {
        transform = Entity?.GetComponent<Transform>();
    }

    public override void Draw()
    {
        if (transform == null)
            return;

        Vector2 position = transform.Position;
        Vector2 size = Size * transform.Scale;

        Render.DrawRectangle(position - size / 2, size, Color, InstructionSource.Entity, transform.ZLayer + ZLayerOffset);
    }
}
