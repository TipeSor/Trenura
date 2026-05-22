using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Draws a texture with pro-parameters.
/// </summary>
internal record DrawTexturePro(
    Texture2D Texture,
    Rectangle SourceRectangle,
    Rectangle DestinationRectangle,
    Vector2 Origin,
    float Rotation,
    Color Tint
) : Instruction
{
    internal override void Execute()
    {
        base.Execute();
        Raylib.DrawTexturePro(
            Texture,
            SourceRectangle,
            DestinationRectangle,
            Origin,
            Rotation,
            Tint
        );
    }
}
