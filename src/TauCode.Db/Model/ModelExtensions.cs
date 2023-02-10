using TauCode.Db.Model.Interfaces;

namespace TauCode.Db.Model;

internal static class ModelExtensions
{
    internal static IDictionary<string, string> ClonePropertiesIfNeeded(this IMold mold, bool needed)
    {
        if (!needed)
        {
            return new Dictionary<string, string>();
        }

        var clonedProperties = mold
            .Properties
            .Where(x => !x.Key.StartsWith("#"))
            .ToDictionary(
                x => x.Key,
                x => x.Value);

        return clonedProperties;
    }
}