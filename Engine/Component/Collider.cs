using System.Numerics;
using Raylib_cs;

namespace Engine;

public abstract class Collider : Component
{
    public Vector2 Offset { get; set; }
    public bool IsTrigger { get; set; }
    public bool Enabled { get; set; }
    public bool DrawBoundingBox { get; set; }

    public event Action<Collider>? OnEnter;
    public event Action<Collider>? OnStay;
    public event Action<Collider>? OnExit;

    public Rigidbody? AttachedRigidbody => Entity?.GetComponent<Rigidbody>();
    public Transform? Transform => Entity?.GetComponent<Transform>();

    protected Collider()
    {
        Offset = Vector2.Zero;
        Enabled = true;
    }

    public bool CanParticipateInPhysics => Enabled && Transform != null;

    public abstract PhysicsShape Shape { get; }

    public PhysicsAabb GetWorldBounds()
    {
        if (Transform == null)
            return PhysicsAabb.Empty;

        return Shape.GetWorldBounds(Transform, Offset);
    }

    internal void InvokeOnEnter(Collider other) => OnEnter?.Invoke(other);

    internal void InvokeOnStay(Collider other) => OnStay?.Invoke(other);

    internal void InvokeOnExit(Collider other) => OnExit?.Invoke(other);

    public override void Draw()
    {
        if (!DrawBoundingBox)
            return;

        PhysicsAabb bounds = GetWorldBounds();
        if (bounds.IsEmpty)
            return;

        float zLayer = Transform?.ZLayer ?? 0;
        Color color = IsTrigger ? Color.Orange : Color.Lime;

        Renderer.DrawRectangleLines(bounds.ToRectangle(), 1.0f, color, InstructionSource.Entity, zLayer);
    }
}

public sealed class BoxCollider : Collider
{
    public Vector2 Size { get; set; }

    public override PhysicsShape Shape => new(PhysicsShapeType.Box, Size);

    public BoxCollider()
    {
        Size = Vector2.One;
    }
}

public readonly record struct PhysicsShape(PhysicsShapeType Type, Vector2 Size)
{
    public PhysicsAabb GetWorldBounds(Transform transform, Vector2 offset)
    {
        return Type switch
        {
            PhysicsShapeType.Box => PhysicsAabb.FromPositionSize(transform.Position, offset, Size, transform.Scale),
            _ => PhysicsAabb.Empty,
        };
    }
}

public readonly record struct PhysicsAabb(Vector2 Min, Vector2 Max)
{
    public static PhysicsAabb Empty => new(Vector2.Zero, Vector2.Zero);

    public float Width => Max.X - Min.X;
    public float Height => Max.Y - Min.Y;

    public bool IsEmpty => Width <= 0 || Height <= 0;

    public bool Intersects(PhysicsAabb other)
    {
        return !IsEmpty
            && !other.IsEmpty
            && Min.X < other.Max.X
            && Max.X > other.Min.X
            && Min.Y < other.Max.Y
            && Max.Y > other.Min.Y;
    }

    public static PhysicsAabb FromPositionSize(Vector2 position, Vector2 offset, Vector2 size, Vector2 scale)
    {
        Vector2 scaledSize = size * scale;
        Vector2 scaledOffset = offset * scale;
        Vector2 center = position + scaledOffset;
        Vector2 halfSize = scaledSize * 0.5f;
        Vector2 start = center - halfSize;
        Vector2 end = center + halfSize;

        return new(Vector2.Min(start, end), Vector2.Max(start, end));
    }

    public Rectangle ToRectangle() => new(Min.X, Min.Y, Width, Height);
}

public enum PhysicsShapeType
{
    Box,
    Circle,
    Capsule,
    Polygon,
    Edge,
}
