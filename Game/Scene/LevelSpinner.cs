using System.Numerics;
using Engine;
using Raylib_cs;

class LevelSpinner : Scene
{
    private readonly Player _player;
    private readonly Tilemap _tilemap;
    private readonly Spinner _spinner;
    private readonly CoinLabel _coinLabel;
    private readonly List<Coin> _coins = [];
    private readonly Random _rng = new();

    public LevelSpinner()
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
        AddEntity(new Door(_player, GetDoorPosition(), [
            GameManager.SceneIndex.Level4,
            GameManager.SceneIndex.Level5,
            GameManager.SceneIndex.Level6,
            GameManager.SceneIndex.CoinFlip
        ]));
        _coinLabel = AddWidget(new CoinLabel(fontSize: 28));

        Entity spinnerObject = AddEntity(new Entity());
        spinnerObject.AddComponent(new Transform(
            GetSpinnerPosition(),
            new Vector2(1.5f, 1.5f),
            zLayer: 15
        ));
        _spinner = spinnerObject.AddComponent(new Spinner(
            "spinner",
            new Vector2(256, 256),
            48,
            framesPerSecond: 48,
            faceFrameStep: 8,
            faceFrameOffset: 7,
            loop: true
        ));

        _spinner.OnLanded += (index, face) =>
        {
                 if (face == 0) GameManager.Coins -= 2;
            else if (face == 1) GameManager.Coins *= 4;
            else if (face == 2) GameManager.Coins /= 4;
            else if (face == 3) GameManager.Coins *= 0;
            else if (face == 4) GameManager.Coins /= 8;
            else if (face == 5) GameManager.Coins *= 2;
        };

        Entity spinner_trigger = AddEntity(new Entity());
        spinner_trigger.AddComponent(new Transform(GetSpinnerTriggerPosition(), new Vector2(64, 32)));
        spinner_trigger.AddComponent(new BoxCollider(){ IsTrigger = true })
            .OnEnter += (other) => { if (other.Entity != _player) return; _spinner.SpinToFace(_rng.Next(0, 6), 0, 1f, true, 24); };
        spinner_trigger.AddComponent(new RectRenderer(Color.Orange));

        SpawnCoins();
    }

    public override void OpenScene()
    {
        if (!GameManager.RunActive)
            GameManager.StartRun();

        _player.ResetState(GetPlayerSpawnPosition());
        _spinner.Reset();

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

    private Vector2 GetPlayerSpawnPosition()
    {
        Vector2 topLeft = _tilemap.GetTopLeft();
        return new Vector2(
            topLeft.X + (_tilemap.CellSize.X * 2.5f),
            topLeft.Y + _tilemap.Size.Y - (_tilemap.CellSize.Y * 2.5f)
        );
    }

    private List<Vector2> GetCoinPositions()
    {
        return
        [
            // GetSpinnerPosition() + new Vector2(-240, 0),
        ];
    }

    private Vector2 GetSpinnerPosition() => Vector2.Zero;

    private Vector2 GetSpinnerTriggerPosition()
    {
        return GetSpinnerPosition() + new Vector2(0, 192);
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
