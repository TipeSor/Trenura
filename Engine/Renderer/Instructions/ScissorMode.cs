using Raylib_cs;

namespace Engine;

/// <summary>
/// Draws in scissored mode.
/// </summary>
internal record ScissorMode(
    float PosX,
    float PosY,
    float Width,
    float Height,
    List<Instruction> Instructions
) : Instruction
{
    internal override void Execute()
    {
        base.Execute();
        Raylib.BeginScissorMode((int)PosX, (int)PosY, (int)Width, (int)Height);
        Render.DrawInstructions(Instructions);
        Raylib.EndScissorMode();
    }
}
