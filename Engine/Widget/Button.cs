using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Widget that displays a clickable button
/// </summary>
public class Button : Widget
{
    protected enum ButtonState
    {
        Idle,
        Down,
        Hover,
    }

    /// <summary>
    /// Text of button
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Font name of button
    /// </summary>
    public string? Font { get; set; }

    /// <summary>
    /// Size of button
    /// </summary>
    public Vector2 Size { get; set; }

    /// <summary>
    /// Font color
    /// </summary>
    public Color FontColor { get; set; }

    /// <summary>
    /// Background color
    /// </summary>
    public Color BackgroundColor { get; set; }

    /// <summary>
    /// Font size
    /// </summary>
    public float FontSize { get; set; }

    /// <summary>
    /// Text spacing
    /// </summary>
    public float Spacing { get; set; }

    /// <summary>
    /// Event triggered when button is clicked
    /// </summary>
    public event Action? Clicked;

    protected ButtonState State = ButtonState.Idle;

    public Button(
        Vector2 position,
        string text = "",
        string? font = null,
        Vector2? size = null,
        Color? fontColor = null,
        Color? backgroundColor = null,
        float? fontSize = null,
        int zLayer = 0)
        : base(position, zLayer)
    {
        Text = text;
        Font = font;
        Size = size ?? new Vector2(200, 40);
        FontColor = fontColor ?? Color.Black;
        BackgroundColor = backgroundColor ?? Color.Gray;
        FontSize = fontSize ?? Raylib.GetFontDefault().BaseSize;
        Spacing = 2;
    }

    protected override Rectangle CalculateBounds()
    {
        return GetAlignedBounds(Size);
    }

    public override void Update(float delta)
    {
        base.Update(delta);

        if (!RealActive || !RealDisplayed)
        {
            State = ButtonState.Idle;
            return;
        }

        bool hovered = ConsumesMouse
            ? InputManager.TryConsumeMouse(Bounds)
            : InputManager.IsMouseInRectangle(Bounds);

        if (!hovered)
        {
            State = ButtonState.Idle;
            return;
        }

        State = ButtonState.Hover;

        if (InputManager.IsMouseButtonPressed(MouseButton.Left))
            Clicked?.Invoke();

        if (InputManager.IsMouseButtonDown(MouseButton.Left))
            State = ButtonState.Down;
    }

    public override void Draw()
    {
        if (!RealDisplayed || !IsVisibleOnScreen)
            return;

        if (State == ButtonState.Hover && RealActive)
            Renderer.DrawRectangle(
                Bounds.X - 2,
                Bounds.Y - 2,
                Bounds.Width + 4,
                Bounds.Height + 4,
                Color.White,
                InstructionSource.Ui,
                ZLayer
            );

        Renderer.DrawRectangle(
            Bounds.X,
            Bounds.Y,
            Bounds.Width,
            Bounds.Height,
            Color.Black,
            InstructionSource.Ui,
            ZLayer + 0.00001f
        );

        Renderer.DrawRectangle(
            Bounds.X + 2,
            Bounds.Y + 2,
            Bounds.Width - 4,
            Bounds.Height - 4,
            BackgroundColor,
            InstructionSource.Ui,
            ZLayer + 0.00002f
        );

        if (!string.IsNullOrEmpty(Text))
        {
            Font font = Font != null && FontManager.HasFont(Font)
                ? FontManager.GetFont(Font)
                : Raylib.GetFontDefault();

            Vector2 textSize = Raylib.MeasureTextEx(font, Text, FontSize, Spacing);
            Renderer.DrawText(
                font,
                Text,
                RealPosition,
                textSize * 0.5f,
                0,
                FontSize,
                Spacing,
                FontColor,
                InstructionSource.Ui,
                ZLayer + 0.00003f
            );
        }

        if (State == ButtonState.Down || !RealActive)
            Renderer.DrawRectangle(
                Bounds.X,
                Bounds.Y,
                Bounds.Width,
                Bounds.Height,
                new Color(0, 0, 0, 128),
                InstructionSource.Ui,
                ZLayer + 0.00004f
            );

        base.Draw();
    }
}
