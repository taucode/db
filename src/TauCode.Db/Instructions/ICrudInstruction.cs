namespace TauCode.Db.Instructions;

public interface ICrudInstruction : IInstruction
{
    string? SchemaName { get; }
    string TableName { get; }
}