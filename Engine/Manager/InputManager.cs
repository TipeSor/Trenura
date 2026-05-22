using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Class which manage input interactions
/// </summary>
public static class InputManager
{
    internal static List<int> InternalPressedChars { get; } = [];
    private static List<int> InternalPressedKeys { get; } = [];
    internal static Window InternalWindow { get; set; } = null!;

    /// <summary>
    /// List of Pressed Chars in Frame
    /// </summary>
    public static List<char> GetPressedChars() => InternalPressedChars.Select(x => (char)x).ToList();

    /// <summary>
    /// List of Pressed Keys in Frame
    /// </summary>
    public static List<KeyboardKey> GetPressedKeys() => InternalPressedKeys.Cast<KeyboardKey>().ToList();

    /// <summary>
    /// Check if key is down
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>If Key is down</returns>
    public static bool IsKeyDown(KeyboardKey key) => Raylib.IsKeyDown(key);

    /// <summary>
    /// Check if key is up
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>If Key is up</returns>
    public static bool IsKeyUp(KeyboardKey key) => Raylib.IsKeyUp(key);

    /// <summary>
    /// Check if key is pressed
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>If Key was pressed once</returns>
    public static bool IsKeyPressed(KeyboardKey key) => Raylib.IsKeyPressed(key);

    /// <summary>
    /// Check if key is released
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>If Key was released once</returns>
    public static bool IsKeyReleased(KeyboardKey key) => Raylib.IsKeyReleased(key);

    /// <summary>
    /// Check if the mouse button is down
    /// </summary>
    /// <param name="button">Mouse button</param>
    /// <returns>If the mouse button is down</returns>
    public static bool IsMouseButtonDown(MouseButton button) =>
        Raylib.IsMouseButtonDown(button);

    /// <summary>
    /// Check if the mouse button is up
    /// </summary>
    /// <param name="button">Mouse button</param>
    /// <returns>If the mouse button is up</returns>
    public static bool IsMouseButtonUp(MouseButton button) =>
        Raylib.IsMouseButtonUp(button);

    /// <summary>
    /// Check if the mouse button is pressed
    /// </summary>
    /// <param name="button">Mouse button</param>
    /// <returns>If the mouse button is pressed</returns>
    public static bool IsMouseButtonPressed(MouseButton button) =>
        Raylib.IsMouseButtonPressed(button);

    /// <summary>
    /// Check if the mouse button is released
    /// </summary>
    /// <param name="button">Mouse button</param>
    /// <returns>If the mouse button is released</returns>
    public static bool IsMouseButtonReleased(MouseButton button) =>
        Raylib.IsMouseButtonReleased(button);

    /// <summary>
    /// Get Mouse position
    /// </summary>
    /// <returns>Position</returns>
    public static Vector2 GetMousePosition()
    {
        Vector2 realPosition = Raylib.GetMousePosition();
        return (realPosition - ((InternalWindow.ScreenSize - (InternalWindow.RenderSize * InternalWindow.RenderScale)) * 0.5f)) / InternalWindow.RenderScale;
    }

    /// <summary>
    /// Get Real Mouse position
    /// </summary>
    /// <returns>Position</returns>
    public static Vector2 GetRealMousePosition() => Raylib.GetMousePosition();

    /// <summary>
    /// Set Mouse Position
    /// </summary>
    /// <param name="position">Position</param>
    public static void SetMousePosition(Vector2 position)
    {
        var x = position.X * InternalWindow.RenderScale + (InternalWindow.ScreenSize.X - (InternalWindow.RenderSize.X * InternalWindow.RenderScale)) * 0.5f;
        var y = position.Y * InternalWindow.RenderScale + (InternalWindow.ScreenSize.Y - (InternalWindow.RenderSize.Y * InternalWindow.RenderScale)) * 0.5f;
        Raylib.SetMousePosition((int)x, (int)y);
    }

    /// <summary>
    /// Set Real Mouse Position
    /// </summary>
    /// <param name="position">Position</param>
    public static void SetRealMousePosition(Vector2 position) =>
        Raylib.SetMousePosition((int)position.X, (int)position.Y);

    /// <summary>
    /// Get Mouse Wheel Movement Value
    /// </summary>
    /// <returns>Value</returns>
    public static float GetMouseWheelMove() => Raylib.GetMouseWheelMove();

    /// <summary>
    /// Check if the gamepad is connected
    /// </summary>
    /// <param name="index">Index of Gamepad</param>
    /// <returns>If the gamepad is connected</returns>
    public static bool IsGamePadConnected(int index) => Raylib.IsGamepadAvailable(index);

    /// <summary>
    /// Check if the gamepad button is down
    /// </summary>
    /// <param name="index">Index of Gamepad</param>
    /// <param name="button">Button of Gamepad</param>
    /// <returns>If the gamepad button is down</returns>
    public static bool IsGamePadButtonDown(int index, GamepadButton button) =>
        Raylib.IsGamepadButtonDown(index, button);

    /// <summary>
    /// Check if the gamepad button is up
    /// </summary>
    /// <param name="index">Index of Gamepad</param>
    /// <param name="button">Button of Gamepad</param>
    /// <returns>If the gamepad button is up</returns>
    public static bool IsGamePadButtonUp(int index, GamepadButton button) =>
        Raylib.IsGamepadButtonUp(index, button);

    /// <summary>
    /// Check if the gamepad button is pressed
    /// </summary>
    /// <param name="index">Index of Gamepad</param>
    /// <param name="button">Button of Gamepad</param>
    /// <returns>If the gamepad button is pressed</returns>
    public static bool IsGamePadButtonPressed(int index, GamepadButton button) =>
        Raylib.IsGamepadButtonPressed(index, button);

    /// <summary>
    /// Check if the gamepad button is released
    /// </summary>
    /// <param name="index">Index of Gamepad</param>
    /// <param name="button">Button of Gamepad</param>
    /// <returns>If the gamepad button is released</returns>
    public static bool IsGamePadButtonReleased(int index, GamepadButton button) =>
        Raylib.IsGamepadButtonReleased(index, button);

    /// <summary>
    /// Get Gamepad axis value
    /// </summary>
    /// <param name="index">Index of Gamepad</param>
    /// <param name="axis">Axis of Gamepad</param>
    /// <returns>Value</returns>
    public static float GetGamePadAxis(int index, GamepadAxis axis) =>
        Raylib.GetGamepadAxisMovement(index, axis);

    /// <summary>
    /// Updates the input.
    /// </summary>
    internal static void UpdateInput()
    {
        InternalPressedChars.Clear();
        InternalPressedKeys.Clear();

        var key = Raylib.GetKeyPressed();
        while (key > 0)
        {
            InternalPressedKeys.Add(key);
            key = Raylib.GetKeyPressed();
        }

        var charGot = Raylib.GetCharPressed();
        while (charGot > 0)
        {
            InternalPressedChars.Add(charGot);
            charGot = Raylib.GetCharPressed();
        }
    }
}

