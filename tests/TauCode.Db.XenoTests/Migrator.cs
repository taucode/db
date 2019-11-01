using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace TauCode.Db.XenoTests
{
    public class Migrator
    {
        public string ConnectionString { get; set; }
        public TargetDbType TargetDbType { get; set; }
        public Assembly MigrationsAssembly { get; set; }

        public Migrator()
        {
        }

        public Migrator(string connectionString, TargetDbType targetDbType, Assembly migrationsAssembly)
        {
            this.ConnectionString = connectionString;
            this.TargetDbType = targetDbType;
            this.MigrationsAssembly = migrationsAssembly;
        }

        public void Migrate()
        {
            if (string.IsNullOrWhiteSpace(this.ConnectionString))
            {
                throw new InvalidOperationException("Connection string must not be empty.");
            }

            if (!Enum.GetValues(typeof(TargetDbType))
                    .Cast<TargetDbType>()
                    .Contains(this.TargetDbType))
            {
                throw new InvalidOperationException("Valid target DB type must be provided.");
            }

            if (this.MigrationsAssembly == null)
            {
                throw new InvalidOperationException("'MigrationsAssembly' must not be null.");
            }

            var serviceProvider = new ServiceCollection()
                // Add common FluentMigrator services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb =>
                {
                    switch (this.TargetDbType)
                    {
                        case TargetDbType.SQLite:
                            rb.AddSQLite();
                            break;

                        case TargetDbType.SqlServer:
                            rb.AddSqlServer();
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(this.TargetDbType), $"'{this.TargetDbType}' not supported.");
                    }

                    rb
                        // Set the connection string
                        .WithGlobalConnectionString(this.ConnectionString)
                        // Define the assembly containing the migrations
                        .ScanIn(this.MigrationsAssembly).For.Migrations();
                })
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false);

            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            using (serviceProvider.CreateScope())
            {
                // Instantiate the runner
                var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

                // Execute the migrations
                runner.MigrateUp();
            }
        }
    }
}
