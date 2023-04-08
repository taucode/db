namespace TauCode.Db.Instructions.Impl;

public class InsertInstruction : CrudInstruction, IInsertInstruction
{
    public InsertInstruction(string? schemaName, string tableName, IRowSet rowsToInsert)
        : base(InstructionType.Insert, schemaName, tableName)
    {
        this.RowsToInsert = rowsToInsert;
    }

    public IRowSet RowsToInsert { get; }
}