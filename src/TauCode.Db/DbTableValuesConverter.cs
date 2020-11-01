using System;
using System.Collections.Generic;
using System.Linq;

namespace TauCode.Db
{
    // todo regions
    public class DbTableValuesConverter : IDbTableValuesConverter
    {
        private readonly Dictionary<string, IDbValueConverter> _dbValueConverters;

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
            var converter = _dbValueConverters.GetValueOrDefault(columnName);

            if (converter == null)
            {
                throw new KeyNotFoundException($"Value converter not found for column '{columnName}'.");
            }

            return converter;
        }

        public void SetColumnConverter(string columnName, IDbValueConverter dbValueConverter)
        {
            // todo: checks, ut-s.

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

            else throw new ArgumentException($"Column '{columnName}' does not exist.");
        }
    }
}
