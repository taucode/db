using System.Data;
using TauCode.Db.Instructions;

namespace TauCode.Db;

public abstract class InstructionProcessor : DataUtility, IInstructionProcessor
{
    #region Fields

    private ICruder? _cruder;

    #endregion

    #region ctor

    protected InstructionProcessor()
    {
    }

    protected InstructionProcessor(IDbConnection connection)
        : base(connection)
    {
    }

    #endregion

    #region Private

    private void ProcessInsertInstruction(IInsertInstruction insertInstruction)
    {
        this.Cruder.Connection = this.Connection;

        this.Cruder.InsertRows(
            insertInstruction.SchemaName,
            insertInstruction.TableName,
            insertInstruction.RowsToInsert);
    }

    #endregion

    #region Polymorph

    protected virtual ICruder CreateCruder() => this.Factory.CreateCruder();

    #endregion

    #region IInstructionProcessor Members

    public ICruder Cruder => _cruder ??= this.CreateCruder();

    public void Process(IEnumerable<IInstruction> instructions)
    {
        foreach (var dbInstruction in instructions)
        {
            if (dbInstruction is IInsertInstruction insertInstruction)
            {
                this.ProcessInsertInstruction(insertInstruction);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    public Task ProcessAsync(IEnumerable<IInstruction> instructions, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Action<IInstruction, int>? OnBeforeProcessInstruction
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public Action<IInstruction, int>? OnAfterProcessInstruction
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    #endregion
}
