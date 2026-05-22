using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Draws text with pro-parameters.
/// </summary>
internal record DrawTextPro(
    Font Font,
    string Text,
    Vector2 Position,
    Vector2 Origin,
    float Rotation,
    float FontSize,
    float Spacing,
    Color Color
) : Instruction
{
    internal override void Execute()
    {
        base.Execute();
        Raylib.DrawTextPro(Font, Text, Position, Origin, Rotation, FontSize, Spacing, Color);
    }
}
