using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Draws text with extended parameters.
/// </summary>
internal record DrawTextEx(
    Font Font,
    string Text,
    Vector2 Position,
    float FontSize,
    float Spacing,
    Color Color
) : Instruction
{
    internal override void Execute()
    {
        base.Execute();
        Raylib.DrawTextEx(Font, Text, Position, FontSize, Spacing, Color);
    }
}
