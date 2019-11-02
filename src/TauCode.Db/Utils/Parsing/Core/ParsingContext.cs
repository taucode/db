using System;
using System.Collections.Generic;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Utils.Parsing.Core
{
    public class ParsingContext
    {
        private readonly Dictionary<string, object> _properties;

        public ParsingContext()
        {
            _properties = new Dictionary<string, object>();
        }

        public object Result { get; private set; }

        public void SetResult(object result)
        {
            if (this.Result != null)
            {
                throw new InvalidOperationException("Current 'Result' is not null.");
            }

            this.Result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public void ResetResult()
        {
            if (this.Result == null)
            {
                throw new InvalidOperationException("Current 'Result' is null.");
            }

            this.Result = null;
        }

        public void AddProperty(string propertyName, object propertyValue)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException($"'{nameof(propertyName)}' cannot be empty.", nameof(propertyName));
            }

            if (propertyValue == null)
            {
                throw new ArgumentNullException(nameof(propertyValue));
            }

            _properties.Add(propertyName, propertyValue);
        }

        public object GetProperty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Property name cannot be empty.", nameof(propertyName));
            }

            return _properties.GetOrDefault(propertyName);
        }

        public T GetProperty<T>(string propertyName)
        {
            var value = this.GetProperty(propertyName);

            if (value is T castedToT)
            {
                return castedToT;
            }

            throw new ArgumentException($"Property with name '{propertyName}' and type '{typeof(T).FullName}' not found.", nameof(propertyName));
        }

        public void RemoveProperty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Property name must not be empty.", nameof(propertyName));
            }

            var deleted = _properties.Remove(propertyName);
            if (!deleted)
            {
                throw new ArgumentException($"Property '{propertyName}' does not exist.", nameof(propertyName));
            }
        }

        public int GetPropertyCount()
        {
            return _properties.Count;
        }

        public bool ContainsProperty(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            return _properties.ContainsKey(propertyName);
        }
    }
}
