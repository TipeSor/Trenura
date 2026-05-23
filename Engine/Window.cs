using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Raylib_cs;

namespace Engine;

public class Window
{
    /// <summary>
    /// Title of Window
    /// </summary>
    public string Title { get; set { Raylib.SetWindowTitle(value); field = value; } } = string.Empty;

    /// <summary>
    /// Size of Window
    /// </summary>
    public Vector2 ScreenSize
    {
        get;
        set
        {
            field = value;
            Raylib.SetWindowSize((int)value.X, (int)value.Y);
            RenderScale = MathF.Min(ScreenSize.X / RenderSize.X, ScreenSize.Y / RenderSize.Y);
        }
    }

    /// <summary>
    /// Size of Render
    /// </summary>
    public Vector2 RenderSize
    {
        get;
        set
        {
            field = value;
            UpdateRenderSize();
        }
    }

    /// <summary>
    /// Position of Window
    /// </summary>
    public static Vector2 Position
    {
        get => Raylib.GetWindowPosition();
        set => Raylib.SetWindowPosition((int)value.X, (int)value.Y);
    }

    /// <summary>
    /// Background Color used in Window
    /// </summary>
    public Color BackgroundColor { get; set; }

    /// <summary>
    /// Scale of Render
    /// </summary>
    public float RenderScale { get; private set; }

    /// <summary>
    /// Index of Current Scene
    /// </summary>
    public int CurrentSceneIndex
    {
        get;
        set
        {
            if (field != -1) Scenes[field].CloseScene();
            field = value;
            Scenes[value].OpenScene();
        }
    }

    /// <summary>
    /// Current Scene
    /// </summary>
    public Scene CurrentScene
    {
        get => Scenes[CurrentSceneIndex];
        set => CurrentSceneIndex = Scenes.IndexOf(value);
    }

    /// <summary>
    /// Get All Scenes
    /// </summary>
    public List<Scene> Scenes { get; } = [];

    private readonly RenderTexture2D _targetTexture;
    private bool _closeWindow;

    public Window(
        int height,
        int width,
        string title,
        Color? backgroundColor = null,
        int fps = 60)
        : this(new Vector2(width, height), title, backgroundColor, fps) { }

    public Window(
        Vector2 screenSize,
        string title,
        Color? backgroundColor = null,
        int fps = 60)
    {
        InputManager.InternalWindow = this;
        Title = title;
        ScreenSize = screenSize;
        RenderSize = screenSize;
        BackgroundColor = backgroundColor ?? Color.Black;

        unsafe
        {
            Raylib.SetTraceLogCallback(&LogCustom);
        }

        Raylib.InitWindow((int)screenSize.X, (int)screenSize.Y, title);
        Raylib.InitAudioDevice();

        CameraManager.SetScreenSize(screenSize);

        Raylib.SetTargetFPS(fps);

        _targetTexture = Raylib.LoadRenderTexture((int)screenSize.X, (int)screenSize.Y);
    }

    /// <summary>
    /// Take a screenshot and save it
    /// </summary>
    /// <param name="path">Path of saved screenshot</param>
    public static void TakeScreenshot(string path) => Raylib.TakeScreenshot(path);

    /// <summary>
    /// Set master volume
    /// </summary>
    /// <param name="volume">Volume (0 to 1)</param>
    public static void SetMasterVolume(float volume) => Raylib.SetMasterVolume(volume);

    /// <summary>
    /// Add Scene to Window
    /// </summary>
    /// <param name="scene">Scene which be added</param>
    public void AddScene(Scene scene)
    {
        scene.Window = this;
        Scenes.Add(scene);
    }

    /// <summary>
    /// Get Scene by Index
    /// </summary>
    /// <param name="index">Index of Scene</param>
    /// <returns>Scene</returns>
    public Scene GetScene(int index) => Scenes[index];

    /// <summary>
    /// Get Scene cast as T
    /// </summary>
    /// <param name="index">Index of Scene</param>
    /// <typeparam name="T">Type as Scene</typeparam>
    /// <returns>Scene cast as T</returns>
    public T GetScene<T>(int index)
        where T : Scene => (T)Scenes[index];

    /// <summary>
    /// Get Current Scene cast as T
    /// </summary>
    /// <typeparam name="T">Type as Scene</typeparam>
    /// <returns>Current Scene cast as T</returns>
    public T GetCurrentScene<T>()
        where T : Scene => (T)Scenes[CurrentSceneIndex];

