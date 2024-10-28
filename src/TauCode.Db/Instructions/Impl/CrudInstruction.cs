namespace TauCode.Db.Instructions.Impl;

public abstract class CrudInstruction : Instruction, ICrudInstruction
{
    protected CrudInstruction(InstructionType type, string? schemaName, string tableName)
        : base(type)
    {
        this.SchemaName = schemaName;
        this.TableName = tableName;
    }

    public string? SchemaName { get; }
    public string TableName { get; }
}