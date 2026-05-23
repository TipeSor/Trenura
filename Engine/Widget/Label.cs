using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Widget that displays text
/// </summary>
public class Label : Widget
{
    /// <summary>
    /// Text of label
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Font name of label
    /// </summary>
    public string? Font { get; set; }

    /// <summary>
    /// Font color
    /// </summary>
    public Color FontColor { get; set; }

    /// <summary>
    /// Font size
    /// </summary>
    public float FontSize { get; set; }

    /// <summary>
    /// Text spacing
    /// </summary>
    public float Spacing { get; set; }

    public Label(
        Vector2 position,
        string text = "",
        string? font = null,
        Color? fontColor = null,
        float? fontSize = null,
        int zLayer = 0)
        : base(position, zLayer)
    {
        Text = text;
        Font = font;
        FontColor = fontColor ?? Color.Black;
        FontSize = fontSize ?? Raylib.GetFontDefault().BaseSize;
        Spacing = 2;
        ConsumesMouse = false;
    }

    protected override Rectangle CalculateBounds()
    {
        Font font = Font != null && FontManager.HasFont(Font)
            ? FontManager.GetFont(Font)
            : Raylib.GetFontDefault();

        Vector2 textSize = Raylib.MeasureTextEx(font, Text, FontSize, Spacing);
        return GetAlignedBounds(textSize);
    }

    public override void Draw()
    {
        if (!RealDisplayed || !IsVisibleOnScreen || string.IsNullOrEmpty(Text))
            return;

        Font font = Font != null && FontManager.HasFont(Font)
            ? FontManager.GetFont(Font)
            : Raylib.GetFontDefault();

        Renderer.DrawText(
            font,
            Text,
            RealPosition,
            GetAlignmentOffset(Bounds.Size),
            0,
            FontSize,
            Spacing,
            FontColor,
            InstructionSource.Ui,
            ZLayer
        );

        base.Draw();
    }
}
