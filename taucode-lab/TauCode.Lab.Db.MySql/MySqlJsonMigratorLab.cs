using System;
using System.Data;
using TauCode.Db;
using TauCode.Db.Data;
using TauCode.Db.Model;

// todo everything
namespace TauCode.Lab.Db.MySql
{
    public class MySqlJsonMigratorLab : DbJsonMigratorBase
    {
        public MySqlJsonMigratorLab(IDbConnection connection, string schemaName, Func<string> metadataJsonGetter, Func<string> dataJsonGetter, Func<string, bool> tableNamePredicate = null, Func<TableMold, DynamicRow, DynamicRow> rowTransformer = null) : base(connection, schemaName, metadataJsonGetter, dataJsonGetter, tableNamePredicate, rowTransformer)
        {
        }

        public override IDbUtilityFactory Factory => MySqlUtilityFactoryLab.Instance;

        protected override bool NeedCheckSchemaExistence => throw new NotImplementedException();
        protected override bool SchemaExists(string schemaName)
        {
            throw new NotImplementedException();
        }
    }
}
