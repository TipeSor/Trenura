using System.Numerics;
using Engine;
using Raylib_cs;

class MainMenu : Scene
{
    public MainMenu()
    {
        Panel panel = AddWidget(new Panel(
            Vector2.Zero,
            new Vector2(420, 340),
            new Color(36, 24, 20, 220),
            Color.Beige,
            3,
            10
        ));
        panel.HorizontalAnchor = HorizontalAlignment.Center;
        panel.VerticalAnchor = VerticalAlignment.Center;

        Label title = AddWidget(new Label(
            new Vector2(0, -105),
            "TRENURA",
            fontColor: Color.Gold,
            fontSize: 54,
            zLayer: 11
        ));
        title.HorizontalAnchor = HorizontalAlignment.Center;
        title.VerticalAnchor = VerticalAlignment.Center;

        Label subtitle = AddWidget(new Label(
            new Vector2(0, -68),
            "Spin, collect and waste coins wisely.",
            fontColor: Color.Beige,
            fontSize: 18,
            zLayer: 11
        ));
        subtitle.HorizontalAnchor = HorizontalAlignment.Center;
        subtitle.VerticalAnchor = VerticalAlignment.Center;

        Button runButton = AddWidget(new Button(
            new Vector2(0, -8),
            "Start Run",
            size: new Vector2(220, 54),
            fontColor: Color.Beige,
            backgroundColor: new Color(28, 110, 52, 255),
            fontSize: 28,
            zLayer: 12
        ));
        runButton.HorizontalAnchor = HorizontalAlignment.Center;
        runButton.VerticalAnchor = VerticalAlignment.Center;

        Button tutorialButton = AddWidget(new Button(
            new Vector2(0, 54),
            "Tutorial",
            size: new Vector2(220, 54),
            fontColor: Color.Beige,
            backgroundColor: new Color(153, 108, 30, 255),
            fontSize: 28,
            zLayer: 12
        ));
        tutorialButton.HorizontalAnchor = HorizontalAlignment.Center;
        tutorialButton.VerticalAnchor = VerticalAlignment.Center;

        Button quitButton = AddWidget(new Button(
            new Vector2(0, 116),
            "Quit",
            size: new Vector2(220, 54),
            fontColor: Color.Beige,
            backgroundColor: new Color(110, 40, 36, 255),
            fontSize: 28,
            zLayer: 12
        ));
        quitButton.HorizontalAnchor = HorizontalAlignment.Center;
        quitButton.VerticalAnchor = VerticalAlignment.Center;

        tutorialButton.Clicked += () =>
        {
            GameManager.AbandonRun();
            GameManager.Coins = 0;
            GameManager.Window.CurrentSceneIndex = GameManager.SceneIndex.TutorialLevel1;
        };

        runButton.Clicked += () => GameManager.Window.CurrentSceneIndex = GameManager.SceneIndex.Level4;
        quitButton.Clicked += () => GameManager.Window.Stop();
    }

    public override void OpenScene()
    {
        CameraManager.Position = Vector2.Zero;
        GameManager.CoinLabel = null;
    }
}
