using System;
using System.Data;
using TauCode.Db;
using TauCode.Db.Data;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteJsonMigratorLab : DbJsonMigratorBase
    {
        public SQLiteJsonMigratorLab(IDbConnection connection, string schemaName, Func<string> metadataJsonGetter, Func<string> dataJsonGetter, Func<string, bool> tableNamePredicate = null, Func<TableMold, DynamicRow, DynamicRow> rowTransformer = null) : base(connection, schemaName, metadataJsonGetter, dataJsonGetter, tableNamePredicate, rowTransformer)
        {
        }

        public override IDbUtilityFactory Factory => throw new NotImplementedException();
        protected override bool NeedCheckSchemaExistence => throw new NotImplementedException();
        protected override bool SchemaExists(string schemaName)
        {
            throw new NotImplementedException();
        }
    }
}
