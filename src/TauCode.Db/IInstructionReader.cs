namespace TauCode.Db;

public interface IInstructionReader
{
    IEnumerable<IInstruction> ReadInstructions(TextReader textReader);
}