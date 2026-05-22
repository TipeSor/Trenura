namespace Engine;

/// <summary>
/// Struct which represents renderer instruction
/// </summary>
internal abstract record Instruction
{
    /// <summary>
    /// if Instruction is for entities or ui
    /// </summary>
    public InstructionSource Source;

    /// <summary>
    /// Z Layer of Instruction
    /// </summary>
    public float ZLayer;

    /// <summary>
    /// Executes the instruction.
    /// </summary>
    internal virtual void Execute() { }
}
