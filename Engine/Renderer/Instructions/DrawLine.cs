using Raylib_cs;

namespace Engine;

/// <summary>
/// Draws line.
/// </summary>
internal record DrawLine(int StartPosX, int StartPosY, int EndPosX, int EndPosY, Color Color) : Instruction
{
    internal override void Execute()
    {
        base.Execute();
        Raylib.DrawLine(StartPosX, StartPosY, EndPosX, EndPosY, Color);
    }
}
