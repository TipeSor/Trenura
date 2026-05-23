using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Widget that displays a colored panel
/// </summary>
public class Panel : Widget
{
    /// <summary>
    /// Size of panel
    /// </summary>
    public Vector2 Size { get; set; }

    /// <summary>
    /// Background color
    /// </summary>
    public Color BackgroundColor { get; set; }

    /// <summary>
    /// Border color
    /// </summary>
    public Color BorderColor { get; set; }

    /// <summary>
    /// Border thickness
    /// </summary>
    public float BorderThickness { get; set; }

    public Panel(
        Vector2 position,
        Vector2? size = null,
        Color? backgroundColor = null,
        Color? borderColor = null,
        float borderThickness = 0,
        int zLayer = 0)
        : base(position, zLayer)
    {
        Size = size ?? new Vector2(200, 100);
        BackgroundColor = backgroundColor ?? Color.Gray;
        BorderColor = borderColor ?? Color.Black;
        BorderThickness = borderThickness;
    }

    protected override Rectangle CalculateBounds()
    {
        return GetAlignedBounds(Size);
    }

    public override void Draw()
    {
        if (!RealDisplayed || !IsVisibleOnScreen)
            return;

        Renderer.DrawRectangle(
            Bounds.X,
            Bounds.Y,
            Bounds.Width,
            Bounds.Height,
            BackgroundColor,
            InstructionSource.Ui,
            ZLayer
        );

        if (BorderThickness > 0)
            Renderer.DrawRectangleLines(
                Bounds,
                BorderThickness,
                BorderColor,
                InstructionSource.Ui,
                ZLayer + 0.00001f
            );

        base.Draw();
    }
}
