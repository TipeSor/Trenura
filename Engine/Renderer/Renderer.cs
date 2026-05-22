using System.Numerics;
using Raylib_cs;

namespace Engine;

/// <summary>
/// Static class which used to render textures, texts or rectangles
/// </summary>
// ReSharper disable once InconsistentNaming
public static class Render
{
    /// <summary>
    /// Number of Last Instructions
    /// </summary>
    public static int LastInstructionsNumber { get; set; }

    /// <summary>
    /// Number of Last Entity Instructions
    /// </summary>
    public static int LastEntityInstructionsNumber { get; set; }

    /// <summary>
    /// Number of Last UI Instructions
    /// </summary>
    public static int LastUiInstructionsNumber { get; set; }

    /// <summary>
    /// Current Instructions to be rendered
    /// </summary>
    private static List<Instruction> _instructions = [];

    internal static void DrawInstructions(List<Instruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            LastInstructionsNumber++;
            if (instruction.Source == InstructionSource.Entity)
                LastEntityInstructionsNumber++;
            else
                LastUiInstructionsNumber++;
            instruction.Execute();
        }
    }

    /// <summary>
    /// Draw all instructions in the Window
    /// </summary>
    /// <param name="window">Window</param>
    public static void Draw(Window window)
    {
        LastInstructionsNumber = 0;
        LastEntityInstructionsNumber = 0;
        LastUiInstructionsNumber = 0;
        List<Instruction> entityInstructions = [];
        List<Instruction> uiInstructions = [];

        foreach (var instruction in _instructions)
        {
            switch (instruction.Source)
            {
                case InstructionSource.Entity:
                    entityInstructions.Add(instruction);
                    break;
                case InstructionSource.Ui:
                    uiInstructions.Add(instruction);
                    break;
                default:
                    throw new ArgumentException("Unknown instruction source");
            }
        }

        entityInstructions.Sort((i1, i2) => i1.ZLayer.CompareTo(i2.ZLayer));
        uiInstructions.Sort((i1, i2) => i1.ZLayer.CompareTo(i2.ZLayer));

        Raylib.BeginMode2D(CameraManager.Camera2D);
        DrawInstructions(entityInstructions);
        Raylib.EndMode2D();

        DrawInstructions(uiInstructions);

        _instructions.Clear();
    }

    /// <summary>
    /// Add Shader Mode Instructions
    /// </summary>
    /// <param name="shader">Shader</param>
    /// <param name="source">Instruction Source</param>
    /// <param name="zLayer">Z Layer</param>
    /// <param name="shaderAction">Function that renders in shader mode</param>
    public static void ShaderMode(
        Shader shader,
        InstructionSource source,
        float zLayer,
        Action shaderAction
    )
    {
        var instructions = new List<Instruction>(_instructions);
        _instructions.Clear();
        shaderAction();
        var instruction = new ShaderMode(shader, new List<Instruction>(_instructions))
        {
            Source = source,
            ZLayer = zLayer
        };
        _instructions = instructions;
        _instructions.Add(instruction);
    }

    /// <summary>
    /// Add Scissor Mode Instructions
    /// </summary>
    /// <param name="posX">Position X</param>
    /// <param name="posY">Position Y</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    /// <param name="source">Instruction Source</param>
    /// <param name="zLayer">Z Layer</param>
    /// <param name="scissorAction">Function that renders in scissored mode</param>
    public static void ScissorMode(
        float posX,
        float posY,
        float width,
        float height,
        InstructionSource source,
        float zLayer,
        Action scissorAction
    )
    {
        var instructions = new List<Instruction>(_instructions);
        _instructions.Clear();
        scissorAction();
        var instruction = new ScissorMode(
            posX,
            posY,
            width,
            height,
            new List<Instruction>(_instructions)
        )
        {
            Source = source,
            ZLayer = zLayer
        };
        _instructions = instructions;
        _instructions.Add(instruction);
    }

    /// <summary>
    /// Add Draw Line Instruction
    /// </summary>
    /// <param name="startX">Start X</param>
    /// <param name="startY">Start Y</param>
    /// <param name="endX">End X</param>
    /// <param name="endY">End Y</param>
    /// <param name="color">Color</param>
    /// <param name="source">Instruction Source</param>
    /// <param name="zLayer">Z Layer</param>
    public static void DrawLine(
        int startX,
        int startY,
        int endX,
        int endY,
        Color color,
        InstructionSource source,
        float zLayer
    )
    {
        _instructions.Add(
            new DrawLine(startX, startY, endX, endY, color)
            {
                Source = source,
                ZLayer = zLayer
            }
        );
    }

    /// <summary>
    /// Add Draw Line V Instruction
    /// </summary>
    /// <param name="startPos">Start Position</param>
    /// <param name="endPos">End Position</param>
    /// <param name="color">Color</param>
    /// <param name="source">Instruction Source</param>
    /// <param name="zLayer">Z Layer</param>
    public static void DrawLine(
        Vector2 startPos,
        Vector2 endPos,
        Color color,
        InstructionSource source,
        float zLayer
    )
    {
        _instructions.Add(
            new DrawLineV(startPos, endPos, color)
            {
                Source = source,
                ZLayer = zLayer
            }
        );
    }

    /// <summary>
    /// Add Draw Line Ex Instruction
    /// </summary>
    /// <param name="startPos">Start Position</param>
    /// <param name="endPos">End Position</param>
    /// <param name="thick">Thickness</param>
    /// <param name="color">Color</param>
    /// <param name="source">Instruction Source</param>
    /// <param name="zLayer">Z Layer</param>
    public static void DrawLine(
        Vector2 startPos,
        Vector2 endPos,
        float thick,
        Color color,
        InstructionSource source,
        float zLayer
    )
    {
        _instructions.Add(
            new DrawLineEx(startPos, endPos, thick, color)
            {
                Source = source,
                ZLayer = zLayer
            }
        );
    }

    /// <summary>
    /// Add Draw Rectangle Pro Instruction
    /// </summary>
    /// <param name="rectangle">Rectangle</param>
    /// <param name="origin">Origin</param>
    /// <param name="rotation">Rotation</param>
    /// <param name="color">Color</param>
    /// <param name="source">Instruction Source</param>
    /// <param name="zLayer">Z Layer</param>
    public static void DrawRectangle(
        Rectangle rectangle,
        Vector2 origin,
        float rotation,
        Color color,
        InstructionSource source,
        float zLayer
    )
    {
        _instructions.Add(
            new DrawRectanglePro(rectangle, origin, rotation, color)
            {
                Source = source,
                ZLayer = zLayer
            }
        );
    }

    /// <summary>
    /// Add Draw Rectangle Instruction
    /// </summary>
    /// <param name="posX">Position X</param>
    /// <param name="posY">Position Y</param>
    /// <param name="width">Width</param>
    /// <param name="height">Height</param>
    /// <param name="color">Color</param>
    /// <param name="source">Instruction Source</param>
    /// <param name="zLayer">Z Layer</param>
    public static void DrawRectangle(
        float posX,
        float posY,
        float width,
        float height,
        Color color,
        InstructionSource source,
        float zLayer
    )
    {
        _instructions.Add(
            new DrawRectangle(posX, posY, width, height, color)
            {
                Source = source,
                ZLayer = zLayer
            }
        );
    }
    /// <summary>
    /// Add Draw Rectangle V Instruction
    /// </summary>
    /// <param name="position">Position</param>
    /// <param name="size">Size</param>
    /// <param name="color">Color</param>
    /// <param name="source">Instruction Source</param>
    /// <param name="zLayer">Z Layer</param>
    public static void DrawRectangle(
        Vector2 position,
        Vector2 size,
        Color color,
        InstructionSource source,
        float zLayer
    )
    {
        _instructions.Add(
            new DrawRectangleV(position, size, color)
            {
                Source = source,
                ZLayer = zLayer
            }
        );
    }

    /// <summary>
    /// Add Draw Rectangle Lines Ex Instruction
    /// </summary>
    /// <param name="rect">Rectangle</param>
    /// <param name="borderSize">Border Size</param>
    /// <param name="borderColor">Border Color</param>
    /// <param name="source">Instruction Source</param>
    /// <param name="zLayer">Z Layer</param>
    public static void DrawRectangleLines(
        Rectangle rect,
        float borderSize,
        Color borderColor,
        InstructionSource source,
        float zLayer
    )
    {
        _instructions.Add(
            new DrawRectangleLinesEx(rect, borderSize, borderColor)
            {
                Source = source,
                ZLayer = zLayer
            }
        );
    }

    /// <summary>
    /// Add Draw Circle Instruction
    /// </summary>
    /// <param name="posX">Position X</param>
    /// <param name="posY">Position Y</param>
    /// <param name="radius">Radius</param>
    /// <param name="color">Color</param>
    /// <param name="source">Instruction Source</param>
    /// <param name="zLayer">Z Layer</param>
    public static void DrawCircle(
        float posX,
        float posY,
        float radius,
        Color color,
        InstructionSource source,
        float zLayer
    )
    {
        _instructions.Add(
            new DrawCircle((int)posX, (int)posY, radius, color)
            {
                Source = source,
                ZLayer = zLayer
            }
        );
    }

    /// <summary>
    /// Add Draw Circle V Instruction
    /// </summary>
    /// <param name="pos">Position</param>
    /// <param name="radius">Radius</param>
    /// <param name="color">Color</param>
    /// <param name="source">Instruction Source</param>
    /// <param name="zLayer">Z Layer</param>
    public static void DrawCircle(
        Vector2 pos,
        float radius,
        Color color,
        InstructionSource source,
        float zLayer
    )
    {
        _instructions.Add(
            new DrawCircleV(pos, radius, color)
            {
                Source = source,
                ZLayer = zLayer
            }
        );
    }

    /// <summary>
    /// Add Draw Circle Lines Instruction
    /// </summary>
    /// <param name="posX">Position X</param>
    /// <param name="posY">Position Y</param>
    /// <param name="radius">Radius</param>
    /// <param name="borderColor">Border Color</param>
    /// <param name="source">Instruction Source</param>
    /// <param name="zLayer">Z Layer</param>
    public static void DrawCircleLines(
        float posX,
        float posY,
        float radius,
        Color borderColor,
        InstructionSource source,
        float zLayer
    )
    {
        _instructions.Add(
            new DrawCircleLines((int)posX, (int)posY, radius, borderColor)
            {
                Source = source,
                ZLayer = zLayer
            }
        );
    }

    /// <summary>
    /// Add Draw Texture Pro Instruction
    /// </summary>
    /// <param name="texture">Texture</param>
    /// <param name="src">Rectangle Source</param>
    /// <param name="dest">Rectangle Destination</param>
    /// <param name="origin">Origin</param>
    /// <param name="rotation">Rotation</param>
    /// <param name="tint">Color Tint</param>
    /// <param name="source">Instruction Source</param>
    /// <param name="zLayer">Z Layer</param>
    public static void DrawTexture(
        Texture2D texture,
        Rectangle src,
        Rectangle dest,
        Vector2 origin,
        float rotation,
        Color tint,
        InstructionSource source,
        float zLayer
    )
    {
        _instructions.Add(
            new DrawTexturePro(texture, src, dest, origin, rotation, tint)
            {
                Source = source,
                ZLayer = zLayer
            }
        );
    }

    /// <summary>
    /// Add Draw Text Pro Instruction
    /// </summary>
    /// <param name="font">Font</param>
    /// <param name="text">Text</param>
    /// <param name="position">Position</param>
    /// <param name="origin">Origin</param>
    /// <param name="rotation">Rotation</param>
    /// <param name="fontSize">Font Size</param>
    /// <param name="spacing">Spacing</param>
    /// <param name="color">Color</param>
    /// <param name="source">Instruction Source</param>
    /// <param name="zLayer">Z Layer</param>
    public static void DrawText(
        Font font,
        string text,
        Vector2 position,
        Vector2 origin,
        float rotation,
        float fontSize,
        float spacing,
        Color color,
        InstructionSource source,
        float zLayer
    )
    {
        _instructions.Add(
            new DrawTextPro(font, text, position, origin, rotation, fontSize, spacing, color)
            {
                Source = source,
                ZLayer = zLayer
            }
        );
    }

    /// <summary>
    /// Add Draw Text Ex Instruction
    /// </summary>
    /// <param name="font">Font</param>
    /// <param name="text">Text</param>
    /// <param name="position">Position</param>
    /// <param name="fontSize">Font Size</param>
    /// <param name="spacing">Spacing</param>
    /// <param name="color">Color</param>
    /// <param name="source">Instruction Source</param>
    /// <param name="zLayer">Z Layer</param>
    public static void DrawText(
        Font font,
        string text,
        Vector2 position,
        float fontSize,
        float spacing,
        Color color,
        InstructionSource source,
        float zLayer
    )
    {
        _instructions.Add(
            new DrawTextEx(font, text, position, fontSize, spacing, color)
            {
                Source = source,
                ZLayer = zLayer
            }
        );
    }
}
