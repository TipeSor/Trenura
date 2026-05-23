using System.Numerics;
using Engine;
using Raylib_cs;

public class CoinChangePopup : Label
{
    private readonly float _speed;
    private float _timeLeft;

    public CoinChangePopup(
        int amount,
        Vector2 position,
        string? font = null,
        float fontSize = 24,
        float speed = 48,
        float lifetime = 0.4f,
        int zLayer = 101)
        : base(
            position,
            $"{amount:+0;-0;0}",
            font,
            amount > 0 ? Color.Green : Color.Red,
            fontSize,
            zLayer
        )
    {
        _speed = speed;
        _timeLeft = lifetime;
        HorizontalAlign = HorizontalAlignment.Left;
        VerticalAlign = VerticalAlignment.Center;
    }

    public override void Update(float delta)
    {
        base.Update(delta);

        Position += new Vector2(0, _speed * delta);
        _timeLeft -= delta;

        if (_timeLeft <= 0 && Scene != null)
            Scene.RemoveWidget(this);
    }
}
