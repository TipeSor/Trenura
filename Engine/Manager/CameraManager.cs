using System;
using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Class which manager Camera information
/// </summary>
public static class CameraManager
{
    /// <summary>
    /// Rotation of Camera
    /// </summary>
    public static float Rotation
    {
        get => Camera2D.Rotation;
        set => Camera2D.Rotation = value;
    }

    /// <summary>
    /// Zoom of Camera
    /// </summary>
    public static float Zoom
    {
        get => Camera2D.Zoom;
        set => Camera2D.Zoom = value;
    }

    /// <summary>
    /// Position of Camera
    /// </summary>
    public static Vector2 Position
    {
        get => Camera2D.Target;
        set => Camera2D.Target = value;
    }

    /// <summary>
    /// Size of the camera view in world units
    /// </summary>
    public static Vector2 ViewSize => (Camera2D.Offset * 2.0f) / Zoom;

    /// <summary>
    /// Half size of the camera view in world units
    /// </summary>
    public static Vector2 HalfViewSize => Camera2D.Offset / Zoom;

    internal static Camera2D Camera2D = new Camera2D(Vector2.Zero, Vector2.Zero, 0, 1);

    internal static void SetScreenSize(Vector2 screenSize)
    {
        Camera2D.Offset = screenSize / 2;
        Camera2D.Target = Camera2D.Offset;
    }

    /// <summary>
    /// Transform position from screen space to world space
    /// </summary>
    /// <param name="screen">Position in screen space</param>
    /// <returns>Position in world space</returns>
    public static Vector2 ScreenToWorld(Vector2 screen)
    {
        var mat = GetCameraMatrix();
        Matrix4x4.Invert(mat, out var inv);

        Vector3 result = Vector3.Transform(new Vector3(screen.X, screen.Y, 0), inv);
        return new Vector2(result.X, result.Y);
    }

    /// <summary>
    /// Transform position from world space to screen space
    /// </summary>
    /// <param name="world">Position in world space</param>
    /// <returns>Position in screen space</returns>
    public static Vector2 WorldToScreen(Vector2 world)
    {
        var mat = GetCameraMatrix();
        Vector3 result = Vector3.Transform(new Vector3(world.X, world.Y, 0), mat);
        return new Vector2(result.X, result.Y);
    }

    /// <summary>
    /// Get camera transformation matrix
    /// </summary>
    /// <returns>Transformation matrix from world space to screen space</returns>
    public static Matrix4x4 GetCameraMatrix()
    {
        float rot = MathF.PI / 180f * Rotation;

        return
            Matrix4x4.CreateTranslation(-Position.X, -Position.Y, 0) *
            Matrix4x4.CreateRotationZ(rot) *
            Matrix4x4.CreateScale(Zoom, Zoom, 1) *
            Matrix4x4.CreateTranslation(Camera2D.Offset.X, Camera2D.Offset.Y, 0);
    }
}
