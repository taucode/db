using System.Collections.Generic;

namespace TauCode.Db
{
    public interface IDbInspector : IDbUtility
    {
        IReadOnlyList<string> GetTableNames(bool? independentFirst = null);
    }
}
