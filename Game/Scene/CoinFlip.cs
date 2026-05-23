using System.Numerics;
using Engine;
using Raylib_cs;

class CoinFlip : Scene
{
    private enum CoinSide
    {
        Heads,
        Tails,
    }

    private readonly Player _player;
    private readonly Tilemap _tilemap;
    private readonly Spinner _coinSpinner;
    private readonly CoinLabel _coinLabel;
    private readonly Label _statusLabel;
    private readonly Random _rng = new();
    private CoinSide? _selectedSide;
    private bool _flipInProgress;

    public CoinFlip()
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
        AddEntity(new Door(_player, GetDoorPosition(), [
            GameManager.SceneIndex.Level4,
            GameManager.SceneIndex.Level5,
            GameManager.SceneIndex.Level6,
            GameManager.SceneIndex.Spinner
        ]));

        _coinLabel = AddWidget(new CoinLabel(fontSize: 28));

        Label title = AddWidget(new Label(
            new Vector2(0, -184),
            "Choose heads or tails",
            fontColor: Color.Gold,
            fontSize: 28,
            zLayer: 8
        ));
        title.HorizontalAnchor = HorizontalAlignment.Center;
        title.VerticalAnchor = VerticalAlignment.Center;

        _statusLabel = AddWidget(new Label(
            new Vector2(0, -148),
            "Step on a side to call the flip.",
            fontColor: Color.Beige,
            fontSize: 20,
            zLayer: 8
        ));
        _statusLabel.HorizontalAnchor = HorizontalAlignment.Center;
        _statusLabel.VerticalAnchor = VerticalAlignment.Center;

        Label legendLabel = AddWidget(new Label(
            new Vector2(0, -118),
            "Blue pad = Heads, red pad = Tails",
            fontColor: Color.Beige,
            fontSize: 18,
            zLayer: 8
        ));
        legendLabel.HorizontalAnchor = HorizontalAlignment.Center;
        legendLabel.VerticalAnchor = VerticalAlignment.Center;

        AddChoiceTrigger(GetChoicePadPosition(-176), new Color(68, 112, 164, 255), "Heads", CoinSide.Heads);
        AddChoiceTrigger(GetChoicePadPosition(176), new Color(164, 88, 68, 255), "Tails", CoinSide.Tails);

        Entity coinObject = AddEntity(new Entity());
        coinObject.AddComponent(new Transform(
            GetCoinPosition(),
            new Vector2(1, 1),
            zLayer: 15
        ));
        _coinSpinner = coinObject.AddComponent(new Spinner(
            "ht_coin",
            new Vector2(256, 256),
            16,
            framesPerSecond: 48,
            faceFrameOffset: 7,
            faceFrameStep: 8,
            loop: true
        ));
        _coinSpinner.SetFrame(0);
        _coinSpinner.OnLanded += (_, _) => ResolveFlip();
    }

    public override void OpenScene()
    {
        if (!GameManager.RunActive)
            GameManager.StartRun();

        _player.ResetState(GetPlayerSpawnPosition());
        _coinSpinner.Reset();
        _coinSpinner.SetFrame(0);
        _coinSpinner.Play();
        _selectedSide = null;
        _flipInProgress = false;
        _statusLabel.Text = "Step on a side to call the flip.";

        GameManager.CoinLabel = _coinLabel;
        GameManager.EnterRoom();
        GameManager.UpdateText();
    }

    private void AddChoiceTrigger(Vector2 position, Color color, string text, CoinSide side)
    {
        Entity choiceTrigger = AddEntity(new Entity());
        choiceTrigger.AddComponent(new Transform(position, new Vector2(96, 48), zLayer: 13));
        choiceTrigger.AddComponent(new BoxCollider() { IsTrigger = true })
            .OnEnter += other =>
            {
                if (other.Entity != _player)
                    return;

                _selectedSide = side;
                TryFlip();
            };
        choiceTrigger.AddComponent(new RectRenderer(color));
    }

    private void TryFlip()
    {
        if (_flipInProgress)
            return;

        if (_selectedSide == null)
        {
            _statusLabel.Text = "Choose heads or tails first.";
            return;
        }

        _flipInProgress = true;
        _statusLabel.Text = _selectedSide == CoinSide.Heads
            ? "Heads selected. Slowing down..."
            : "Tails selected. Slowing down...";

        int landedFace = _rng.Next(0, 2);
        _coinSpinner.SpinToFace(
            landedFace,
            extraLoops: 2,
            holdTime: 0.75f,
            restartAfterHold: true,
            spinFramesPerSecond: 24
        );
    }

    private void ResolveFlip()
    {
        CoinSide landedSide = GetSideFromFrame(_coinSpinner.CurrentFaceIndex);
        if (_selectedSide == landedSide)
        {
            GameManager.Coins *= 2;
            _statusLabel.Text = $"{landedSide} won. Coins doubled.";
        }
        else
        {
            GameManager.Coins /= 2;
            _statusLabel.Text = $"{landedSide} won. Coins halved.";
        }

        _flipInProgress = false;
        _selectedSide = null;
    }

    private static CoinSide GetSideFromFrame(int face)
    {
        return face == 1 ? CoinSide.Heads : CoinSide.Tails;
    }

    private Vector2 GetPlayerSpawnPosition()
    {
        Vector2 topLeft = _tilemap.GetTopLeft();
        return new Vector2(
            topLeft.X + (_tilemap.CellSize.X * 2.5f),
            topLeft.Y + _tilemap.Size.Y - (_tilemap.CellSize.Y * 2.5f)
        );
    }

    private Vector2 GetCoinPosition() => Vector2.Zero;

    private static Vector2 GetChoicePadPosition(float x) => new(x, 192);

    private Vector2 GetDoorPosition()
    {
        Vector2 topLeft = _tilemap.GetTopLeft();
        return new Vector2(
            topLeft.X + _tilemap.Size.X - (_tilemap.CellSize.X * 2.5f),
            topLeft.Y + _tilemap.Size.Y - (_tilemap.CellSize.Y * 1.75f)
        );
    }
}
