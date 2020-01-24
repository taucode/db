using System.Collections.Generic;

namespace TauCode.Db
{
    public interface IDbInspector : IUtility
    {
        IReadOnlyList<string> GetTableNames(bool? independentFirst = null);
    }
}
