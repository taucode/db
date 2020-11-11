using MySql.Data.MySqlClient;
using System;
using System.Data;
using TauCode.Db;
using TauCode.Db.Exceptions;

namespace TauCode.Lab.Db.MySql
{
    // todo: schema_name must be null, check & ut it.
    // todo: regions
    // todo clean
    public class MySqlUtilityFactoryLab : IDbUtilityFactory
    {
        public static MySqlUtilityFactoryLab Instance { get; } = new MySqlUtilityFactoryLab();

        private MySqlUtilityFactoryLab()
        {
        }

        public IDbDialect GetDialect() => MySqlDialectLab.Instance;

        public IDbScriptBuilder CreateScriptBuilder(string schemaName) => new MySqlScriptBuilderLab(schemaName);

        public IDbConnection CreateConnection() => new MySqlConnection();

        public IDbInspector CreateInspector(IDbConnection connection, string schemaName)
        {
            //this.CheckConnectionAndSchemaName(connection, schemaName);
            return new MySqlInspectorLab((MySqlConnection)connection);
        }

        public IDbTableInspector CreateTableInspector(IDbConnection connection, string schemaName, string tableName)
        {
            //this.CheckConnectionAndSchemaName(connection, schemaName);
            return new MySqlTableInspectorLab((MySqlConnection)connection, tableName);
        }

        public IDbCruder CreateCruder(IDbConnection connection, string schemaName)
        {
            //this.CheckConnectionAndSchemaName(connection, schemaName);
            return new MySqlCruderLab((MySqlConnection)connection);
        }

        public IDbSerializer CreateSerializer(IDbConnection connection, string schemaName)
        {
            //this.CheckConnectionAndSchemaName(connection, schemaName);
            return new MySqlSerializerLab((MySqlConnection)connection);
        }
            

        //private void CheckConnectionAndSchemaName(IDbConnection connection, string schemaName)
        //{
        //    if (connection == null)
        //    {
        //        throw new ArgumentNullException(nameof(connection));
        //    }

        //    if (connection.GetType() != typeof(MySqlConnection))
        //    {
        //        throw new ArgumentException($"'{connection}' must be of type '{typeof(MySqlConnection).FullName}'.");
        //    }

        //    if (schemaName == null)
        //    {
        //        throw new ArgumentNullException(nameof(schemaName));
        //    }

        //    var mySqlConnection = (MySqlConnection)connection;
        //    //var connectionSchemaName = mySqlConnection.GetSchemaName();

        //    if (connectionSchemaName == null)
        //    {
        //        var supposedSchemaName = MySqlToolsLab.TryExtractSchemaName(connection.ConnectionString);
        //        if (supposedSchemaName != null)
        //        {
        //            throw DbTools.CreateSchemaDoesNotExistException(supposedSchemaName);
        //        }

        //        throw new TauDbException("Connection's schema does not exist.");
        //    }

        //    if (connectionSchemaName != schemaName)
        //    {
        //        throw new ArgumentException($"Value of '{nameof(schemaName)}' ('{schemaName}') differs from connection's schema name '{connectionSchemaName}'.");
        //    }
        //}
    }
}
