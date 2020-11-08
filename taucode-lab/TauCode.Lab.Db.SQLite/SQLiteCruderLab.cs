using System;
using System.Data;
using System.Data.SQLite;
using TauCode.Db;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteCruderLab : DbCruderBase
    {
        public SQLiteCruderLab(SQLiteConnection connection)
            : base(connection, null)
        {
        }

        public override IDbUtilityFactory Factory => throw new NotImplementedException();
        protected override IDbValueConverter CreateDbValueConverter(ColumnMold column)
        {
            throw new System.NotImplementedException();
        }

        protected override IDbDataParameter CreateParameter(string tableName, ColumnMold column)
        {
            throw new System.NotImplementedException();
        }

        protected override bool NeedCheckSchemaExistence => throw new NotImplementedException();

        protected override bool SchemaExists(string schemaName)
        {
            throw new System.NotImplementedException();
        }
    }
}
