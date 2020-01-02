using System.Collections.Generic;
using System.Linq;

namespace TauCode.Db
{
    public class TableValuesConverter : ITableValuesConverter
    {
        private readonly IDictionary<string, IDbValueConverter> _dbValueConverters;

        public TableValuesConverter(IDictionary<string, IDbValueConverter> dictionary)
        {
            // todo: check args
            _dbValueConverters = dictionary.ToDictionary(x => x.Key, x => x.Value);
        }

        public IDbValueConverter GetColumnConverter(string columnName)
        {
            return _dbValueConverters[columnName]; // todo checks
        }

        public void SetColumnConverter(string columnName, IDbValueConverter dbValueConverter)
        {
            // todo checks

            if (_dbValueConverters.ContainsKey(columnName))
            {
                _dbValueConverters[columnName] = dbValueConverter;
            }
        }
    }
}
