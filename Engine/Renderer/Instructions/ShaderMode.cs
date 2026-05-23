using Raylib_cs;

namespace Engine;

/// <summary>
/// Draws in shader mode.
/// </summary>
internal record ShaderMode(Shader Shader, List<Instruction> Instructions) : Instruction
{
    internal override void Execute()
    {
        base.Execute();
        Raylib.BeginShaderMode(Shader);
        Renderer.DrawInstructions(Instructions);
        Raylib.EndShaderMode();
    }
}
