using System.Collections.Generic;
using System.Dynamic;
using TauCode.Data;

namespace TauCode.Db.Data
{
    // todo: ignore case
    public class DynamicRow : DynamicObject
    {
        private readonly IDictionary<string, object> _values;

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

        public IDictionary<string, object> ToDictionary() => _values;

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

        public void SetValue(string name, object value)
        {
            _values[name] = value;
        }

        public object GetValue(string name)
        {
            return _values[name];
        }

        public bool DeleteValue(string name)
        {
            return _values.Remove(name);
        }

        public bool IsEquivalentTo(object other)
        {
            if (other == null)
            {
                return false; // no object is equiv. to null
            }

            var otherDynamic = new DynamicRow(other);

            if (_values.Count != otherDynamic._values.Count)
            {
                return false;
            }

            foreach (var pair in _values)
            {
                var key = pair.Key;
                var value = pair.Value;

                var otherHas = otherDynamic._values.TryGetValue(key, out var otherValue);
                if (!otherHas)
                {
                    return false;
                }

                var otherEq = Equals(value, otherValue);
                if (!otherEq)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
