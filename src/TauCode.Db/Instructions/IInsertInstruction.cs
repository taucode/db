namespace TauCode.Db.Instructions;

public interface IInsertInstruction : ICrudInstruction
{
    IRowSet RowsToInsert { get; }
}