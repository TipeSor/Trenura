using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Draws a rectangle v.
/// </summary>
internal record DrawRectangleV(Vector2 Position, Vector2 Size, Color Color) : Instruction
{
    internal override void Execute()
    {
        base.Execute();
        Raylib.DrawRectangleV(Position, Size, Color);
    }
}
