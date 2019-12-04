using System;
using System.Collections.Generic;
using System.Data;

namespace TauCode.Db
{
    public class CrudCommandBuilder
    {
        private readonly IDbConnection _connection;
        //private readonly TableMold _table;

        public CrudCommandBuilder(IDbConnection connection, IEnumerable<string> columnNames)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            //_table = table ?? throw new ArgumentNullException(nameof(table));
        }

        public void SetData(IDictionary<string, object> dataDictionary)
        {
            throw new System.NotImplementedException();
        }

        public IDbCommand GetCommand()
        {
            throw new System.NotImplementedException();
        }

        public string CommandText { get; set; }

        public IDictionary<string, string> GetColumnToParameterMappings()
        {
            throw new NotImplementedException();
        }
    }
}
