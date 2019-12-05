namespace TauCode.Db.Testing
{
    public abstract class DbTestBase
    {/*
        #region Utilities

        protected abstract string GetDbProviderName();
        protected abstract string GetConnectionString();
        protected IUtilityFactory UtilityFactory { get; set; }
        protected IDbConnection Connection { get; set; }
        protected IDbInspector DbInspector { get; set; }
        protected ICruder Cruder { get; set; }
        protected IScriptBuilder ScriptBuilder { get; set; }
        protected IDbSerializer DbSerializer { get; set; }
        protected IDbMigrator DbMigrator { get; set; }

        #endregion

        #region Utility Creation

        protected virtual IUtilityFactory
            GetUtilityFactory() => // DbUtils.GetUtilityFactory(this.GetDbProviderName());
            throw new NotImplementedException();

        protected virtual IDbConnection CreateConnection() => //DbUtils.CreateConnection(this.GetDbProviderName());
            throw new NotImplementedException();

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

        protected virtual ICruder CreateCruder()
        {
            if (this.UtilityFactory == null)
            {
                throw new InvalidOperationException($"'{nameof(UtilityFactory)}' is null.");
            }

            if (this.Connection == null)
            {
                throw new InvalidOperationException($"'{nameof(Connection)}' is null.");
            }

            return this.UtilityFactory.CreateCruder(this.Connection);
        }

        protected virtual IScriptBuilder GetScriptBuilder()
        {
            if (this.Cruder == null)
            {
                throw new InvalidOperationException($"'{nameof(Cruder)}' is null.");
            }

            return this.Cruder.ScriptBuilder;
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

        protected abstract IDbMigrator CreateDbMigrator();

        protected virtual FluentDbMigrator CreateFluentMigrator(Assembly migrationsAssembly)
        {
            return new FluentDbMigrator(this.GetDbProviderName(), this.GetConnectionString(), migrationsAssembly);
        }

        #endregion

        #region Fixture Support

        protected virtual void OneTimeSetUpImpl()
        {

        }

        protected virtual void OneTimeTearDownImpl()
        {
        }

        protected virtual void SetUpImpl()
        {
        }

        protected virtual void TearDownImpl()
        {
        }

        #endregion
        */
    }
}
