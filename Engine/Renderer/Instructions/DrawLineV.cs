using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Draws line.
/// </summary>
internal record DrawLineV(Vector2 StartPos, Vector2 EndPos, Color Color) : Instruction
{
    internal override void Execute()
    {
        base.Execute();
        Raylib.DrawLineV(StartPos, EndPos, Color);
    }
}
