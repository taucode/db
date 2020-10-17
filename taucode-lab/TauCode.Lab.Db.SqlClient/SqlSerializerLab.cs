﻿using Microsoft.Data.SqlClient;
using TauCode.Db;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlSerializerLab : DbSerializerBase
    {
        public SqlSerializerLab(SqlConnection connection, string schema)
            : base(connection, schema)

        {
        }

        public override IDbUtilityFactory Factory => SqlUtilityFactoryLab.Instance;
    }
}