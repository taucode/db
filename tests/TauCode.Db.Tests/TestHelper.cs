using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;

namespace TauCode.Db.Tests
{
    internal static class TestHelper
    {
        internal static void ExecuteNonQuery(this IDbConnection connection, string sql)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        internal static IEnumerable<T> Check<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            foreach (var item in collection)
            {
                if (!predicate(item))
                {
                    Assert.Fail("Check failed");
                }
            }

            return collection;
        }

        internal static void AddParameterWithValue(this IDbCommand command, string parameterName, object parameterValue)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            command.Parameters.Add(parameter);
        }
    }
}
