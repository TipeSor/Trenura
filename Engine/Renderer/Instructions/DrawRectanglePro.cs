using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Draws a rectangle with pro-parameters.
/// </summary>
internal record DrawRectanglePro(Rectangle Rectangle, Vector2 Origin, float Rotation, Color Color)
    : Instruction
{
    internal override void Execute()
    {
        base.Execute();
        Raylib.DrawRectanglePro(Rectangle, Origin, Rotation, Color);
    }
}
