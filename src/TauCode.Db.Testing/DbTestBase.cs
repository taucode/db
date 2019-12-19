using System;
using System.Data;
using System.Reflection;
using TauCode.Db.FluentMigrations;

namespace TauCode.Db.Testing
{
    public abstract class DbTestBase
    {
        #region Utilities

        protected abstract string GetDbProviderName();
        protected abstract string GetConnectionString();
        protected IUtilityFactory UtilityFactory { get; set; }
        protected IDbConnection Connection { get; set; }
        protected IDbInspector DbInspector { get; set; }
        protected ICruder Cruder { get; set; }
        protected IScriptBuilder ScriptBuilder { get; set; }
        protected IDbSerializer DbSerializer { get; set; }

        #endregion

        #region Utility Creation

        protected virtual IUtilityFactory
            GetUtilityFactory() => DbUtils.GetUtilityFactory(this.GetDbProviderName());

        protected virtual IDbConnection CreateConnection() => DbUtils.CreateConnection(this.GetDbProviderName());

        protected virtual IDbInspector CreateDbInspector()
        {
            if (this.UtilityFactory == null)
            {
                throw new InvalidOperationException($"'{nameof(UtilityFactory)}' is null.");
            }

            if (this.Connection == null)
            {
                throw new InvalidOperationException($"'{nameof(Connection)}' is null.");
            }

            return this.UtilityFactory.CreateDbInspector(this.Connection);
        }

        protected virtual IDbSerializer CreateDbSerializer()
        {
            if (this.UtilityFactory == null)
            {
                throw new InvalidOperationException($"'{nameof(UtilityFactory)}' is null.");
            }

            if (this.Connection == null)
            {
                throw new InvalidOperationException($"'{nameof(Connection)}' is null.");
            }

            return this.UtilityFactory.CreateDbSerializer(this.Connection);
        }

        protected virtual FluentDbMigrator CreateFluentMigrator(Assembly migrationsAssembly)
        {
            return new FluentDbMigrator(this.GetDbProviderName(), this.GetConnectionString(), migrationsAssembly);
        }

        #endregion

        #region Fixture Support

        protected virtual void OneTimeSetUpImpl()
        {
            this.UtilityFactory = this.GetUtilityFactory();

            this.Connection = this.CreateConnection();
            this.Connection.ConnectionString = this.GetConnectionString();
            this.Connection.Open();

            this.DbInspector = this.CreateDbInspector();
            this.DbSerializer = this.CreateDbSerializer();
            this.Cruder = this.DbSerializer.Cruder;
            this.ScriptBuilder = this.Cruder.ScriptBuilder;
        }

        protected virtual void OneTimeTearDownImpl()
        {
            this.Connection.Dispose();
            this.Connection = null;
        }

        protected virtual void SetUpImpl()
        {
        }

        protected virtual void TearDownImpl()
        {
        }

        #endregion
    }
}
