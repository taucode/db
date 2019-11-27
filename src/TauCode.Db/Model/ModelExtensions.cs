using System.Collections.Generic;
using System.Linq;

namespace TauCode.Db.Model
{
    internal static class ModelExtensions
    {
        internal static IDictionary<string, string> ClonePropertiesIfNeeded(this IDbMold dbMold, bool needed)
        {
            return needed ? dbMold.Properties.ToDictionary(x => x.Key, x => x.Value) : new Dictionary<string, string>();
        }
    }
}
