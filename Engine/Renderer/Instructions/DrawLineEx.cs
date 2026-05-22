using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Draws line.
/// </summary>
internal record DrawLineEx(Vector2 StartPos, Vector2 EndPos, float Thick, Color Color) : Instruction
{
    internal override void Execute()
    {
        base.Execute();
        Raylib.DrawLineEx(StartPos, EndPos, Thick, Color);
    }
}
