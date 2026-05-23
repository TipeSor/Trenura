using System.Numerics;
using Engine;

class Level5 : Scene
{
    private readonly Player _player;
    private readonly Tilemap _tilemap;
    private readonly CoinLabel _coinLabel;
    private readonly List<Coin> _coins = [];

    public Level5()
    {
        _player = AddEntity(new Player());

        Entity tilemapObject = AddEntity(new Entity());
        tilemapObject.AddComponent(new Transform(Vector2.Zero));
        _tilemap = tilemapObject.AddComponent(new Tilemap(
            "ground",
            "level_5",
            new Vector2(32, 32)
        ));
        tilemapObject.AddComponent(new TilemapRenderer());
        tilemapObject.AddComponent(new TilemapCollider());

        _player.transform.LocalPosition = GetPlayerSpawnPosition();

        AddEntity(new CameraHandler(_player.transform, _tilemap));
        AddEntity(new Door(_player, GetDoorPosition(), [
            GameManager.SceneIndex.Level4,
            GameManager.SceneIndex.Level6,
            GameManager.SceneIndex.Spinner,
            GameManager.SceneIndex.CoinFlip
        ]));
        AddEntity(new TileClickDebugger(_tilemap));
        _coinLabel = AddWidget(new CoinLabel(fontSize: 28));

        SpawnCoins();
    }

    public override void OpenScene()
    {
        if (!GameManager.RunActive)
            GameManager.StartRun();

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
            new Vector2(-400, -176),
            new Vector2(112, -240),
            new Vector2(336, -16),
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
