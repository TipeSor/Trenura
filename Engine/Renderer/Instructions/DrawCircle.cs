using Raylib_cs;

namespace Engine;

/// <summary>
/// Draws circle.
/// </summary>
internal record DrawCircle(int CircleX, int CircleY, float Radius, Color Color) : Instruction
{
    internal override void Execute()
    {
        base.Execute();
        Raylib.DrawCircle(CircleX, CircleY, Radius, Color);
    }
}
