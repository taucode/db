namespace TauCode.Db.Model.Interfaces;

public interface IForeignKeyMold : IConstraintMold
{
    IList<string> ColumnNames { get; }

    string? ReferencedTableSchemaName { get; set; }
    string ReferencedTableName { get; set; }
    IList<string> ReferencedColumnNames { get; }
}