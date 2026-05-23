using Engine;
using System.Numerics;
using Raylib_cs;

public static class GameManager
{
    public class SceneIndex
    {
        public const int MainMenu = 0;

        public const int TutorialLevel1 = 1;
        public const int TutorialLevel2 = 2;
        public const int TutorialLevel3 = 3;
        public const int TutorialSubmit = 4;

        public const int Level4 = 5;
        public const int Level5 = 6;
        public const int Level6 = 7;
        public const int Spinner = 8;
        public const int CoinFlip = 9;
        public const int Summary = 10;
    }

    public const KeyboardKey SubmitRunKey = KeyboardKey.Backspace;
    public const string SubmitRunKeyLabel = "Backspace";

    public static int Coins
    {
        get;
        set
        {
            int val = int.Max(value, 0);

            int change = val - field;
            field = val;

            if (RunActive && field > MaxCoins)
                MaxCoins = field;

            UpdateText();

            if (change != 0)
                SpawnCoinChangePopup(change);
        }
    }

    public static Window Window = null!;
    public static CoinLabel? CoinLabel { get; set; }
    public static bool RunActive { get; private set; }
    public static int MaxCoins { get; private set; }
    public static int RoomsVisited { get; private set; }
    public static int LastRunCoins { get; private set; }
    public static int LastRunMaxCoins { get; private set; }
    public static int LastRunRoomsVisited { get; private set; }
    public static int LastRunScore { get; private set; }

    public static bool CanSubmitRunInScene(int sceneIndex)
    {
        return sceneIndex == SceneIndex.TutorialSubmit
            || sceneIndex == SceneIndex.Level4
            || sceneIndex == SceneIndex.Level5
            || sceneIndex == SceneIndex.Level6
            || sceneIndex == SceneIndex.Spinner
            || sceneIndex == SceneIndex.CoinFlip;
    }

    public static void StartRun()
    {
        RunActive = true;
        MaxCoins = 0;
        RoomsVisited = 0;
        Coins = 0;
    }

    public static void EnterRoom()
    {
        if (!RunActive)
            StartRun();

        RoomsVisited++;
    }

    public static void EndRun()
    {
        if (!RunActive)
            return;

        LastRunCoins = Coins;
        LastRunMaxCoins = MaxCoins;
        LastRunRoomsVisited = RoomsVisited;
        LastRunScore = CalculateScore(LastRunCoins, LastRunRoomsVisited);
        RunActive = false;
        CoinLabel = null;
        Window.CurrentSceneIndex = SceneIndex.Summary;
    }

    public static void AbandonRun()
    {
        RunActive = false;
        CoinLabel = null;
    }

    public static int CalculateScore(int coins, int roomsVisited)
    {
        return roomsVisited <= 0 ? 0 : (coins * 1000) / roomsVisited;
    }

    public static void UpdateText()
    {
        if (CoinLabel == null)
            return;

        CoinLabel.Text = $"Coins: {Coins}";
    }

    private static void SpawnCoinChangePopup(int change)
    {
        if (CoinLabel?.Scene == null)
            return;

        if (change == 0)
            return;

        Rectangle bounds = CoinLabel.Bounds;
        Vector2 popupPosition = new(
            bounds.X + bounds.Width + 10,
            bounds.Y + (bounds.Height * 0.5f)
        );

        CoinLabel.Scene.AddWidget(
            new CoinChangePopup(
                change,
                popupPosition,
                font: CoinLabel.Font,
                fontSize: CoinLabel.FontSize * 0.75f,
                zLayer: CoinLabel.ZLayer + 1
            )
        );
    }
}
