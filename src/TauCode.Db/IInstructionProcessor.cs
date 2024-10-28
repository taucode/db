namespace TauCode.Db;

public interface IInstructionProcessor : IUtility
{
    ICruder Cruder { get; }

    void Process(IEnumerable<IInstruction> instructions);
    Task ProcessAsync(IEnumerable<IInstruction> instructions, CancellationToken cancellationToken = default);

    Action<IInstruction, int>? OnBeforeProcessInstruction { get; set; }
    Action<IInstruction, int>? OnAfterProcessInstruction { get; set; }
}
