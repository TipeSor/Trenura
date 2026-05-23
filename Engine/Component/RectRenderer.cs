using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Component that renders a rectangle using the Transform center position
/// </summary>
public class RectRenderer : Component
{
    /// <summary>
    /// Size of rectangle
    /// </summary>
    public Vector2 Size { get; set; }

    /// <summary>
    /// Color of rectangle
    /// </summary>
    public Color Color { get; set; }

    /// <summary>
    /// ZLayer offset of rectangle
    /// </summary>
    public int ZLayerOffset { get; set; }

    private Transform? transform;

    /// <summary>
    /// Create RectRenderer
    /// </summary>
    /// <param name="color">Color of rectangle</param>
    /// <param name="size">Size of rectangle</param>
    /// <param name="zLayerOffset">ZLayer offset of rectangle</param>
    public RectRenderer(Color color, Vector2? size = null, int zLayerOffset = 0)
    {
        Color = color;
        Size = size ?? Vector2.One;
        ZLayerOffset = zLayerOffset;
    }

    /// <summary>
    /// Load RectRenderer
    /// </summary>
    public override void Load()
    {
        transform = Entity?.GetComponent<Transform>();
    }

    /// <summary>
    /// Draw rectangle
    /// </summary>
    public override void Draw()
    {
        if (transform == null)
            return;

        Vector2 size = Size * transform.Scale;
        Vector2 position = transform.Position - (size * 0.5f);

        Renderer.DrawRectangle(position, size, Color, InstructionSource.Entity, transform.ZLayer + ZLayerOffset);
    }
}
