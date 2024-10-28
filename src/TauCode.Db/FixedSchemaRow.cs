using System.Collections;

namespace TauCode.Db;

public class FixedSchemaRow : IRow
{
    #region Fields

    private readonly IReadOnlyDictionary<string, int> _schema;
    private readonly object[] _values;

    #endregion

    #region ctor

    public FixedSchemaRow(IReadOnlyDictionary<string, int> schema)
    {
        _schema = schema;
        _values = new object[_schema.Count];
    }

    #endregion

    #region Public

    public IReadOnlyDictionary<string, int> Schema => _schema;

    #endregion

    #region IDictionary<string, object> Members

    public object this[string key]
    {
        get
        {
            var index = _schema[key];
            var value = _values[index];
            return value;
        }
        set
        {
            var index = _schema[key];
            _values[index] = value!;
        }
    }

    public ICollection<string> Keys => ((Dictionary<string, int>)_schema).Keys;

    public ICollection<object> Values => _values;

    public void Add(string key, object value)
    {
        throw new InvalidOperationException();
    }

    public bool ContainsKey(string key)
    {
        return _schema.ContainsKey(key);
    }

    public bool Remove(KeyValuePair<string, object> item)
    {
        throw new InvalidOperationException();
    }

    public bool TryGetValue(string key, out object value)
    {
        throw new NotSupportedException();
    }

    #endregion

    #region ICollection<KeyValuePair<string, object>> Members

    public int Count => _values.Length;

    public bool IsReadOnly => false;

    public void Add(KeyValuePair<string, object> item)
    {
        throw new InvalidOperationException();
    }

    public void Clear()
    {
        throw new InvalidOperationException();
    }

    public bool Contains(KeyValuePair<string, object> item)
    {
        throw new NotSupportedException();
    }

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        throw new NotSupportedException();
    }

    public bool Remove(string key)
    {
        throw new InvalidOperationException();
    }

    #endregion

    #region IEnumerable<KeyValuePair<string, object>>

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        foreach (var fieldName in _schema.Keys)
        {
            var fieldValue = this[fieldName];
            var pair = new KeyValuePair<string, object>(fieldName, fieldValue);
            yield return pair;
        }
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion
}