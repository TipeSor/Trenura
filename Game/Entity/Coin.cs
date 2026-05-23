using System.Numerics;
using Engine;
using Raylib_cs;

public class Coin : Entity
{
    public Transform Transform { get; }
    public int Value { get; }

    public Coin(Vector2 position, int value)
    {
        Value = value;

        Transform = AddComponent(new Transform(position, Vector2.One * 32));

        BoxCollider collider = AddComponent(new BoxCollider()
        {
            IsTrigger = true,
        });

        collider.OnEnter += (other) => {
            GameManager.Coins += value;
            Active = false;
        };

        AddComponent(new SpriteRenderer(
            "coin",
            tint: value < 0 ? new Color(255, 96, 96, 255) : Color.White
        ));
    }

    public void Reset()
    {
        Active = true;
    }
}
