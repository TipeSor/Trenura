using System.Numerics;
using Engine;
using Raylib_cs;

class TutorialSubmit : Scene
{
    private readonly Player _player;
    private readonly Tilemap _tilemap;
    private readonly CoinLabel _coinLabel;

    public TutorialSubmit()
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

        AddEntity(new CameraHandler(_player.transform, _tilemap));
        _coinLabel = AddWidget(new CoinLabel(fontSize: 28));

        Label title = AddWidget(new Label(
            new Vector2(0, -24),
            $"Press {GameManager.SubmitRunKeyLabel} to submit a run",
            fontColor: Color.Beige,
            fontSize: 28,
            zLayer: 5
        ));
        title.HorizontalAnchor = HorizontalAlignment.Center;
        title.VerticalAnchor = VerticalAlignment.Center;
    }

    public override void OpenScene()
    {
        if (!GameManager.RunActive)
            GameManager.StartRun();

        _player.ResetState(GetPlayerSpawnPosition());
        GameManager.CoinLabel = _coinLabel;
        GameManager.EnterRoom();
        GameManager.UpdateText();
    }

    private Vector2 GetPlayerSpawnPosition()
    {
        Vector2 topLeft = _tilemap.GetTopLeft();
        return new Vector2(
            topLeft.X + (_tilemap.CellSize.X * 2.5f),
            topLeft.Y + _tilemap.Size.Y - (_tilemap.CellSize.Y * 2.5f)
        );
    }
}
