using System;
using System.Collections.Generic;
using TauCode.Db.Model;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Utils.Inspection.SQLite
{
    internal static class SQLiteParsingHelper
    {
        internal static void SetLastConstraintName(this TableMold tableMold, string value)
        {
            tableMold.Properties["last-constraint-name"] = value;
        }

        internal static string GetLastConstraintName(this TableMold tableMold)
        {
            return tableMold.Properties["last-constraint-name"];
        }

        internal static void SetIsPrimaryKey(this ColumnMold columnMold, bool value)
        {
            columnMold.Properties["is-primary-key"] = value.ToString();
        }

        internal static void SetIsCreationFinalized(this IndexMold indexMold, bool value)
        {
            indexMold.Properties["is-creation-finalized"] = value.ToString();
        }

        internal static bool GetIsCreationFinalized(this IndexMold indexMold)
        {
            return bool.Parse(indexMold.Properties["is-creation-finalized"]);
        }

        internal static void SetIsAutoIncrement(this ColumnMold columnMoldd, bool value)
        {
            columnMoldd.Properties["is-autoincrement"] = value.ToString();
        }

        internal static bool GetIsAutoIncrement(this ColumnMold columnMold)
        {
            return bool.Parse(columnMold.Properties["is-autoincrement"]);
        }

        internal static void InitPrimaryKey(this TableMold tableMold)
        {
            if (tableMold.PrimaryKey != null)
            {
                return;
            }

            foreach (var column in tableMold.Columns)
            {
                var isPrimaryKeyString = column.Properties.GetOrDefault("is-primary-key");
                if (isPrimaryKeyString != null)
                {
                    var isPrimaryKey = bool.Parse(isPrimaryKeyString);
                    if (tableMold.PrimaryKey != null)
                    {
                        throw new Exception("Failed to parse SQLite statement.");
                    }

                    tableMold.PrimaryKey = new PrimaryKeyMold
                    {
                        Columns = new List<IndexColumnMold>
                        {
                            new IndexColumnMold
                            {
                                Name = column.Name,
                            },
                        },
                    };
                    return;
                }
            }

            throw new Exception("Failed to parse SQLite statement.");
        }
    }
}
