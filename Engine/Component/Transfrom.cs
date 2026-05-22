using System.Numerics;
using Engine;


/// <summary>
/// Component that defines Transform (Position, Rotation, Scale)
/// </summary>
/// <param name="position">Position (Vector2(0))</param>
/// <param name="scale">Scale (Vector2(1))</param>
/// <param name="rotation">Rotation (0)</param>
/// <param name="zLayer">ZLayer (0)</param>
public class Transform(
    Vector2? position = null,
    Vector2? scale = null,
    float rotation = 0,
    int zLayer = 0
) : Component
{
    /// <summary>
    /// Position of Component
    /// </summary>
    public Vector2 Position => Entity?.Parent?.GetComponent<Transform>()?.Position is { } parentPosition ? parentPosition + LocalPosition : LocalPosition;

    /// <summary>
    /// Scale of Component
    /// </summary>
    public Vector2 Scale => Entity?.Parent?.GetComponent<Transform>()?.Scale is { } parentScale ? parentScale * LocalScale : LocalScale;

    /// <summary>
    /// Rotation of Component
    /// </summary>
    public float Rotation => Entity?.Parent?.GetComponent<Transform>()?.Rotation + LocalRotation ?? LocalRotation;

    /// <summary>
    /// ZLayer of Component
    /// </summary>
    public int ZLayer => Entity?.Parent?.GetComponent<Transform>()?.ZLayer + LocalZLayer ?? LocalZLayer;

    /// <summary>
    /// Local Position of Component
    /// </summary>
    public Vector2 LocalPosition { get; set; } = position ?? Vector2.Zero;

    /// <summary>
    /// Local Scale of Component
    /// </summary>
    public Vector2 LocalScale { get; set; } = scale ?? Vector2.One;

    /// <summary>
    /// Local Rotation of Component
    /// </summary>
    public float LocalRotation { get; set; } = rotation;

    /// <summary>
    /// Local ZLayer of Component
    /// </summary>
    public int LocalZLayer { get; set; } = zLayer;

    /// <summary>
    /// Get transformed Position
    /// </summary>
    /// <param name="offset">Offset (Vector2(0))</param>
    /// <returns>Transformed Position</returns>
    public Vector2 GetTransformedPosition(Vector2? offset = null) => offset == null ? Position : Position + offset.Value;
}

