namespace TauCode.Db;

public abstract class InstructionReader : IInstructionReader
{
    public abstract IEnumerable<IInstruction> ReadInstructions(TextReader textReader);
}