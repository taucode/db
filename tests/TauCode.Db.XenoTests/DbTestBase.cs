using NUnit.Framework;
using System;
using System.Data;
using System.Reflection;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Building.SqlServer;
using TauCode.Db.Utils.Crud;
using TauCode.Db.Utils.Crud.SqlServer;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.SqlServer;
using TauCode.Db.Utils.Serialization;
using TauCode.Db.Utils.Serialization.SqlServer;

namespace TauCode.Db.XenoTests
{
    [TestFixture]
    public abstract class DbTestBase
    {
        protected IDbConnection Connection { get; private set; }
        protected string ConnectionString { get; private set; }
        protected IDbInspector DbInspector { get; private set; }
        protected IScriptBuilder ScriptBuilder { get; private set; }
        protected ICruder Cruder { get; private set; }

        protected IDataSerializer DataSerializer { get; private set; }

        protected abstract Assembly GetMigrationAssembly();
        protected abstract TargetDbType GetTargetDbType();
        protected abstract string GetConnectionString();
        protected abstract IDbConnection CreateDbConnection(string connectionString);

        protected virtual IDbInspector CreateDbInspector(IDbConnection connection)
        {
            var dbType = this.GetTargetDbType();
            switch (dbType)
            {
                case TargetDbType.SqlServer:
                    return new SqlServerInspector(connection);

                default:
                    throw new NotSupportedException($"{dbType} is not supported.");
            }
        }

        protected virtual IScriptBuilder CreateScriptBuilder()
        {
            var dbType = this.GetTargetDbType();
            switch (dbType)
            {
                case TargetDbType.SqlServer:
                    return new SqlServerScriptBuilder();

                default:
                    throw new NotSupportedException($"{dbType} is not supported.");
            }
        }

        protected virtual ICruder CreateCruder()
        {
            var dbType = this.GetTargetDbType();
            switch (dbType)
            {
                case TargetDbType.SqlServer:
                    return new SqlServerCruder();

                default:
                    throw new NotSupportedException($"{dbType} is not supported.");
            }
        }

        protected void DeserializeDbJson(string json)
        {
            this.DataSerializer.DeserializeDb(this.Connection, json);
        }

        [OneTimeSetUp]
        public void OneTimeSetUpDbTestBase()
        {
            this.ConnectionString = this.GetConnectionString();
            this.Connection = this.CreateDbConnection(this.ConnectionString);
            this.Connection.Open();
            this.DbInspector = this.CreateDbInspector(this.Connection);
            this.ScriptBuilder = this.CreateScriptBuilder();
            this.Cruder = this.CreateCruder();
            this.DataSerializer = this.CreateDataSerializer();
        }

        [OneTimeTearDown]
        public void OneTimeTearDownDbTestBase()
        {
            if (this.Connection != null)
            {
                this.Connection.Dispose();
                this.Connection = null;
            }
        }

        protected virtual IDataSerializer CreateDataSerializer()
        {
            var dbType = this.GetTargetDbType();
            switch (dbType)
            {
                case TargetDbType.SqlServer:
                    return new SqlServerDataSerializer();

                default:
                    throw new NotSupportedException($"{dbType} is not supported.");
            }
        }


        protected virtual void PurgeDb()
        {
            this.DbInspector.ClearDb();
            var tables = this.DbInspector.GetOrderedTableMolds(false);

            using (var command = this.Connection.CreateCommand())
            {
                foreach (var table in tables)
                {
                    var sql = this.ScriptBuilder.BuildDropTableSql(table.Name);
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
        }

        protected virtual void Migrate()
        {
            var migrator = new Migrator(this.ConnectionString, this.GetTargetDbType(), this.GetMigrationAssembly());
            migrator.Migrate();
        }

        protected void InsertRow(string tableName, object row)
        {
            this.Cruder.InsertRow(this.Connection, tableName, row);
        }

        protected bool DeleteRow(string tableName, object id)
        {
            return this.Cruder.DeleteRow(this.Connection, tableName, id);
        }
    }
}
