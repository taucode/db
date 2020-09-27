using System.Collections.Generic;

namespace TauCode.Db
{
    public interface IDbInspector : IDbUtility
    {
        string Schema { get; }

        IReadOnlyList<string> GetSchemata();

        IReadOnlyList<string> GetTableNames();
    }
}
