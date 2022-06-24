using System.Collections.Generic;
using System.Dynamic;
using TauCode.Data;

namespace TauCode.Db.Data
{
    public class DynamicRow : DynamicObject
    {
        #region Fields

        private readonly IDictionary<string, object> _values;

        #endregion

        #region Constructor

        public DynamicRow(object original = null)
        {
            IDictionary<string, object> values;

            if (original is DynamicRow dynamicRow)
            {
                values = new ValueDictionary(dynamicRow.ToDictionary());
            }
            else
            {
                values = new ValueDictionary(original);
            }

            _values = values;
        }

        #endregion

        #region Overridden

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var name = binder.Name;
            _values[name] = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = binder.Name;
            return _values.TryGetValue(name, out result);
        }

        public override IEnumerable<string> GetDynamicMemberNames() => _values.Keys;

        #endregion

        #region Public

        public IDictionary<string, object> ToDictionary() => _values;

        public void SetProperty(string propertyName, object propertyValue) => _values[propertyName] = propertyValue;

        public object GetProperty(string propertyName) => _values[propertyName];

        public bool RemoveProperty(string propertyName) => _values.Remove(propertyName);

        public bool ContainsProperty(string propertyName) => _values.ContainsKey(propertyName);

        public void Clear() => _values.Clear();

        #endregion
    }
}
