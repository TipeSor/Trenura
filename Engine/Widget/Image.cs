using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Widget that displays a texture
/// </summary>
public class Image : Widget
{
    /// <summary>
    /// Texture name from texture manager
    /// </summary>
    public string Texture { get; set; }

    /// <summary>
    /// Size of image
    /// </summary>
    public Vector2 Size { get; set; }

    /// <summary>
    /// Tint color
    /// </summary>
    public Color Tint { get; set; }

    public Image(
        Vector2 position,
        string texture,
        Vector2? size = null,
        Color? tint = null,
        int zLayer = 0)
        : base(position, zLayer)
    {
        Texture = texture;
        Size = size ?? GetTextureSize(texture);
        Tint = tint ?? Color.White;
        ConsumesMouse = false;
    }

    protected override Rectangle CalculateBounds()
    {
        Vector2 size = Size == Vector2.Zero ? GetTextureSize(Texture) : Size;
        return GetAlignedBounds(size);
    }

    public override void Draw()
    {
        if (!RealDisplayed || !IsVisibleOnScreen || !TextureManager.HasTexture(Texture))
            return;

        Texture2D texture = TextureManager.GetTexture(Texture);

        Renderer.DrawTexture(
            texture,
            new Rectangle(0, 0, texture.Width, texture.Height),
            Bounds,
            GetAlignmentOffset(Bounds.Size),
            0,
            Tint,
            InstructionSource.Ui,
            ZLayer
        );

        base.Draw();
    }

    private static Vector2 GetTextureSize(string texture)
    {
        if (!TextureManager.HasTexture(texture))
            return Vector2.Zero;

        Texture2D texture2D = TextureManager.GetTexture(texture);
        return new Vector2(texture2D.Width, texture2D.Height);
    }
}
