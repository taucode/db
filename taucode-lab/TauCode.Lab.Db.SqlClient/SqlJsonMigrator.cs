﻿using Microsoft.Data.SqlClient;
using System;
using System.Data;
using TauCode.Db;
using TauCode.Db.Data;
using TauCode.Db.Model;
using TauCode.Db.Schema;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlJsonMigrator : DbJsonMigratorBase
    {
        public SqlJsonMigrator(
            SqlConnection connection,
            string schemaName,
            Func<string> metadataJsonGetter,
            Func<string> dataJsonGetter,
            Func<string, bool> tableNamePredicate = null,
            Func<TableMold, DynamicRow, DynamicRow> rowTransformer = null)
            : base(
                connection,
                schemaName ?? SqlTools.DefaultSchemaName,
                metadataJsonGetter,
                dataJsonGetter,
                tableNamePredicate,
                rowTransformer)
        {
        }

        protected SqlConnection SqlConnection => (SqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => SqlUtilityFactory.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) => new SqlSchemaExplorer(this.SqlConnection);
    }
}