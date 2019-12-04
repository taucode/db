using System;
using System.Data;
using System.Linq;
using TauCode.Db.Model;

namespace TauCode.Db.SQLite
{
    public class SQLiteSerializer : DbSerializerBase
    {
        private readonly IDbConnection _connection;

        public SQLiteSerializer(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        //protected override ICruder CreateCruder() => new SQLiteCruder(_connection);

        //protected override IScriptBuilder CreateScriptBuilder()
        //{
        //    return new SQLiteScriptBuilder
        //    {
        //        CurrentOpeningIdentifierDelimiter = '[',
        //    };
        //}

        protected override IUtilityFactory GetFactoryImpl()
        {
            throw new NotImplementedException();
        }

        protected override ParameterInfo GetParameterInfo(TableMold tableMold, string columnName)
        {
            var column = tableMold.Columns.Single(x => x.Name == columnName);
            switch (column.Type.Name.ToLowerInvariant())
            {
                case "uniqueidentifier":
                    return new ParameterInfo
                    {
                        DbType = DbType.String,
                    };

                case "varchar":
                case "text":
                    return new ParameterInfo
                    {
                        DbType = DbType.String,
                    };

                case "datetime":
                    return new ParameterInfo
                    {
                        DbType = DbType.DateTime,
                    };

                case "numeric":
                    return new ParameterInfo
                    {
                        DbType = DbType.Decimal,
                    };

                default:
                    return null;
            }
        }
    }
}
