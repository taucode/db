using System.Data;
using System.Linq;
using TauCode.Db.Model;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Building.SQLite;
using TauCode.Db.Utils.Crud;
using TauCode.Db.Utils.Crud.SQLite;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.SQLite;

namespace TauCode.Db.Utils.Serialization.SQLite
{
    public class SQLiteDataSerializer : DataSerializerBase
    {
        public SQLiteDataSerializer()
        {
        }

        protected override ICruder CreateCruder()
        {
            return new SQLiteCruder();
        }

        protected override IScriptBuilder CreateScriptBuilder()
        {
            return new SQLiteScriptBuilder
            {
                CurrentOpeningIdentifierDelimiter = '[',
            };
        }

        protected override IDbInspector GetDbInspector(IDbConnection connection)
        {
            return new SQLiteInspector(connection);
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
                    return new ParameterInfo
                    {
                        DbType = DbType.String,
                    };

                default:
                    return null;
            }
        }
    }
}
