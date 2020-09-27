using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Extensions;

namespace TauCode.Db
{
    public class DbTableValuesConverter : IDbTableValuesConverter
    {
        private readonly IDictionary<string, IDbValueConverter> _dbValueConverters;

        public DbTableValuesConverter(IDictionary<string, IDbValueConverter> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            _dbValueConverters = dictionary.ToDictionary(x => x.Key, x => x.Value);
        }

        public IDbValueConverter GetColumnConverter(string columnName)
        {
            var converter = _dbValueConverters.GetOrDefault(columnName.ToLowerInvariant());

            if (converter == null)
            {
                throw new KeyNotFoundException($"Value converter not found for column '{columnName}'.");
            }

            return converter;
        }

        public void SetColumnConverter(string columnName, IDbValueConverter dbValueConverter)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            if (dbValueConverter == null)
            {
                throw new ArgumentNullException(nameof(dbValueConverter));
            }

            if (_dbValueConverters.ContainsKey(columnName))
            {
                _dbValueConverters[columnName] = dbValueConverter;
            }
        }
    }
}
