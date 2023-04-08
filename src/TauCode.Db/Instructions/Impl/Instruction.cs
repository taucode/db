namespace TauCode.Db.Instructions.Impl;

public abstract class Instruction : IInstruction
{
    protected Instruction(InstructionType type)
    {
        this.Type = type;
    }

    public InstructionType Type { get; }
}