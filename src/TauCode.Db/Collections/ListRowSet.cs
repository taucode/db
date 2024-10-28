namespace TauCode.Db.Collections;

public class ListRowSet : RowSetBase
{
    private readonly List<FixedSchemaRow> _rows;

    public int Count => _rows.Count;

    public ListRowSet(IEnumerable<string> fieldNames)
        : base(fieldNames)
    {
        _rows = new List<FixedSchemaRow>();
    }

    public FixedSchemaRow Add()
    {
        if (this.FieldOrders == null)
        {
            throw new NotImplementedException();
        }

        var row = new FixedSchemaRow(this.FieldOrders);
        _rows.Add(row);

        return row;
    }

    public override IEnumerator<FixedSchemaRow> GetEnumerator()
    {
        return _rows.GetEnumerator();
    }
}