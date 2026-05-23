using System.Numerics;
using Engine;
using Raylib_cs;

class RunSummary : Scene
{
    private readonly Label _coinsValue;
    private readonly Label _maxCoinsValue;
    private readonly Label _roomsValue;
    private readonly Label _scoreValue;

    public RunSummary()
    {
        Panel panel = AddWidget(new Panel(
            Vector2.Zero,
            new Vector2(460, 380),
            new Color(36, 24, 20, 220),
            Color.Beige,
            3,
            10
        ));
        panel.HorizontalAnchor = HorizontalAlignment.Center;
        panel.VerticalAnchor = VerticalAlignment.Center;

        AddCenteredLabel(new Vector2(0, -142), "RUN SUBMITTED", Color.Gold, 44, 11);
        AddCenteredLabel(new Vector2(-140, -80), "Coins", Color.Beige, 24, 11, HorizontalAlignment.Left);
        AddCenteredLabel(new Vector2(-140, -32), "Max Coins", Color.Beige, 24, 11, HorizontalAlignment.Left);
        AddCenteredLabel(new Vector2(-140, 16), "Rooms Visited", Color.Beige, 24, 11, HorizontalAlignment.Left);
        AddCenteredLabel(new Vector2(0, 82), "Score", Color.Gold, 28, 11);

        _coinsValue = AddCenteredLabel(new Vector2(140, -80), "0", Color.Gold, 24, 11, HorizontalAlignment.Right);
        _maxCoinsValue = AddCenteredLabel(new Vector2(140, -32), "0", Color.Gold, 24, 11, HorizontalAlignment.Right);
        _roomsValue = AddCenteredLabel(new Vector2(140, 16), "0", Color.Gold, 24, 11, HorizontalAlignment.Right);
        _scoreValue = AddCenteredLabel(new Vector2(0, 120), "0", Color.Beige, 42, 12);

        Button runAgainButton = AddWidget(new Button(
            new Vector2(-110, 230),
            "New Run",
            size: new Vector2(180, 52),
            fontColor: Color.Beige,
            backgroundColor: new Color(28, 110, 52, 255),
            fontSize: 24,
            zLayer: 12
        ));
        runAgainButton.HorizontalAnchor = HorizontalAlignment.Center;
        runAgainButton.VerticalAnchor = VerticalAlignment.Center;
        runAgainButton.Clicked += () => GameManager.Window.CurrentSceneIndex = GameManager.SceneIndex.Level4;

        Button menuButton = AddWidget(new Button(
            new Vector2(110, 230),
            "Main Menu",
            size: new Vector2(180, 52),
            fontColor: Color.Beige,
            backgroundColor: new Color(110, 40, 36, 255),
            fontSize: 24,
            zLayer: 12
        ));
        menuButton.HorizontalAnchor = HorizontalAlignment.Center;
        menuButton.VerticalAnchor = VerticalAlignment.Center;
        menuButton.Clicked += () => GameManager.Window.CurrentSceneIndex = GameManager.SceneIndex.MainMenu;
    }

    public override void OpenScene()
    {
        CameraManager.Position = Vector2.Zero;
        GameManager.CoinLabel = null;

        _coinsValue.Text = GameManager.LastRunCoins.ToString();
        _maxCoinsValue.Text = GameManager.LastRunMaxCoins.ToString();
        _roomsValue.Text = GameManager.LastRunRoomsVisited.ToString();
        _scoreValue.Text = GameManager.LastRunScore.ToString();
    }

    private Label AddCenteredLabel(
        Vector2 position,
        string text,
        Color color,
        int fontSize,
        int zLayer,
        HorizontalAlignment horizontalAlign = HorizontalAlignment.Center)
    {
        Label label = AddWidget(new Label(position, text, fontColor: color, fontSize: fontSize, zLayer: zLayer));
        label.HorizontalAnchor = HorizontalAlignment.Center;
        label.VerticalAnchor = VerticalAlignment.Center;
        label.HorizontalAlign = horizontalAlign;
        return label;
    }
}
