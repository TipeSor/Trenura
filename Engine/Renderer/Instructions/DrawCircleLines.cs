using Raylib_cs;

namespace Engine;

/// <summary>
/// Draws circle lines.
/// </summary>
internal record DrawCircleLines(int CircleX, int CircleY, float Radius, Color Color) : Instruction
{
    internal override void Execute()
    {
        base.Execute();
        Raylib.DrawCircleLines(CircleX, CircleY, Radius, Color);
    }
}
