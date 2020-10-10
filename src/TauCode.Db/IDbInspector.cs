using System.Collections.Generic;

namespace TauCode.Db
{
    public interface IDbInspector : IDbUtility
    {
        string SchemaName { get; }

        IReadOnlyList<string> GetSchemaNames();

        IReadOnlyList<string> GetTableNames();
    }
}
