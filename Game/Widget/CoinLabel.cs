using System.Numerics;
using Engine;
using Raylib_cs;

public class CoinLabel : Widget
{
    public Vector2 Size { get; set; }
    public Label Label { get; }

    public string Text
    {
        get => Label.Text;
        set => Label.Text = value;
    }

    public string? Font
    {
        get => Label.Font;
        set => Label.Font = value;
    }

    public float FontSize
    {
        get => Label.FontSize;
        set => Label.FontSize = value;
    }

    public CoinLabel(
        Vector2? position = null,
        Vector2? size = null,
        string text = "Coins: 0",
        string? font = null,
        float fontSize = 28,
        int zLayer = 99)
        : base(position ?? Vector2.Zero, zLayer)
    {
        Size = size ?? new Vector2(256, 32);
        HorizontalAlign = HorizontalAlignment.Left;
        VerticalAlign = VerticalAlignment.Top;
        ConsumesMouse = false;

        Panel panel = AddChild(new Panel(
            Size * 0.5f,
            Size,
            new Color(36, 24, 20, 220),
            Color.Beige,
            2,
            zLayer
        ));
        panel.ConsumesMouse = false;

        Label = AddChild(new Label(
            new Vector2(8, 4),
            text,
            font,
            Color.Gold,
            fontSize,
            zLayer + 1
        ));
        Label.HorizontalAlign = HorizontalAlignment.Left;
        Label.VerticalAlign = VerticalAlignment.Top;
        Label.ConsumesMouse = false;
    }

    protected override Rectangle CalculateBounds()
    {
        return GetAlignedBounds(Size);
    }
}
