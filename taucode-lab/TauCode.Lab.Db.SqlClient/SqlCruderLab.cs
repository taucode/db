using Microsoft.Data.SqlClient;
using System.Data;
using TauCode.Db;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlCruderLab : DbCruderBase
    {
        public SqlCruderLab(SqlConnection connection, string schemaName)
            : base(connection, schemaName)
        {
        }

        public override IDbUtilityFactory Factory => SqlUtilityFactoryLab.Instance;

        protected override IDbValueConverter CreateDbValueConverter(ColumnMold column)
        {
            throw new System.NotImplementedException();
        }

        protected override IDbDataParameter CreateParameter(string tableName, ColumnMold column)
        {
            throw new System.NotImplementedException();
        }
    }
}
