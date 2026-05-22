using Raylib_cs;

namespace Engine;

/// <summary>
/// Draws rectangle lines extended.
/// </summary>
internal record DrawRectangleLinesEx(Rectangle Rect, float BorderSize, Color BorderColor)
    : Instruction
{
    internal override void Execute()
    {
        base.Execute();
        Raylib.DrawRectangleLinesEx(Rect, BorderSize, BorderColor);
    }
}
