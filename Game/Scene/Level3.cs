using System.Numerics;
using Engine;
using Raylib_cs;

class Level3 : Scene
{
    private readonly Player _player;
    private readonly Tilemap _tilemap;
    private readonly CoinLabel _coinLabel;
    private readonly List<Coin> _coins = [];

    public Level3()
    {
        _player = AddEntity(new Player());

        Entity tilemapObject = AddEntity(new Entity());
        tilemapObject.AddComponent(new Transform(Vector2.Zero));
        _tilemap = tilemapObject.AddComponent(new Tilemap(
            "ground",
            "level_3",
            new Vector2(32, 32)
        ));
        tilemapObject.AddComponent(new TilemapRenderer());
        tilemapObject.AddComponent(new TilemapCollider());

        _player.transform.LocalPosition = GetPlayerSpawnPosition();

        AddEntity(new CameraHandler(_player.transform, _tilemap));
        AddEntity(new Door(_player, GetDoorPosition(), GameManager.SceneIndex.TutorialSubmit));

        _coinLabel = AddWidget(new CoinLabel(fontSize: 28));

        Label hint = AddWidget(new Label(
            new Vector2(0, 16),
            "Collect the coins",
            fontColor: Color.Beige,
            fontSize: 28,
            zLayer: 5
        ));
        hint.HorizontalAnchor = HorizontalAlignment.Center;
        hint.VerticalAnchor = VerticalAlignment.Center;

        SpawnCoins();
    }

    public override void OpenScene()
    {
        _player.ResetState(GetPlayerSpawnPosition());

        foreach (Coin coin in _coins)
            coin.Reset();

        GameManager.CoinLabel = _coinLabel;
        GameManager.EnterRoom();
        GameManager.UpdateText();
    }

    private void SpawnCoins()
    {
        foreach (Vector2 position in GetCoinPositions())
            _coins.Add(AddEntity(new Coin(position, 1)));
    }

    private List<Vector2> GetCoinPositions()
    {
        return
        [
            new Vector2(-176, -16),
            new Vector2(-368, -208),
            new Vector2(208, -208),
            new Vector2(400, 48),
            new Vector2(16, 272),
            new Vector2(-336, 144),
            new Vector2(-80, 112),
            new Vector2(240, 16),
            new Vector2(48, -80),
        ];
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
