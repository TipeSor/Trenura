using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Draws circle.
/// </summary>
internal record DrawCircleV(Vector2 Center, float Radius, Color Color) : Instruction
{
    internal override void Execute()
    {
        base.Execute();
        Raylib.DrawCircleV(Center, Radius, Color);
    }
}
