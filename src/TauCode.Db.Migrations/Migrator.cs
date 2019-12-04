//using FluentMigrator.Runner;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Reflection;

//namespace TauCode.Db.Migrations
//{
//    public class Migrator : IMigrator
//    {
//        #region Fields

//        private readonly Dictionary<Type, object> _singletons;

//        #endregion

//        #region Constructor

//        public Migrator()
//        {   
//        }

//        public Migrator(string connectionString, DbProviderName providerName, Assembly migrationsAssembly)
//        {
//            this.ConnectionString = connectionString;
//            this.ProviderName = providerName;
//            this.MigrationsAssembly = migrationsAssembly;
//            _singletons = new Dictionary<Type, object>();
//        }

//        #endregion

//        #region Protected


//        #endregion

//        #region IMigrator Members

//        public string ConnectionString { get; set; }
//        public DbProviderName ProviderName { get; set; }
//        public Assembly MigrationsAssembly { get; set; }


//        public void AddSingleton(Type serviceType, object serviceImplementation)
//        {
//            if (serviceType == null)
//            {
//                throw new ArgumentNullException(nameof(serviceType));
//            }

//            if (serviceImplementation == null)
//            {
//                throw new ArgumentNullException(nameof(serviceImplementation));
//            }

//            _singletons.Add(serviceType, serviceImplementation);
//        }

//        public IReadOnlyDictionary<Type, object> Singletons => _singletons;

//        public void Migrate()
//        {
//            if (string.IsNullOrWhiteSpace(this.ConnectionString))
//            {
//                throw new InvalidOperationException("Connection string must not be empty.");
//            }

//            if (this.MigrationsAssembly == null)
//            {
//                throw new InvalidOperationException("'MigrationsAssembly' must not be null.");
//            }

//            var serviceCollection = new ServiceCollection()
//                // Add common FluentMigrator services
//                .AddFluentMigratorCore();

//            foreach (var pair in _singletons)
//            {
//                var type = pair.Key;
//                var impl = pair.Value;

//                serviceCollection.AddSingleton(type, impl);
//            }

//            var serviceProvider = serviceCollection
//                .ConfigureRunner(rb =>
//                {
//                    switch (this.ProviderName)
//                    {
//                        case DbProviderName.SQLite:
//                            rb.AddSQLite();
//                            break;

//                        case DbProviderName.SqlServer:
//                            rb.AddSqlServer();
//                            break;

//                        default:
//                            throw new ArgumentOutOfRangeException(nameof(this.ProviderName), $"'{this.ProviderName}' not supported.");
//                    }

//                    rb
//                        // Set the connection string
//                        .WithGlobalConnectionString(this.ConnectionString)
//                        // Define the assembly containing the migrations
//                        .ScanIn(this.MigrationsAssembly).For.Migrations();
//                })
//                // Enable logging to console in the FluentMigrator way
//                .AddLogging(lb => lb.AddFluentMigratorConsole())
//                // Build the service provider
//                .BuildServiceProvider(false);

//            // Put the database update into a scope to ensure
//            // that all resources will be disposed.
//            using (serviceProvider.CreateScope())
//            {
//                // Instantiate the runner
//                var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

//                // Execute the migrations
//                runner.MigrateUp();
//            }
//        }

//        #endregion
//    }
//}
