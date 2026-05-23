using System.Numerics;
using Engine;
using Raylib_cs;

class Level1 : Scene
{
    private readonly Player _player;
    private readonly Tilemap _tilemap;

    public Level1()
    {
        _player = AddEntity(new Player());

        Entity tilemapObject = AddEntity(new Entity());
        tilemapObject.AddComponent(new Transform(Vector2.Zero));
        _tilemap = tilemapObject.AddComponent(new Tilemap(
            "ground",
            "level_1",
            new Vector2(32, 32)
        ));
        tilemapObject.AddComponent(new TilemapRenderer());
        tilemapObject.AddComponent(new TilemapCollider());

        _player.transform.LocalPosition = GetPlayerSpawnPosition();

        AddEntity(new CameraHandler(_player.transform, _tilemap));
        AddEntity(new Door(_player, GetDoorPosition(), GameManager.SceneIndex.TutorialLevel2));

        Label hint = AddWidget(new Label(
            Vector2.Zero,
            "Use A/D to move",
            fontColor: Color.Beige,
            fontSize: 28,
            zLayer: 5
        ));
        hint.HorizontalAnchor = HorizontalAlignment.Center;
        hint.VerticalAnchor = VerticalAlignment.Center;
    }

    public override void OpenScene()
    {
        GameManager.CoinLabel = null;
        _player.ResetState(GetPlayerSpawnPosition());

        GameManager.EnterRoom();
    }

    private Vector2 GetPlayerSpawnPosition()
    {
        Vector2 topLeft = _tilemap.GetTopLeft();
        return new Vector2(
            topLeft.X + (_tilemap.CellSize.X * 2.5f),
            topLeft.Y + _tilemap.Size.Y - (_tilemap.CellSize.Y * 2.5f)
        );
    }

    private Vector2 GetDoorPosition()
    {
        Vector2 topLeft = _tilemap.GetTopLeft();
        return new Vector2(
            topLeft.X + _tilemap.Size.X - (_tilemap.CellSize.X * 2.5f),
            topLeft.Y + _tilemap.Size.Y - (_tilemap.CellSize.Y * 1.75f)
        );
    }
}
