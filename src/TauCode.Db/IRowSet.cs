namespace TauCode.Db;

public interface IRowSet : IEnumerable<IRow>
{
    IReadOnlyList<string>? FieldNames { get; set; }
}