    /// <summary>
    /// Run Window
    /// </summary>
    public void Run()
    {
        if (Scenes.Count == 0)
        {
            DebugManager.Log(LogLevel.Error, "There are no scenes.");
            return;
        }

        DebugManager.Log(LogLevel.Info, "Loading Scenes...");
        for (int i = 0; i < Scenes.Count; i++)
            Scenes[i].Load();
        DebugManager.Log(LogLevel.Info, "Scenes loaded.");

        CurrentScene.OpenScene();

        while (!Raylib.WindowShouldClose() && !_closeWindow)
        {
            InputManager.UpdateInput();

            if (System.Math.Abs(ScreenSize.X - Raylib.GetScreenWidth()) > 0.01f || System.Math.Abs(ScreenSize.Y - Raylib.GetScreenHeight()) > 0.01f)
            {
                ScreenSize = new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
                RenderScale = MathF.Min(ScreenSize.X / RenderSize.X, ScreenSize.Y / RenderSize.Y);
            }

            var delta = Raylib.GetFrameTime();

            #region Update
            CurrentScene.Update(delta);
            #endregion

            #region Draw
            CurrentScene.Draw();

            {
                Raylib.BeginTextureMode(_targetTexture);
                Raylib.ClearBackground(BackgroundColor);

                Renderer.Draw(this);

                Raylib.EndTextureMode();
            }

            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                var dest = new Rectangle(
                    (ScreenSize.X - (RenderSize.X * RenderScale)) * 0.5f,
                    (ScreenSize.Y - (RenderSize.Y * RenderScale)) * 0.5f,
                    RenderSize.X * RenderScale,
                    RenderSize.Y * RenderScale
                );

                Raylib.DrawTexturePro(_targetTexture.Texture, new Rectangle(0, 0, _targetTexture.Texture.Width, -_targetTexture.Texture.Height),
                    dest, new Vector2(0, 0), 0, Color.White);

                Raylib.EndDrawing();
            }
            #endregion
        }

        DebugManager.Log(LogLevel.Info, "Unloading scenes...");
        foreach (var scene in Scenes)
            scene.Unload();
        DebugManager.Log(LogLevel.Info, "Scenes unloaded.");

        DebugManager.Log(LogLevel.Info, "Unloading textures...");
        Raylib.UnloadRenderTexture(_targetTexture);
        TextureManager.Unload();
        DebugManager.Log(LogLevel.Info, "Textures unloaded.");
        DebugManager.Log(LogLevel.Info, "Unloading fonts...");
        FontManager.Unload();
        DebugManager.Log(LogLevel.Info, "Fonts unloaded.");
        DebugManager.Log(LogLevel.Info, "Unloading shaders...");
        // ShaderManager.Unload();
        DebugManager.Log(LogLevel.Info, "Shaders unloaded.");

        DebugManager.Log(LogLevel.Info, "Closing window.");
        Raylib.CloseAudioDevice();
        Raylib.CloseWindow();
    }

    /// <summary>
    /// Stop Window
    /// </summary>
    public void Stop()
    {
        _closeWindow = true;
    }

    private void UpdateRenderSize()
    {
        CameraManager.SetScreenSize(RenderSize);
        RenderScale = MathF.Min(ScreenSize.X / RenderSize.X, ScreenSize.Y / RenderSize.Y);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void LogCustom(int logLevel, sbyte* text, sbyte* args)
    {
        var message = Logging.GetLogMessage(new IntPtr(text), new IntPtr(args));
        var level = (LogLevel)logLevel;

        var timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

        var formatted = FormatMessage(level, message, useColor: !Console.IsOutputRedirected);
        Console.WriteLine($"{timestamp} - {formatted}");
    }

    private static string FormatMessage(LogLevel level, string message, bool useColor)
    {
        string? label = level switch
        {
            LogLevel.Trace => "TRACE",
            LogLevel.All => "ALL",
            LogLevel.Debug => "DEBUG",
            LogLevel.Info => "INFO",
            LogLevel.Warning => "WARNING",
            LogLevel.Error => "ERROR",
            LogLevel.Fatal => "FATAL",
            _ => null
        };

        if (label == null)
            return message;

        if (!useColor)
            return $"{label}: {message}";

        return level switch
        {
            LogLevel.Info => $"{ColorText(Color.SkyBlue, label)}: {message}",
            LogLevel.Warning => $"{ColorText(Color.Yellow, label)}: {message}",
            LogLevel.Error => $"{ColorText(Color.Red, label)}: {message}",
            LogLevel.Fatal => $"{ColorText(Color.Maroon, label)}: {message}",
            _ => $"{label}: {message}"
        };
    }

    private static string ColorText(Color color, string text)
    {
        if (Console.IsOutputRedirected)
            return text;

        return $"\u001b[38;2;{color.R};{color.G};{color.B}m{text}\u001b[0m";
    }
}
