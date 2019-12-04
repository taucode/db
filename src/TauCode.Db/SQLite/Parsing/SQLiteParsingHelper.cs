using TauCode.Db.Model;

namespace TauCode.Db.SQLite.Parsing
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

        internal static void SetIsCreationFinalized(this IndexMold indexMold, bool value)
        {
            indexMold.Properties["is-creation-finalized"] = value.ToString();
        }

        internal static bool GetIsCreationFinalized(this IndexMold indexMold)
        {
            return bool.Parse(indexMold.Properties["is-creation-finalized"]);
        }
    }
}
