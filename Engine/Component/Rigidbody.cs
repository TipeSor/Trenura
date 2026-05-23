using System.Numerics;

namespace Engine;

public class Rigidbody : Component
{
    public Vector2 Velocity { get; set; }
    
    public RigidbodyType BodyKind { get; set; }
    public RigidbodyConstraint Constraints { get; set; }

    public float GravityScale { get; set; }

    public RigidbodyInterpolation Interpolation { get; set; }

    public float Mass { get; set; }
    public bool Simulated { get; set; }

    public Rigidbody()
    {
        Velocity = Vector2.Zero;
        BodyKind = RigidbodyType.Dynamic;
        Constraints = RigidbodyConstraint.None;

        GravityScale = 1.0f;

        Interpolation = RigidbodyInterpolation.None;

        Mass = 1.0f;
        Simulated = true;
    }
}

public enum RigidbodyType
{
    Dynamic,
    Kinematic,
    Static,
}

[Flags]
public enum RigidbodyConstraint
{
    None            = 0,
    FreezePositionX = 1,
    FreezePositionY = 2,
    FreezeRotation  = 4,

    FreezePosition  = FreezePositionX | FreezePositionY,
    FreezeAll       = FreezePosition  | FreezeRotation,
}

public enum RigidbodyInterpolation
{
    None,
    Interpolate,
    Extrapolate,
}
