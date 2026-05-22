using Raylib_cs;

namespace Engine;

/// <summary>
/// Draws a rectangle.
/// </summary>
internal record DrawRectangle(float PosX, float PosY, float Width, float Height, Color Color)
    : Instruction
{
    internal override void Execute()
    {
        base.Execute();
        Raylib.DrawRectangle((int)PosX, (int)PosY, (int)Width, (int)Height, Color);
    }
}
