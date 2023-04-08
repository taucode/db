using System.Collections;

namespace TauCode.Db;

public abstract class RowSetBase : IRowSet
{
    #region Fields

    private IReadOnlyList<string>? _fieldNames;
    private Dictionary<string, int>? _fieldOrders;

    #endregion

    #region ctor

    protected RowSetBase()
    {
    }

    protected RowSetBase(IEnumerable<string> fieldNames)
    {
        this.InitFieldNames(fieldNames);
    }

    #endregion

    #region Private

    private void InitFieldNames(IEnumerable<string> fieldNames)
    {
        if (fieldNames == null)
        {
            throw new ArgumentNullException(nameof(fieldNames));
        }

        if (_fieldNames != null)
        {
            throw new InvalidOperationException($"'{nameof(IRowSet.FieldNames)}' is already set.");
        }

        // todo: check fieldNames (uniqueness etc)
        _fieldNames = fieldNames.ToList();
        _fieldOrders = new Dictionary<string, int>();
        var order = 0;
        foreach (var fieldName in _fieldNames)
        {
            _fieldOrders.Add(fieldName, order);
            order++;
        }
    }

    #endregion

    #region Public

    public IReadOnlyDictionary<string, int>? FieldOrders => _fieldOrders;

    #endregion

    #region IRowSet Members

    public IReadOnlyList<string>? FieldNames
    {
        get => _fieldNames;
        set => this.InitFieldNames(value!);
    }

    #endregion

    #region IEnumerator<IRow> Members

    public abstract IEnumerator<IRow> GetEnumerator();

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion
}